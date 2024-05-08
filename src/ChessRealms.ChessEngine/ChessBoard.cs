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

        int color = (int)(whiteOcc.GetBitAt(square) > 0 
            ? PieceColor.White 
            : PieceColor.Black);
        
        int piece = -1;

        for (int pieceIndex = 0; pieceIndex < 6; ++pieceIndex)
        {
            if (_pieces[color, pieceIndex].GetBitAt(square) > 0)
            {
                piece = pieceIndex;
                break;
            }
        }

        if (piece == -1)
        {
            return null;
        }

        return new Piece
        {
            Color = (PieceColor)color,
            Type = (PieceType)piece
        };
    }

    public void SetPieceAt(SquareIndex square, PieceColor color, PieceType piece)
    {
        int colorIndex = (int)color;
        int pieceIndex = (int)piece;
        
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

        int colorIndex = (int)piece.Color;
        int pieceIndex = (int)piece.Type;

        _pieces[colorIndex, pieceIndex].PopBitAt(square);
        _occupancies[colorIndex].PopBitAt(square);
        _allOccupancies.PopBitAt(square);
    }

    public readonly bool IsSquareAttacked(SquareIndex square, PieceColor attackerColor)
    {
        int attacker = (int)attackerColor;

        return (PawnAttacks.AttackMasks[attackerColor.Opposite()][square] & _pieces[attacker, (int)PieceType.Pawn]) > 0
            || (KnightAttacks.AttackMasks[square] & _pieces[attacker, (int)PieceType.Knight]) > 0
            || (BishopAttacks.GetSliderAttack(square, _allOccupancies) & _pieces[attacker, (int)PieceType.Bishop]) > 0
            || (RookAttacks.GetSliderAttack(square, _allOccupancies) & _pieces[attacker, (int)PieceType.Rook]) > 0
            || (KingAttacks.AttackMasks[square] & _pieces[attacker, (int)PieceType.King]) > 0;
    }

    public readonly SquareIndex[] GetBishopMoves(SquareIndex from, PieceColor sideToMoveColor)
    {
        BitBoard attack = BishopAttacks.GetSliderAttack(from, _allOccupancies);

        // Remove our pieces from attacks.
        attack ^= 0UL ^ (attack & _occupancies[(int)sideToMoveColor]);
        // Finally we could set exact size for output array.
        // 💪💪💪 Every bit and memory allocation is matter when this is Chess Engine!!! 💪💪💪.
        // We could replace Array with List(capacity: precalculatedSize) too.
        var moves = new SquareIndex[BitOperations.PopCount(attack)];

        int opposite = (int)sideToMoveColor.Opposite();
        int index = 0;

        while (attack > 0)
        {
            SquareIndex to = attack.TrailingZeroCount();
            BitBoard toBitboard = to.Board;
            
            if ((toBitboard & _occupancies[opposite]) > 0)
            {
                // TODO: set some flag 'is capture = true'
                moves[index] = to;
            }
            
            moves[index] = to;
            attack.PopBitAt(to);
            ++index;
        }

        return moves;
    }
}
