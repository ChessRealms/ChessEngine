using System.Numerics;
using System.Text;

namespace ChessRealms.ChessEngine.Core.Types;

public readonly struct BitBoard(ulong value)
{
    public readonly ulong Value = value;

    public readonly BitBoard GetBitAt(SquareIndex squareIndex)
    {
        return Value & (1ul << squareIndex.Value);
    }

    public readonly bool TryGetBitAt(SquareIndex squareIndex, out BitBoard bitBoard)
    {
        bitBoard = Value & (1ul << squareIndex.Value);
        return bitBoard != 0;
    }

    public readonly BitBoard PopBitAt(SquareIndex squareIndex)
    {
        return Value ^ (1ul << squareIndex.Value);
    }

    public readonly bool TryPopBitAt(SquareIndex squareIndex, out BitBoard bitBoard)
    {
        bitBoard = Value ^ (1ul << squareIndex.Value);
        return bitBoard != 0;
    }

    public readonly BitBoard PopFirstSquare(out SquareIndex squareIndex)
    {
        if (Value == 0)
        {
            squareIndex = SquareIndex.None;
            return this;
        }

        squareIndex = Ls1b();
        return PopBitAt(squareIndex);
    }

    public readonly bool TryPopFirstSquare(out SquareIndex squareIndex, out BitBoard bitBoard)
    {
        if (Value == 0)
        {
            squareIndex = SquareIndex.None;
            bitBoard = 0ul;
            return false;
        }

        squareIndex = Ls1b();
        bitBoard = PopBitAt(squareIndex);
        return true;
    }

    public readonly BitBoard SetBitAt(SquareIndex index)
    {
        return Value | index.Board;
    }

    internal readonly int Ls1b()
    {
        return BitOperations.TrailingZeroCount(Value);
    }

    #region Implicit UInt64
    public static implicit operator BitBoard(ulong value) => new(value);

    public static implicit operator ulong(BitBoard board) => board.Value;
    #endregion
}
