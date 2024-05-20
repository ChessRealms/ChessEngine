using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ChessRealms.ChessEngine2.Core.Math;

public static class BitboardOps
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong GetBitAt(ulong bitboard, int square)
    {
        return bitboard & SquareOps.ToBitboard(square);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong SetBitAt(ulong bitboard, int square)
    {
        return bitboard | SquareOps.ToBitboard(square);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong PopBitAt(ulong bitboard, int square)
    {
        return bitboard ^ SquareOps.ToBitboard(square);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty(ulong bitboard)
    {
        return bitboard == 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotEmpty(ulong bitboard)
    {
        return !IsEmpty(bitboard);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Lsb(ulong bitboard)
    {
        Debug.Assert(IsNotEmpty(bitboard));
        return BitOperations.TrailingZeroCount(bitboard);
    }
}
