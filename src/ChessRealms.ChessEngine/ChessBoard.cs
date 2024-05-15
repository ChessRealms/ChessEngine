using ChessRealms.ChessEngine.Core.Attacks;
using ChessRealms.ChessEngine.Core.Builders;
using ChessRealms.ChessEngine.Core.Extensions;
using ChessRealms.ChessEngine.Core.Types;
using System.Collections.Immutable;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ChessRealms.ChessEngine;

public struct ChessBoard
{
    private readonly BitBoard[,] _pieces;    
    private readonly BitBoard[] _occupancies;

    private BitBoard _allOccupancies;

    public SquareIndex Enpassant { get; set; }

    public PieceColor CurrentColor { get; set; }

    public Castling CastlingState { get; set; }

    public int HalfMoveClock { get; set; }

    public int FullMoveNumber { get; set; }

    public ChessBoard()
    {
        _pieces = new BitBoard[2, 6];
        _occupancies = new BitBoard[2];

        Enpassant = SquareIndex.None;
        CurrentColor = PieceColor.White;
        CastlingState = 0;
    }

    public readonly bool TryGetPieceAt(SquareIndex square, out Piece piece)
    {
        if (_allOccupancies.GetBitAt(square) == 0)
        {
            piece = Piece.Empty;
            return false;
        }

        BitBoard whiteOcc = _occupancies[PieceColor.White.ToIndex()];

        int colorIndex = (whiteOcc.GetBitAt(square) != 0 
            ? PieceColor.White 
            : PieceColor.Black).ToIndex();
        
        int pieceIndex = -1;

        for (int i = 0; i < 6; ++i)
        {
            if (_pieces[colorIndex, i].GetBitAt(square) != 0)
            {
                pieceIndex = i;
                break;
            }
        }

        if (pieceIndex == -1)
        {
            piece = Piece.Empty;
            return false;
        }

        piece = new Piece(pieceIndex.ToPiece(), colorIndex.ToColor());
        return true;
    }

    public void SetPieceAt(SquareIndex square, in Piece piece)
    {
        int colorIndex = piece.Color.ToIndex();
        int pieceIndex = piece.Type.ToIndex();
        
        _pieces[colorIndex, pieceIndex].SetBitAt(square);
        _occupancies[colorIndex].SetBitAt(square);
        _allOccupancies.SetBitAt(square);
    }

    public void SetPieceAt(SquareIndex square, PieceColor color, PieceType piece)
    {
        int colorIndex = color.ToIndex();
        int pieceIndex = piece.ToIndex();
        
        _pieces[colorIndex, pieceIndex].SetBitAt(square);
        _occupancies[colorIndex].SetBitAt(square);
        _allOccupancies.SetBitAt(square);
    }

    public void RemovePieceAt(SquareIndex square, in Piece piece)
    {
        int colorIndex = piece.Color.ToIndex();
        int pieceIndex = piece.Type.ToIndex();

        _pieces[colorIndex, pieceIndex].PopBitAt(square);
        _occupancies[colorIndex].PopBitAt(square);
        _allOccupancies.PopBitAt(square);
    }

    public void MovePiece(SquareIndex source, SquareIndex target, in Piece piece)
    {
        RemovePieceAt(source, in piece);
        SetPieceAt(target, in piece);
    }

    #region Get Moves
    /// <summary>
    /// Determine if square is attacked by specified side.
    /// </summary>
    /// <param name="square"> Square to check. </param>
    /// <param name="attackerColor"> Attacker color. </param>
    /// <returns> <see langword="true"/> if attacked, otherwise <see langword="false"/>. </returns>
    public readonly bool IsSquareAttacked(SquareIndex square, PieceColor attackerColor)
    {
        int attacker = attackerColor.ToIndex();

        return (PawnAttacks.AttackMasks[attackerColor.Opposite()][square] & _pieces[attacker, PieceType.Pawn.ToIndex()]) != 0
            || (KnightAttacks.AttackMasks[square] & _pieces[attacker, PieceType.Knight.ToIndex()]) != 0
            || (BishopAttacks.GetSliderAttack(square, _allOccupancies) & _pieces[attacker, PieceType.Bishop.ToIndex()]) != 0
            || (RookAttacks.GetSliderAttack(square, _allOccupancies) & _pieces[attacker, PieceType.Rook.ToIndex()]) != 0
            || (QueenAttacks.GetSliderAttack(square, _allOccupancies) & _pieces[attacker, PieceType.Queen.ToIndex()]) != 0
            || (KingAttacks.AttackMasks[square] & _pieces[attacker, PieceType.King.ToIndex()]) != 0;
    }

    public readonly IEnumerable<BinaryMove> GetMoves(PieceColor side)
    {
        List<BinaryMove> pawnMoves = GetPawnMoves(side);
        List<BinaryMove> knightMoves = GetLeapingMoves(KnightAttacks.AttackMasks, new Piece(PieceType.Knight, side));
        List<BinaryMove> bishopMoves = GetSlidingMoves(BishopAttacks.GetSliderAttack, new Piece(PieceType.Bishop, side));
        List<BinaryMove> rookMoves = GetSlidingMoves(RookAttacks.GetSliderAttack, new Piece(PieceType.Rook, side));
        List<BinaryMove> queenMoves = GetSlidingMoves(QueenAttacks.GetSliderAttack, new Piece(PieceType.Queen, side));
        List<BinaryMove> kingMoves = GetLeapingMoves(KingAttacks.AttackMasks, new Piece(PieceType.King, side));
        List<BinaryMove> castlingMoves = GetCastlingMoves(side);

        // 'castlingMoves' already look for 'is king checked' so don't include this.
        List<BinaryMove>[] moveBatches = [pawnMoves, knightMoves, bishopMoves, rookMoves, queenMoves, kingMoves];
        
        ChessBoard tempBoard = new();

        PieceColor oppositeColor = side.Opposite();
        
        int sideIndex = side.ToIndex();
        int kingBoardIndex = PieceType.King.ToIndex();

        foreach (var batch in moveBatches)
        {
            for (int i = batch.Count - 1; i >= 0; --i)
            {
                CopyTo(ref tempBoard);
                tempBoard.MakeMove(batch[i]);
                BitBoard king = tempBoard._pieces[sideIndex, kingBoardIndex];
            
                if (king.TryPopFirstSquare(out SquareIndex kingSquare) && 
                    tempBoard.IsSquareAttacked(kingSquare, oppositeColor))
                {
                    batch.RemoveAt(i);
                }
            }
        }

        var moves = pawnMoves
            .Concat(knightMoves)
            .Concat(bishopMoves)
            .Concat(rookMoves)
            .Concat(queenMoves)
            .Concat(kingMoves)
            .Concat(castlingMoves);

        return moves;
    }

    internal readonly List<BinaryMove> GetPawnMoves(PieceColor color)
    {
        var moves = new List<BinaryMove>();
        var moveBuilder = new BinaryMoveBuilder();

        BitBoard pawns = _pieces[color.ToIndex(), PieceType.Pawn.ToIndex()];
        BitBoard empty = _allOccupancies ^ LerfConstants.ALL_SQUARES;
        BitBoard singlePush;
        BitBoard doublePush;
        ulong pawnPromotionRank;
        int rankOffset;

        if (color == PieceColor.White)
        {
            rankOffset = -1;
            singlePush = (pawns << 8) & empty;
            doublePush = (singlePush << 8) & empty & LerfConstants.RANK_4;
            pawnPromotionRank = LerfConstants.RANK_8;
        }
        else
        {
            rankOffset = 1;
            singlePush = (pawns >> 8) & empty;
            doublePush = (singlePush >> 8) & empty & LerfConstants.RANK_5;
            pawnPromotionRank = LerfConstants.RANK_1;
        }

        while (singlePush.TryPopFirstSquare(out SquareIndex targetSquare))
        {
            SquareIndex sourceSquare = SquareIndex.FromFileRank(
                targetSquare.File,
                targetSquare.Rank + (1 * rankOffset));

            moveBuilder
                .WithSourceSquare(sourceSquare)
                .WithSourcePiece(PieceType.Pawn, color)
                .WithTargetSquare(targetSquare);

            #region Pawn Promote Moves
            if ((targetSquare.Board & pawnPromotionRank) != 0)
            {
                int oldCount = moves.Count;
                CollectionsMarshal.SetCount(moves, oldCount + 4);
                Span<BinaryMove> movesSpan = CollectionsMarshal.AsSpan(moves);

                moveBuilder
                    .WithPromote(PromotePiece.Knight)
                    .Build(out movesSpan[oldCount])
                    .ResetPromote();

                moveBuilder
                    .WithPromote(PromotePiece.Bishop)
                    .Build(out movesSpan[oldCount + 1])
                    .ResetPromote();

                moveBuilder
                    .WithPromote(PromotePiece.Rook)
                    .Build(out movesSpan[oldCount + 2])
                    .ResetPromote();

                moveBuilder
                    .WithPromote(PromotePiece.Queen)
                    .Build(out movesSpan[oldCount + 3])
                    .ResetPromote();
            }
            #endregion
            else
            {
                moves.Add(moveBuilder.Build());
            }

            moveBuilder.Reset();
        }

        while (doublePush.TryPopFirstSquare(out SquareIndex targetSquare))
        {
            SquareIndex sourceSquare = SquareIndex.FromFileRank(
                targetSquare.File,
                targetSquare.Rank + (2 * rankOffset));

            BinaryMove move = moveBuilder
                .WithSourceSquare(sourceSquare)
                .WithSourcePiece(PieceType.Pawn, color)
                .WithTargetSquare(targetSquare)
                .WithDoublePush()
                .Build();

            moves.Add(move);
            moveBuilder.Reset();
        }

        BitBoard oppositeOccupancy = _occupancies[color.Opposite().ToIndex()];

        while (pawns.TryPopFirstSquare(out SquareIndex sourceSquare))
        {
            BitBoard mask = PawnAttacks.AttackMasks[color][sourceSquare];
            BitBoard captures = mask & oppositeOccupancy;

            while (captures.TryPopFirstSquare(out SquareIndex targetSquare))
            {
                if (TryGetPieceAt(targetSquare, out Piece piece))
                {
                    moveBuilder
                        .WithSourceSquare(sourceSquare)
                        .WithSourcePiece(PieceType.Pawn, color)
                        .WithTargetSquare(targetSquare)
                        .WithTargetPiece(in piece)
                        .WithCapture();

                    #region Pawn Promote Moves
                    if ((targetSquare.Board & pawnPromotionRank) != 0)
                    {
                        int oldCount = moves.Count;
                        CollectionsMarshal.SetCount(moves, oldCount + 4);
                        Span<BinaryMove> movesSpan = CollectionsMarshal.AsSpan(moves);

                        moveBuilder
                            .WithPromote(PromotePiece.Knight)
                            .Build(out movesSpan[oldCount])
                            .ResetPromote();

                        moveBuilder
                            .WithPromote(PromotePiece.Bishop)
                            .Build(out movesSpan[oldCount + 1])
                            .ResetPromote();

                        moveBuilder
                            .WithPromote(PromotePiece.Rook)
                            .Build(out movesSpan[oldCount + 2])
                            .ResetPromote();

                        moveBuilder
                            .WithPromote(PromotePiece.Queen)
                            .Build(out movesSpan[oldCount + 3])
                            .ResetPromote();
                    }
                    #endregion
                    else
                    {
                        moves.Add(moveBuilder.Build());
                        moveBuilder.Reset();
                    }                    
                }
            }
        }

        if (Enpassant != SquareIndex.None)
        {
            SquareIndex left = Enpassant - 1;
            SquareIndex right = Enpassant + 1;
            
            if (TryGetPieceAt(left, out Piece leftPiece) && left.Rank == Enpassant.Rank)
            {
                if (leftPiece.Type == PieceType.Pawn && leftPiece.Color == color)
                {
                    BinaryMove enpassant = moveBuilder
                        .WithSourceSquare(left)
                        .WithSourcePiece(in leftPiece)
                        // target square is upper/below (depends from rankOffset)
                        .WithTargetSquare(Enpassant + (8 * rankOffset))
                        .WithTargetPiece(new Piece(PieceType.Pawn, color.Opposite()))
                        .WithCapture()
                        .WithEnpassant()
                        .Build();
                        
                    moves.Add(enpassant);
                    moveBuilder.Reset();
                }
            }
            
            if (TryGetPieceAt(right, out Piece rightPiece) && right.Rank == Enpassant.Rank)
            {
                if (rightPiece.Type == PieceType.Pawn && rightPiece.Color == color)
                {
                    BinaryMove enpassant = moveBuilder
                        .WithSourceSquare(right)
                        .WithSourcePiece(in rightPiece)
                        .WithTargetSquare(Enpassant)
                        .WithTargetPiece(PieceType.Pawn, color.Opposite())
                        .WithCapture()
                        .WithEnpassant()
                        .Build();
                        
                    moves.Add(enpassant);
                    moveBuilder.Reset();
                }
            }
        }

        return moves;
    }

    /// <summary>
    /// Get leaping moves by specified piece and attack masks.
    /// Allowed piece types are <see cref="PieceType.Knight"/> and <see cref="PieceType.King"/>.
    /// </summary>
    /// <param name="attackMasks"> Immutable array with pre-allocated leaping attack masks. </param>
    /// <param name="piece"> Piece to generate moves. </param>
    /// <returns> Generated moves. </returns>
    /// <exception cref="ArgumentException"></exception>
    internal readonly List<BinaryMove> GetLeapingMoves(ImmutableArray<ulong> attackMasks, in Piece piece)
    {
        if (!ValidateLeapingPiece(piece.Type))
        {
            throw new ArgumentException("Invalid leaping piece type.", nameof(piece));
        }

        var moves = new List<BinaryMove>();
        var moveBuilder = new BinaryMoveBuilder();

        BitBoard pieces = _pieces[piece.Color.ToIndex(), piece.Type.ToIndex()];
        
        while (pieces.TryPopFirstSquare(out SquareIndex sourceSquare))
        {
            BitBoard attackMask = ClearMaskFromOccupancies(attackMasks[sourceSquare], piece.Color);
            
            while (attackMask.TryPopFirstSquare(out SquareIndex targetSquare))
            {
                moveBuilder
                    .WithSourceSquare(sourceSquare)
                    .WithSourcePiece(in piece)
                    .WithTargetSquare(targetSquare);

                if (TryGetPieceAt(targetSquare, out Piece targetPiece))
                {
                    moveBuilder.WithCapture().WithTargetPiece(in targetPiece);
                }

                moves.Add(moveBuilder.Build());
                moveBuilder.Reset();
            }
        }

        return moves;
    }

    /// <summary>
    /// Get sliding moves by specified piece and related sliderAttackFunc.
    /// Allowed piece types are <see cref="PieceType.Bishop"/>, <see cref="PieceType.Rook"/> and <see cref="PieceType.Queen"/>.
    /// </summary>
    /// <param name="getSliderAttackFunc"> Slider attack method that will be used to get attack/move masks. </param>
    /// <param name="piece"> Piece to generate movess. </param>
    /// <returns> Generated moves. </returns>
    /// <exception cref="ArgumentException"></exception>
    internal readonly List<BinaryMove> GetSlidingMoves(
        Func<SquareIndex, ulong, BitBoard> getSliderAttackFunc,
        in Piece piece)
    {
        if (!ValidateSlidingPiece(piece.Type))
        {
            throw new ArgumentException("Invalid sliding piece type.", nameof(piece));
        }

        var moves = new List<BinaryMove>();
        var moveBuilder = new BinaryMoveBuilder();
        
        BitBoard pieces = _pieces[piece.Color.ToIndex(), piece.Type.ToIndex()];
        BitBoard oppositeOccupancies = _occupancies[piece.Color.Opposite().ToIndex()];

        while (pieces.TryPopFirstSquare(out SquareIndex sourceSquare))
        {
            BitBoard attack = ClearMaskFromOccupancies(
                getSliderAttackFunc.Invoke(sourceSquare, _allOccupancies),
                occupanciesColor: piece.Color);

            while (attack.TryPopFirstSquare(out SquareIndex targetSquare))
            {
                BitBoard toBitboard = targetSquare.Board;
            
                if ((toBitboard & oppositeOccupancies) != 0 && TryGetPieceAt(targetSquare, out Piece targetPiece))
                {
                    moveBuilder.WithCapture().WithTargetPiece(in targetPiece);
                }

                moveBuilder
                    .WithSourceSquare(sourceSquare)
                    .WithSourcePiece(in piece)
                    .WithTargetSquare(targetSquare);

                moves.Add(moveBuilder.Build());
                moveBuilder.Reset();
            }
        }

        return moves;
    }

    internal readonly List<BinaryMove> GetCastlingMoves(PieceColor color)
    {
        var moves = new List<BinaryMove>(capacity: 2);
        var moveBuilder = new BinaryMoveBuilder();

        if (color == PieceColor.Black)
        {
            bool BK_CastlingAvailable = CastlingState.HasFlag(Castling.BK) &&
                _allOccupancies.GetBitAt(EnumSquare.f8) == 0 &&
                _allOccupancies.GetBitAt(EnumSquare.g8) == 0 &&
                !IsSquareAttacked(EnumSquare.e8, PieceColor.White) &&
                !IsSquareAttacked(EnumSquare.g8, PieceColor.White);

            if (BK_CastlingAvailable)
            {
                moves.Add(moveBuilder.WithCastling(Castling.BK).Build());
                moveBuilder.Reset();
            }

            bool BQ_CastlingAvailable = CastlingState.HasFlag(Castling.BQ) &&
                _allOccupancies.GetBitAt(EnumSquare.b8) == 0 &&
                _allOccupancies.GetBitAt(EnumSquare.c8) == 0 &&
                _allOccupancies.GetBitAt(EnumSquare.d8) == 0 &&
                !IsSquareAttacked(EnumSquare.e8, PieceColor.White) &&
                !IsSquareAttacked(EnumSquare.c8, PieceColor.White);

            if (BQ_CastlingAvailable)
            {
                moves.Add(moveBuilder.WithCastling(Castling.BQ).Build());
                moveBuilder.Reset();
            }
        }

        else
        {
            bool WK_CastlingAvailable = CastlingState.HasFlag(Castling.WK) &&
                _allOccupancies.GetBitAt(EnumSquare.f1) == 0 &&
                _allOccupancies.GetBitAt(EnumSquare.g1) == 0 &&
                !IsSquareAttacked(EnumSquare.e1, PieceColor.Black) &&
                !IsSquareAttacked(EnumSquare.g1, PieceColor.Black);

            if (WK_CastlingAvailable)
            {
                moves.Add(moveBuilder.WithCastling(Castling.WK).Build());
                moveBuilder.Reset();
            }

            bool WQ_CastlingAvailable = CastlingState.HasFlag(Castling.WQ) &&
                _allOccupancies.GetBitAt(EnumSquare.b1) == 0 &&
                _allOccupancies.GetBitAt(EnumSquare.c1) == 0 &&
                _allOccupancies.GetBitAt(EnumSquare.d1) == 0 &&
                !IsSquareAttacked(EnumSquare.e1, PieceColor.Black) &&
                !IsSquareAttacked(EnumSquare.c1, PieceColor.Black);

            if (WQ_CastlingAvailable)
            {
                moves.Add(moveBuilder.WithCastling(Castling.WQ).Build());
                moveBuilder.Reset();
            }
        }

        return moves;
    }
    
    private readonly BitBoard ClearMaskFromOccupancies(BitBoard mask, PieceColor occupanciesColor)
    {
        return mask ^ (0UL ^ (mask & _occupancies[occupanciesColor.ToIndex()]));
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
        }
        #endregion

        BreakCastlingIfPossible(move);
        Enpassant = SquareIndex.None;
    }

    internal void MakeCastling(Castling castling)
    {
        switch (castling)
        {
            case Castling.WK:
                MovePiece(WHITE_KING, EnumSquare.g1, new Piece(PieceType.King, PieceColor.White));
                MovePiece(WK_ROOK, EnumSquare.f1, new Piece(PieceType.Rook, PieceColor.White));
                CastlingState ^= CastlingState & (Castling.WK | Castling.WQ);
                break;
            case Castling.WQ: 
                MovePiece(WHITE_KING, EnumSquare.c1, new Piece(PieceType.King, PieceColor.White));
                MovePiece(WQ_ROOK, EnumSquare.d1, new Piece(PieceType.Rook, PieceColor.White));
                CastlingState ^= CastlingState & (Castling.WK | Castling.WQ);
                break;
            case Castling.BK:
                MovePiece(BLACK_KING, EnumSquare.g8, new Piece(PieceType.King, PieceColor.Black));
                MovePiece(BK_ROOK, EnumSquare.f8, new Piece(PieceType.Rook, PieceColor.Black));
                CastlingState ^= CastlingState & (Castling.BK | Castling.BQ);
                break;
            case Castling.BQ:
                MovePiece(BLACK_KING, EnumSquare.c8, new Piece(PieceType.King, PieceColor.Black));
                MovePiece(BQ_ROOK, EnumSquare.d8, new Piece(PieceType.Rook, PieceColor.Black));
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
        PieceColor oppositeColor = move.SourcePieceColor.Opposite();
        SquareIndex left = targetSquare - 1;
        SquareIndex right = targetSquare + 1;

        if (left.Rank == targetSquare.Rank && TryGetPieceAt(left, out Piece pieceNearby))
        {
            if (pieceNearby.Type == PieceType.Pawn && pieceNearby.Color == oppositeColor)
            {
                Enpassant = targetSquare;
            }
        }

        if (right.Rank == targetSquare.Rank && TryGetPieceAt(right, out pieceNearby))
        {
            if (pieceNearby.Type == PieceType.Pawn && pieceNearby.Color == oppositeColor)
            {
                Enpassant = targetSquare;
            }
        }
        #endregion
    }

    internal void MakeEnpassant(BinaryMove move)
    {
        SquareIndex sourceSquare = move.SourceSquare;
        SquareIndex targetSquare = move.TargetSquare;
        MovePiece(sourceSquare, targetSquare, move.SourcePiece);
        RemovePieceAt(Enpassant, move.TargetPiece);
    }

    internal void MakeNormalMove(BinaryMove move)
    {
        if (move.IsCapture)
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
                type: move.Promote.ToPieceType(),
                color: sourcePiece.Color);

            RemovePieceAt(sourceSquare, in sourcePiece);
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
        if (sourcePiece.Type == PieceType.King)
        {
            if (sourcePiece.Color == PieceColor.White)
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
        else if (sourcePiece.Type == PieceType.Rook)
        {
            if (sourcePiece.Color == PieceColor.White)
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
        else if (targetPiece.Type == PieceType.Rook)
        {
            if (targetPiece.Color == PieceColor.White)
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
        board._allOccupancies = _allOccupancies;
        board.CastlingState = CastlingState;
        board.HalfMoveClock = HalfMoveClock;
        board.FullMoveNumber = FullMoveNumber;
        board.CurrentColor = CurrentColor;
        board.Enpassant = Enpassant;
    }

    private static bool ValidateSlidingPiece(PieceType pieceType)
    {
        return pieceType == PieceType.Bishop || pieceType == PieceType.Rook || pieceType == PieceType.Queen;
    }

    private static bool ValidateLeapingPiece(PieceType pieceType)
    {
        return pieceType == PieceType.Knight || pieceType == PieceType.King;
    }
}
