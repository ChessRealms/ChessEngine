using ChessRealms.ChessEngine.Core.Attacks;
using ChessRealms.ChessEngine.Core.Builders;
using ChessRealms.ChessEngine.Core.Constants;
using ChessRealms.ChessEngine.Core.Extensions;
using ChessRealms.ChessEngine.Core.Types;
using ChessRealms.ChessEngine.Core.Types.Enums;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

using static ChessRealms.ChessEngine.Core.Constants.ChessConstants;

namespace ChessRealms.ChessEngine;

public ref struct ChessBoard
{
    private readonly BitBoard[,] _pieces;    
    private readonly BitBoard[] _occupancies;

    public SquareIndex Enpassant { get; set; }

    public PieceColor CurrentColor { get; set; }

    public Castling CastlingState { get; set; }

    public int HalfMoveClock { get; set; }

    public int FullMoveNumber { get; set; }

    public ChessBoard()
    {
        _pieces = new BitBoard[2, 6];
        _occupancies = new BitBoard[3];
        
        Enpassant = SquareIndex.None;
        CurrentColor = PieceColor.White;
        CastlingState = 0;
    }

    #region Piece Management

    public readonly bool TryGetPieceAt(SquareIndex square, out Piece piece)
    {
        if (!_occupancies[COLOR_NONE].TryGetBitAt(square, out _))
        {
            piece = Piece.Empty;
            return false;
        }

        int colorVal = _occupancies[COLOR_BLACK].GetBitAt(square) != 0 
            ? COLOR_BLACK 
            : COLOR_WHITE;
        
        for (int pieceVal = PIECE_PAWN; pieceVal < PIECE_NONE; ++pieceVal)
        {
            if (_pieces[colorVal, pieceVal].GetBitAt(square) != 0)
            {
                piece = new Piece(pieceVal, colorVal);
                return true;
            }
        }

        piece = Piece.Empty;
        return false;
    }

    public readonly void SetPieceAt(SquareIndex square, in Piece piece)
    {
        _pieces[piece.Color, piece.Type] = _pieces[piece.Color, piece.Type].SetBitAt(square);
        _occupancies[piece.Color] = _occupancies[piece.Color].SetBitAt(square);
        _occupancies[COLOR_NONE] = _occupancies[COLOR_NONE].SetBitAt(square);
    }

    public readonly void SetPieceAt(SquareIndex square, PieceColor color, PieceType piece)
    {
        int colorIndex = (int)color;
        int pieceIndex = (int)piece;
        
        _pieces[colorIndex, pieceIndex] = _pieces[colorIndex, pieceIndex].SetBitAt(square);
        _occupancies[colorIndex] = _occupancies[colorIndex].SetBitAt(square);
        _occupancies[COLOR_NONE] = _occupancies[COLOR_NONE].SetBitAt(square);
    }

    public readonly void RemovePieceAt(SquareIndex square, in Piece piece)
    {
        _pieces[piece.Color, piece.Type] = _pieces[piece.Color, piece.Type].PopBitAt(square);
        _occupancies[piece.Color] = _occupancies[piece.Color].PopBitAt(square);
        _occupancies[COLOR_NONE] = _occupancies[COLOR_NONE].PopBitAt(square);
    }

    public readonly void MovePiece(SquareIndex source, SquareIndex target, in Piece piece)
    {
        RemovePieceAt(source, in piece);
        SetPieceAt(target, in piece);
    }

    #endregion

    #region Get Moves
    /// <summary>
    /// Determine if square is attacked by specified side.
    /// </summary>
    /// <param name="square"> Square to check. </param>
    /// <param name="attacker"> Attacker color. </param>
    /// <returns> <see langword="true"/> if attacked, otherwise <see langword="false"/>. </returns>
    internal readonly bool IsSquareAttacked(SquareIndex square, int attacker)
    {
        return (PawnAttacks.AttackMasks[COLOR_NONE + ~attacker][square] & _pieces[attacker, PIECE_PAWN]) != 0
            || (KnightAttacks.AttackMasks[square] & _pieces[attacker, PIECE_KNIGHT]) != 0
            || (BishopAttacks.GetSliderAttack(square, _occupancies[COLOR_NONE]) & _pieces[attacker, PIECE_BISHOP]) != 0
            || (RookAttacks.GetSliderAttack(square, _occupancies[COLOR_NONE]) & _pieces[attacker, PIECE_ROOK]) != 0
            || (QueenAttacks.GetSliderAttack(square, _occupancies[COLOR_NONE]) & _pieces[attacker, PIECE_QUEEN]) != 0
            || (KingAttacks.AttackMasks[square] & _pieces[attacker, PIECE_KING]) != 0;
    }

    public readonly IEnumerable<BinaryMove> GetMoves(PieceColor side)
    {
        int color = (int)side;

        List<BinaryMove> moves = new(capacity: 218);

        AddPawnMoves(moves, color);
        
        AddLeapingMoves(moves,
            KnightAttacks.AttackMasks, 
            new Piece(PIECE_KNIGHT, color));
        
        AddSlidingMoves(moves,
            BishopAttacks.GetSliderAttack, 
            new Piece(PIECE_BISHOP, color));
        
        AddSlidingMoves(moves,
            RookAttacks.GetSliderAttack, 
            new Piece(PIECE_ROOK, color));
        
        AddSlidingMoves(moves,
            QueenAttacks.GetSliderAttack, 
            new Piece(PIECE_QUEEN, color));
        
        AddLeapingMoves(moves,
            KingAttacks.AttackMasks, 
            new Piece(PIECE_KING, color));
        
        AddCastlingMoves(moves, color);

        ChessBoard tempBoard = new();

        int oppositeColor = COLOR_NONE + ~color;
        
        for (int i = moves.Count - 1; i >= 0; --i)
        {
            CopyTo(ref tempBoard);
            tempBoard.MakeMove(moves[i]);
            
            BitBoard king = tempBoard._pieces[color, PIECE_KING];

            if (king.TryPopFirstSquare(out SquareIndex kingSquare, out _) && tempBoard.IsSquareAttacked(kingSquare, oppositeColor))
            {
                moves.RemoveAt(i);
            }
        }

        return moves;
    }

    internal readonly void AddPawnMoves(List<BinaryMove> moves, int color)
    {
        var moveBuilder = new BinaryMoveBuilder();

        #region Setup variables depends from color.
        BitBoard pawns = _pieces[color, PIECE_PAWN];
        BitBoard empty = _occupancies[COLOR_NONE] ^ SquareMapping.ALL_SQUARES;
        BitBoard singlePush;
        BitBoard doublePush;
        ulong pawnPromotionRank;
        int rankOffset;

        if (color == COLOR_WHITE)
        {
            rankOffset = -8;
            singlePush = (pawns << 8) & empty;
            doublePush = (singlePush << 8) & empty & (SquareMapping.RANK_4);
            pawnPromotionRank = SquareMapping.RANK_8;
        }
        else
        {
            rankOffset = 8;
            singlePush = (pawns >> 8) & empty;
            doublePush = (singlePush >> 8) & empty & (SquareMapping.RANK_5);
            pawnPromotionRank = SquareMapping.RANK_1;
        }
        #endregion

        #region Loop Single Pushes
        while (singlePush.TryPopFirstSquare(out SquareIndex targetSquare, out singlePush))
        {
            SquareIndex sourceSquare = targetSquare + rankOffset;
            
            moveBuilder
                .WithSourceSquare(sourceSquare)
                .WithSourcePiece(PIECE_PAWN, color)
                .WithTargetSquare(targetSquare)
                .WithTargetPiece(Piece.Empty);

            #region Pawn Promote Moves
            if ((targetSquare.Board & pawnPromotionRank) != 0)
            {
                moveBuilder
                    .WithPromote(PromotePiece.Knight)
                    .Build(out BinaryMove promote)
                    .ResetPromote();
                moves.Add(promote);

                moveBuilder
                    .WithPromote(PromotePiece.Bishop)
                    .Build(out promote)
                    .ResetPromote();
                moves.Add(promote);

                moveBuilder
                    .WithPromote(PromotePiece.Rook)
                    .Build(out promote)
                    .ResetPromote();
                moves.Add(promote);

                moveBuilder
                    .WithPromote(PromotePiece.Queen)
                    .Build(out promote)
                    .ResetPromote();
                moves.Add(promote);
            }
            #endregion
            else
            {
                moves.Add(moveBuilder.Build());
            }

            moveBuilder.Reset();
        }
        #endregion

        #region Loop Double Pushes
        while (doublePush.TryPopFirstSquare(out SquareIndex targetSquare, out doublePush))
        {
            SquareIndex sourceSquare = targetSquare + (2 * rankOffset);

            BinaryMove move = moveBuilder
                .WithSourceSquare(sourceSquare)
                .WithSourcePiece(PIECE_PAWN, color)
                .WithTargetSquare(targetSquare)
                .WithTargetPiece(Piece.Empty)
                .WithDoublePush()
                .Build();

            moves.Add(move);
            moveBuilder.Reset();
        }
        #endregion

        #region Loop Captures
        BitBoard oppositeOccupancy = _occupancies[COLOR_NONE + ~color];

        while (pawns.TryPopFirstSquare(out SquareIndex sourceSquare, out pawns))
        {
            BitBoard mask = PawnAttacks.AttackMasks[color][sourceSquare];
            BitBoard captures = mask & oppositeOccupancy;

            while (captures.TryPopFirstSquare(out SquareIndex targetSquare, out captures))
            {
                if (TryGetPieceAt(targetSquare, out Piece piece))
                {
                    moveBuilder
                        .WithSourceSquare(sourceSquare)
                        .WithSourcePiece(PIECE_PAWN, color)
                        .WithTargetSquare(targetSquare)
                        .WithTargetPiece(in piece)
                        .WithCapture();

                    #region Pawn Promote Moves
                    if ((targetSquare.Board & pawnPromotionRank) != 0)
                    {
                        moveBuilder
                            .WithPromote(PromotePiece.Knight)
                            .Build(out var promote)
                            .ResetPromote();
                        moves.Add(promote);

                        moveBuilder
                            .WithPromote(PromotePiece.Bishop)
                            .Build(out promote)
                            .ResetPromote();
                        moves.Add(promote);

                        moveBuilder
                            .WithPromote(PromotePiece.Rook)
                            .Build(out promote)
                            .ResetPromote();
                        moves.Add(promote);

                        moveBuilder
                            .WithPromote(PromotePiece.Queen)
                            .Build(out promote)
                            .ResetPromote();
                        moves.Add(promote);
                    }
                    #endregion
                    else
                    {
                        moves.Add(moveBuilder.Build());
                    }                    

                    moveBuilder.Reset();
                }
            }
        }
        #endregion

        #region Handle Enpassant
        if (Enpassant != SquareIndex.None)
        {
            int oppositeColor = COLOR_NONE + ~color;
            BitBoard sources = PawnAttacks.AttackMasks[oppositeColor][Enpassant] & _pieces[color, PIECE_PAWN];

            while (sources.TryPopFirstSquare(out SquareIndex enpassantSrc, out sources))
            {
                BinaryMove ep = moveBuilder
                    .WithSourceSquare(enpassantSrc)
                    .WithSourcePiece(PIECE_PAWN, color)
                    .WithTargetSquare(Enpassant)
                    .WithTargetPiece(PIECE_PAWN, oppositeColor)
                    .WithEnpassant()
                    .WithCapture()
                    .Build();

                moves.Add(ep);
                moveBuilder.Reset();
            }
        }
        #endregion
    }

    internal readonly void AddLeapingMoves(
        List<BinaryMove> moves,
        ImmutableArray<ulong> attackMasks,
        in Piece piece)
    {
        if (!ValidateLeapingPiece(piece.Type))
        {
            throw new ArgumentException("Invalid leaping piece type.", nameof(piece));
        }

        var moveBuilder = new BinaryMoveBuilder();

        BitBoard pieces = _pieces[piece.Color, piece.Type];
        
        while (pieces.TryPopFirstSquare(out SquareIndex sourceSquare, out pieces))
        {
            BitBoard attackMask = ClearMaskFromOccupancies(attackMasks[sourceSquare], piece.Color);
            
            while (attackMask.TryPopFirstSquare(out SquareIndex targetSquare, out attackMask))
            {
                if (TryGetPieceAt(targetSquare, out Piece targetPiece))
                {
                    moveBuilder.WithCapture();
                }
                
                moveBuilder
                    .WithSourceSquare(sourceSquare)
                    .WithSourcePiece(in piece)
                    .WithTargetSquare(targetSquare)
                    .WithTargetPiece(in targetPiece);

                moves.Add(moveBuilder.Build());
                moveBuilder.Reset();
            }
        }
    }

    internal readonly void AddSlidingMoves(
        List<BinaryMove> moves, 
        Func<SquareIndex, ulong, BitBoard> getSliderAttackFunc,
        in Piece piece)
    {
        if (!ValidateSlidingPiece(piece.Type))
        {
            throw new ArgumentException("Invalid sliding piece type.", nameof(piece));
        }

        var moveBuilder = new BinaryMoveBuilder();
        
        BitBoard pieces = _pieces[piece.Color, piece.Type];

        while (pieces.TryPopFirstSquare(out SquareIndex sourceSquare, out pieces))
        {
            BitBoard attack = ClearMaskFromOccupancies(
                getSliderAttackFunc.Invoke(sourceSquare, _occupancies[COLOR_NONE]),
                occupanciesColor: piece.Color);

            while (attack.TryPopFirstSquare(out SquareIndex targetSquare, out attack))
            {
                if (TryGetPieceAt(targetSquare, out Piece targetPiece))
                {
                    moveBuilder.WithCapture();
                }

                moveBuilder
                    .WithSourceSquare(sourceSquare)
                    .WithSourcePiece(in piece)
                    .WithTargetSquare(targetSquare)
                    .WithTargetPiece(in targetPiece);

                moves.Add(moveBuilder.Build());
                moveBuilder.Reset();
            }
        }
    }

    internal readonly List<BinaryMove> AddCastlingMoves(List<BinaryMove> moves, int color)
    { 
        var moveBuilder = new BinaryMoveBuilder();

        if (color == COLOR_BLACK)
        {
            bool BK_CastlingAvailable = CastlingState.HasFlag(Castling.BK) &&
                _occupancies[COLOR_NONE].GetBitAt(EnumSquare.f8) == 0 &&
                _occupancies[COLOR_NONE].GetBitAt(EnumSquare.g8) == 0 &&
                !IsSquareAttacked(EnumSquare.e8, COLOR_WHITE) &&
                !IsSquareAttacked(EnumSquare.f8, COLOR_WHITE) &&
                !IsSquareAttacked(EnumSquare.g8, COLOR_WHITE);

            if (BK_CastlingAvailable)
            {
                moves.Add(moveBuilder.WithCastling(Castling.BK).Build());
                moveBuilder.Reset();
            }

            bool BQ_CastlingAvailable = CastlingState.HasFlag(Castling.BQ) &&
                _occupancies[COLOR_NONE].GetBitAt(EnumSquare.b8) == 0 &&
                _occupancies[COLOR_NONE].GetBitAt(EnumSquare.c8) == 0 &&
                _occupancies[COLOR_NONE].GetBitAt(EnumSquare.d8) == 0 &&
                !IsSquareAttacked(EnumSquare.e8, COLOR_WHITE) &&
                !IsSquareAttacked(EnumSquare.d8, COLOR_WHITE) &&
                !IsSquareAttacked(EnumSquare.c8, COLOR_WHITE);

            if (BQ_CastlingAvailable)
            {
                moves.Add(moveBuilder.WithCastling(Castling.BQ).Build());
                moveBuilder.Reset();
            }
        }

        else
        {
            bool WK_CastlingAvailable = CastlingState.HasFlag(Castling.WK) &&
                _occupancies[COLOR_NONE].GetBitAt(EnumSquare.f1) == 0 &&
                _occupancies[COLOR_NONE].GetBitAt(EnumSquare.g1) == 0 &&
                !IsSquareAttacked(EnumSquare.e1, COLOR_BLACK) &&
                !IsSquareAttacked(EnumSquare.f1, COLOR_BLACK) &&
                !IsSquareAttacked(EnumSquare.g1, COLOR_BLACK);

            if (WK_CastlingAvailable)
            {
                moves.Add(moveBuilder.WithCastling(Castling.WK).Build());
                moveBuilder.Reset();
            }

            bool WQ_CastlingAvailable = CastlingState.HasFlag(Castling.WQ) &&
                _occupancies[COLOR_NONE].GetBitAt(EnumSquare.b1) == 0 &&
                _occupancies[COLOR_NONE].GetBitAt(EnumSquare.c1) == 0 &&
                _occupancies[COLOR_NONE].GetBitAt(EnumSquare.d1) == 0 &&
                !IsSquareAttacked(EnumSquare.e1, COLOR_BLACK) &&
                !IsSquareAttacked(EnumSquare.d1, COLOR_BLACK) &&
                !IsSquareAttacked(EnumSquare.c1, COLOR_BLACK);

            if (WQ_CastlingAvailable)
            {
                moves.Add(moveBuilder.WithCastling(Castling.WQ).Build());
                moveBuilder.Reset();
            }
        }

        return moves;
    }
    
    private readonly BitBoard ClearMaskFromOccupancies(BitBoard mask, int occupanciesColor)
    {
        return mask & ~_occupancies[occupanciesColor];
    }
    #endregion

    #region Apply Moves
    private const EnumSquare WHITE_KING = EnumSquare.e1;
    private const EnumSquare BLACK_KING = EnumSquare.e8;

    private const EnumSquare WK_ROOK = EnumSquare.h1;
    private const EnumSquare WQ_ROOK = EnumSquare.a1;
    private const EnumSquare BK_ROOK = EnumSquare.h8;
    private const EnumSquare BQ_ROOK = EnumSquare.a8;

    public bool TryMakeLegalMove(BinaryMove move)
    {
        IEnumerable<BinaryMove> moves = GetMoves(CurrentColor);
        
        if (moves.Any(x => x.EncodedValue == move.EncodedValue))
        {
            MakeMove(move);
            CurrentColor = CurrentColor.Opposite();
            return true;
        }

        return false;
    }

    public void MakeMove(BinaryMove move)
    {
        if (move.Castling != Castling.None)
        {
            MakeCastling(move.Castling);
        }
        
        else if (move.IsDoublePush)
        {
            Enpassant = SquareIndex.None;
            MakeDoublePush(move);
            return;
        }

        #region Make Enpassant
        else if (move.IsEnpassant)
        {
            MakeEnpassant(move);
        }
        #endregion

        #region Make Normal Move
        else
        {
            MakeNormalMove(move);
            BreakCastlingIfPossible(move);
        }
        #endregion

        Enpassant = SquareIndex.None;
    }

    internal void MakeCastling(Castling castling)
    {
        switch (castling)
        {
            case Castling.WK:
                MovePiece(WHITE_KING, EnumSquare.g1, new Piece(PIECE_KING, COLOR_WHITE));
                MovePiece(WK_ROOK, EnumSquare.f1, new Piece(PIECE_ROOK, COLOR_WHITE));
                CastlingState ^= CastlingState & (Castling.WK | Castling.WQ);
                break;
            case Castling.WQ: 
                MovePiece(WHITE_KING, EnumSquare.c1, new Piece(PIECE_KING, COLOR_WHITE));
                MovePiece(WQ_ROOK, EnumSquare.d1, new Piece(PIECE_ROOK, COLOR_WHITE));
                CastlingState ^= CastlingState & (Castling.WK | Castling.WQ);
                break;
            case Castling.BK:
                MovePiece(BLACK_KING, EnumSquare.g8, new Piece(PIECE_KING, COLOR_BLACK));
                MovePiece(BK_ROOK, EnumSquare.f8, new Piece(PIECE_ROOK, COLOR_BLACK));
                CastlingState ^= CastlingState & (Castling.BK | Castling.BQ);
                break;
            case Castling.BQ:
                MovePiece(BLACK_KING, EnumSquare.c8, new Piece(PIECE_KING, COLOR_BLACK));
                MovePiece(BQ_ROOK, EnumSquare.d8, new Piece(PIECE_ROOK, COLOR_BLACK));
                CastlingState ^= CastlingState & (Castling.BK | Castling.BQ);
                break;
            default: break;
        }
    }

    internal void MakeDoublePush(BinaryMove move)
    {
        SquareIndex targetSquare = move.TargetSquare;

        MovePiece(move.SourceSquare, targetSquare, move.SourcePiece);

        #region Set Enpassant
        int oppositeColor = COLOR_NONE + ~move.SourcePieceColor;
        var offset = oppositeColor == COLOR_WHITE ? 8 : -8;
        SquareIndex left = targetSquare + 1;
        SquareIndex right = targetSquare - 1;

        if (left.Rank == targetSquare.Rank && TryGetPieceAt(left, out Piece pieceNearby))
        {
            if (pieceNearby.Type == PIECE_PAWN && pieceNearby.Color == oppositeColor)
            {
                Enpassant = targetSquare + offset;
            }
        }

        if (right.Rank == targetSquare.Rank && TryGetPieceAt(right, out pieceNearby))
        {
            if (pieceNearby.Type == PIECE_PAWN && pieceNearby.Color == oppositeColor)
            {
                Enpassant = targetSquare + offset;
            }
        }
        #endregion
    }

    internal readonly void MakeEnpassant(BinaryMove move)
    {
        SquareIndex sourceSquare = move.SourceSquare;
        SquareIndex targetSquare = move.TargetSquare;
        
        MovePiece(sourceSquare, targetSquare, move.SourcePiece);

        SquareIndex ep = move.TargetPiece.Color == COLOR_BLACK 
            ? Enpassant - 8 
            : Enpassant + 8;

        RemovePieceAt(ep, move.TargetPiece);
    }

    internal void MakeNormalMove(BinaryMove move)
    {
        if (move.IsCapture || (move.TargetPiece.Type != PIECE_NONE))
        {
            RemovePieceAt(move.TargetSquare, move.TargetPiece);
        }

        SquareIndex sourceSquare = move.SourceSquare;
        SquareIndex targetSquare = move.TargetSquare;
        Piece sourcePiece = move.SourcePiece;

        MovePiece(sourceSquare, targetSquare, in sourcePiece);

        if (move.Promote != PromotePiece.None)
        {
            Piece promotedPiece = new(
                type: (int)move.Promote,
                color: sourcePiece.Color);

            RemovePieceAt(targetSquare, in sourcePiece);
            SetPieceAt(targetSquare, in promotedPiece);
        }
    }

    internal void BreakCastlingIfPossible(BinaryMove move)
    {
        SquareIndex sourceSquare = move.SourceSquare;
        SquareIndex targetSquare = move.TargetSquare;

        Piece sourcePiece = move.SourcePiece;
        Piece targetPiece = move.TargetPiece;

        Castling castlingToBreak = Castling.None;

        #region KING was moved.
        if (sourcePiece.Type == PIECE_KING)
        {
            if (sourcePiece.Color == COLOR_WHITE)
            {
                castlingToBreak |= Castling.WK;
                castlingToBreak |= Castling.WQ;
            }
            else
            {
                castlingToBreak |= Castling.BK;
                castlingToBreak |= Castling.BQ;
            }
        }
        #endregion

        #region ROOK was moved from init position.
        else if (sourcePiece.Type == PIECE_ROOK)
        {
            if (sourcePiece.Color == COLOR_WHITE)
            {
                if (sourceSquare == WK_ROOK) 
                    castlingToBreak |= Castling.WK;
                else if (sourceSquare == WQ_ROOK) 
                    castlingToBreak |= Castling.WQ;
            }
            else
            {
                
                if (sourceSquare == BK_ROOK) castlingToBreak |= Castling.BK;
                else if (sourceSquare == BQ_ROOK) castlingToBreak |= Castling.BQ;
            }
        }
        #endregion

        #region ROOK was captured.
        // Additional check in case when rook wasnt moved but was captured at init position.
        else if (targetPiece.Type == PIECE_ROOK)
        {
            if (targetPiece.Color == COLOR_WHITE)
            {
                if (targetSquare == WK_ROOK) 
                    castlingToBreak |= Castling.WK;
                else if (targetSquare == WQ_ROOK) 
                    castlingToBreak |= Castling.WQ;
            }
            else
            {
                if (targetSquare == BK_ROOK) 
                    castlingToBreak |= Castling.BK;
                else if (targetSquare == BQ_ROOK) 
                    castlingToBreak |= Castling.BQ;
            }
        }
        #endregion

        CastlingState ^= CastlingState & castlingToBreak;
    }

    #endregion

    public readonly void CopyTo(ref ChessBoard board)
    {
        Array.Copy(_pieces, board._pieces, _pieces.Length);
        Array.Copy(_occupancies, board._occupancies, _occupancies.Length);
        board.CastlingState = CastlingState;
        board.HalfMoveClock = HalfMoveClock;
        board.FullMoveNumber = FullMoveNumber;
        board.CurrentColor = CurrentColor;
        board.Enpassant = Enpassant;
    }

    private static bool ValidateSlidingPiece(int piece)
    {
        return piece == PIECE_BISHOP || piece == PIECE_ROOK || piece == PIECE_QUEEN;
    }

    private static bool ValidateLeapingPiece(int piece)
    {
        return piece == PIECE_KNIGHT || piece == PIECE_KING;
    }
}
