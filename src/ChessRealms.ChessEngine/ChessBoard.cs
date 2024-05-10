using ChessRealms.ChessEngine.Core.Attacks;
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

    public readonly Piece? GetPieceAt(SquareIndex square)
    {
        if (_allOccupancies.GetBitAt(square) == 0)
        {
            return null;
        }

        BitBoard whiteOcc = _occupancies[(int)PieceColor.White];

        int colorIndex = (whiteOcc.GetBitAt(square) != 0 
            ? PieceColor.White 
            : PieceColor.Black).ToIndex();
        
        int pieceIndex = -1;

        for (int piece = 0; piece < 6; ++piece)
        {
            if (_pieces[colorIndex, piece].GetBitAt(square) != 0)
            {
                pieceIndex = piece;
                break;
            }
        }

        if (pieceIndex == -1)
        {
            return null;
        }

        return new Piece
        {
            Color = colorIndex.ToColor(),
            Type = pieceIndex.ToPiece()
        };
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
        Piece? piece = GetPieceAt(square);

        if (piece == null)
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

    /// <summary>
    /// Get potential bishop moves.
    /// </summary>
    /// <param name="square"> Square from where bishop going move. </param>
    /// <param name="pieceColor"> Color of piece to move. </param>
    /// <returns> Array with moves. </returns>
    public readonly SquareIndex[] GetBishopMoves(SquareIndex square, PieceColor pieceColor)
    {
        BitBoard attack = BishopAttacks.GetSliderAttack(square, _allOccupancies);

        // Remove our pieces from attacks.
        attack ^= 0UL ^ (attack & _occupancies[pieceColor.ToIndex()]);
        // Finally we could set exact size for output array.
        // 💪💪💪 Every bit and memory allocation is matter when this is Chess Engine!!! 💪💪💪.
        // We could replace Array with List(capacity: precalculatedSize) too.
        var moves = new SquareIndex[BitOperations.PopCount(attack)];

        // TODO: Create 'Move' struct and its encoding to ulong (int64).

        int opposite = pieceColor.Opposite().ToIndex();
        int moveIndex = 0;

        while (attack != 0)
        {
            SquareIndex to = attack.TrailingZeroCount();
            BitBoard toBitboard = to.Board;
            
            if ((toBitboard & _occupancies[opposite]) != 0)
            {
                // TODO: set some flag 'is capture = true'
                moves[moveIndex] = to;
            }
            
            moves[moveIndex] = to;
            attack.PopBitAt(to);
            ++moveIndex;
        }

        return moves;
    }
}
