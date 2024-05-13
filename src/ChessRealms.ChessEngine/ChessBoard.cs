using ChessRealms.ChessEngine.Core.Attacks;
using ChessRealms.ChessEngine.Core.Builders;
using ChessRealms.ChessEngine.Core.Extensions;
using ChessRealms.ChessEngine.Core.Types;
using System.Numerics;

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

    public void SetPieceAt(SquareIndex square, Piece piece)
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

    internal readonly List<BinaryMove> GetMoves(PieceColor side)
    {
        List<BinaryMove> pawnMoves = GetPawnMoves(side);
        return pawnMoves;
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

        while (singlePush != 0)
        {
            SquareIndex targetSquare = singlePush.TrailingZeroCount();
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
            singlePush.PopBitAt(targetSquare);
        }

        while (doublePush != 0)
        {
            SquareIndex targetSquare = doublePush.TrailingZeroCount();
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
            doublePush.PopBitAt(targetSquare);
        }

        while (pawns != 0)
        {
            SquareIndex sourceSquare = pawns.TrailingZeroCount();

            BitBoard mask = PawnAttacks.AttackMasks[color][sourceSquare];
            BitBoard captures = mask & _occupancies[color.Opposite().ToIndex()];

            while (captures != 0)
            {
                SquareIndex targetSquare = captures.TrailingZeroCount();

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

                captures.PopBitAt(targetSquare);
            }

            pawns.PopBitAt(sourceSquare);
        }

        return moves;
    }

    /// <summary>
    /// Get potential bishop moves.
    /// </summary>
    /// <param name="square"> Square from where bishop going move. </param>
    /// <param name="pieceColor"> Color of piece to move. </param>
    /// <returns> Array with moves. </returns>
    internal readonly BinaryMove[] GetBishopMoves(SquareIndex square, PieceColor pieceColor)
    {
        BitBoard attack = BishopAttacks.GetSliderAttack(square, _allOccupancies);

        // Remove our pieces from attacks.
        attack ^= 0UL ^ (attack & _occupancies[pieceColor.ToIndex()]);
        var moves = new BinaryMove[BitOperations.PopCount(attack)];
        var moveBuilder = new BinaryMoveBuilder();

        int opposite = pieceColor.Opposite().ToIndex();
        int moveIndex = 0;

        while (attack != 0)
        {
            SquareIndex target = attack.TrailingZeroCount();
            BitBoard toBitboard = target.Board;
            
            if ((toBitboard & _occupancies[opposite]) != 0 && TryGetPieceAt(target, out Piece targetPiece))
            {
                moveBuilder.WithCapture().WithTargetPiece(in targetPiece);
            }

            moveBuilder
                .WithSourceSquare(square)
                .WithSourcePiece(PieceType.Bishop, pieceColor)
                .WithTargetSquare(target);

            moves[moveIndex] = moveBuilder.Build();

            moveBuilder.Reset();
            attack.PopBitAt(target);
            ++moveIndex;
        }

        return moves;
    }
}
