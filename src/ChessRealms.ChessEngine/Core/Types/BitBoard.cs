﻿using System.Numerics;

namespace ChessRealms.ChessEngine.Core.Types;

public struct BitBoard(ulong value)
{
    public ulong Value { get; private set; } = value;

    public BitBoard() : this(0UL) { }

    public readonly BitBoard GetBitAt(SquareIndex index)
    {
        return Value | index.BitBoard;
    }

    public void PopBitAt(SquareIndex index)
    {
        if (GetBitAt(index) > 0)
        {
            Value ^= index.BitBoard;
        }
    }

    public void SetBitAt(SquareIndex index)
    {
        Value |= index.BitBoard;
    }

    public void SetBitsAt(IEnumerable<SquareIndex> indicies)
    {
        foreach (SquareIndex index in indicies)
        {
            Value |= index.BitBoard;
        }
    }

    public readonly SquareIndex LeastSignificantFirstBit()
    {
        return BitOperations.TrailingZeroCount(Value);
    }

    public static implicit operator BitBoard(ulong value) => new(value);

    public static implicit operator ulong(BitBoard board) => board.Value;
}