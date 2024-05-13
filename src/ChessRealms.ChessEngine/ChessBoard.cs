using ChessRealms.ChessEngine.Core.Attacks;
using ChessRealms.ChessEngine.Core.Builders;
using ChessRealms.ChessEngine.Core.Extensions;
using ChessRealms.ChessEngine.Core.Types;
using System.Collections.Immutable;

namespace ChessRealms.ChessEngine;

public struct ChessBoard
{
    private readonly BitBoard[,] _pieces;    
    private readonly BitBoard[] _occupancies;

    private BitBoard _allOccupancies;

    public SquareIndex Enpassant { get; set; }

    public PieceColor CurrentColor { get; set; }

    public Castling Castling { get; set; }

    public int HalfMoveClock { get; set; }

    public int FullMoveNumber { get; set; }

    public ChessBoard()
    {
        _pieces = new BitBoard[2, 6];
        _occupancies = new BitBoard[2];

        Enpassant = SquareIndex.None;
        CurrentColor = PieceColor.White;
        Castling = 0;
    }

    public void SetPieceAt(SquareIndex square, in Piece piece)
    {
        SetPieceAt(square, piece.Color, piece.Type);
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

    public void SetPieceAt(SquareIndex square, PieceColor color, PieceType piece)
    {
        int colorIndex = color.ToIndex();
        int pieceIndex = piece.ToIndex();
        
        _pieces[colorIndex, pieceIndex].SetBitAt(square);
        _occupancies[colorIndex].SetBitAt(square);
        _allOccupancies.SetBitAt(square);
    }

    public void RemovePieceAt(SquareIndex square)
    {
        if (!TryGetPieceAt(square, out Piece piece))
        {
            return;
        }

        int colorIndex = piece.Color.ToIndex();
        int pieceIndex = piece.Type.ToIndex();

        _pieces[colorIndex, pieceIndex].PopBitAt(square);
        _occupancies[colorIndex].PopBitAt(square);
        _allOccupancies.PopBitAt(square);
    }

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

        return pawnMoves
            .Concat(knightMoves)
            .Concat(bishopMoves)
            .Concat(rookMoves)
            .Concat(queenMoves)
            .Concat(kingMoves)
            .Concat(castlingMoves);
    }

    internal readonly List<BinaryMove> GetPawnMoves(PieceColor color)
    {
        var moves = new List<BinaryMove>();
        var moveBuilder = new BinaryMoveBuilder();

        BitBoard pawns = _pieces[color.ToIndex(), PieceType.Pawn.ToIndex()];
        BitBoard empty = _allOccupancies ^ LerfConstants.ALL_SQUARES;
        BitBoard singlePush;
        BitBoard doublePush;
        int rankOffset;

        if (color == PieceColor.White)
        {
            rankOffset = -1;
            singlePush = (pawns << 8) & empty;
            doublePush = (pawns << 16) & empty & LerfConstants.RANK_4;
        }
        else
        {
            rankOffset = 1;
            singlePush = (pawns >> 8) & empty;
            doublePush = (pawns >> 16) & empty & LerfConstants.RANK_5;
        }

        while (singlePush.TryPopFirstSquare(out SquareIndex targetSquare))
        {
            SquareIndex sourceSquare = SquareIndex.FromFileRank(
                targetSquare.File,
                targetSquare.Rank + (1 * rankOffset));

            BinaryMove move = moveBuilder
                .WithSourceSquare(sourceSquare)
                .WithSourcePiece(PieceType.Pawn, color)
                .WithTargetSquare(targetSquare)
                .Build();

            moves.Add(move);
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
                    BinaryMove move = moveBuilder
                        .WithSourceSquare(sourceSquare)
                        .WithSourcePiece(PieceType.Pawn, color)
                        .WithTargetSquare(targetSquare)
                        .WithTargetPiece(in piece)
                        .WithCapture()
                        .Build();

                    moves.Add(move);
                    moveBuilder.Reset();
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
                        .WithTargetSquare(Enpassant)
                        .WithTargetPiece(PieceType.Pawn, color.Opposite())
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
            bool BK_CastlingAvailable = Castling.HasFlag(Castling.BK) &&
                _allOccupancies.GetBitAt(EnumSquare.f8) == 0 &&
                _allOccupancies.GetBitAt(EnumSquare.g8) == 0 &&
                !IsSquareAttacked(EnumSquare.e8, PieceColor.White) &&
                !IsSquareAttacked(EnumSquare.g8, PieceColor.White);

            if (BK_CastlingAvailable)
            {
                moveBuilder
                    .WithSourceSquare(EnumSquare.e8)
                    .WithSourcePiece(PieceType.King, PieceColor.Black)
                    .WithTargetSquare(EnumSquare.g8)
                    .WithCastling(Castling.BK);

                moves.Add(moveBuilder.Build());
                moveBuilder.Reset();
            }

            bool BQ_CastlingAvailable = Castling.HasFlag(Castling.BQ) &&
                _allOccupancies.GetBitAt(EnumSquare.b8) == 0 &&
                _allOccupancies.GetBitAt(EnumSquare.c8) == 0 &&
                _allOccupancies.GetBitAt(EnumSquare.d8) == 0 &&
                !IsSquareAttacked(EnumSquare.e8, PieceColor.White) &&
                !IsSquareAttacked(EnumSquare.c8, PieceColor.White);

            if (BQ_CastlingAvailable)
            {
                moveBuilder
                    .WithSourceSquare(EnumSquare.e8)
                    .WithSourcePiece(PieceType.King, PieceColor.Black)
                    .WithTargetSquare(EnumSquare.c8)
                    .WithCastling(Castling.BQ);

                moves.Add(moveBuilder.Build());
                moveBuilder.Reset();
            }
        }

        else
        {
            bool WK_CastlingAvailable = Castling.HasFlag(Castling.WK) &&
                _allOccupancies.GetBitAt(EnumSquare.f1) == 0 &&
                _allOccupancies.GetBitAt(EnumSquare.g1) == 0 &&
                !IsSquareAttacked(EnumSquare.e1, PieceColor.Black) &&
                !IsSquareAttacked(EnumSquare.g1, PieceColor.Black);

            if (WK_CastlingAvailable)
            {
                moveBuilder
                    .WithSourceSquare(EnumSquare.e1)
                    .WithSourcePiece(PieceType.King, PieceColor.White)
                    .WithTargetSquare(EnumSquare.g1)
                    .WithCastling(Castling.WK);

                moves.Add(moveBuilder.Build());
                moveBuilder.Reset();
            }

            bool WQ_CastlingAvailable = Castling.HasFlag(Castling.WQ) &&
                _allOccupancies.GetBitAt(EnumSquare.b1) == 0 &&
                _allOccupancies.GetBitAt(EnumSquare.c1) == 0 &&
                _allOccupancies.GetBitAt(EnumSquare.d1) == 0 &&
                !IsSquareAttacked(EnumSquare.e1, PieceColor.Black) &&
                !IsSquareAttacked(EnumSquare.c1, PieceColor.Black);

            if (WQ_CastlingAvailable)
            {
                moveBuilder
                    .WithSourceSquare(EnumSquare.e1)
                    .WithSourcePiece(PieceType.King, PieceColor.White)
                    .WithTargetSquare(EnumSquare.c1)
                    .WithCastling(Castling.WQ);

                moves.Add(moveBuilder.Build());
                moveBuilder.Reset();
            }
        }

        return moves;
    }

    private readonly BitBoard ClearMaskFromOccupancies(BitBoard mask, PieceColor occupanciesColor)
    {
        return mask ^ (0UL ^ (mask & _occupancies[occupanciesColor.ToIndex()]));
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
