using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.Math;
using ChessRealms.ChessEngine2.Debugs;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace ChessRealms.ChessEngine2.Core.Attacks;

internal static class PawnAttacks
{
    public static readonly ImmutableArray<ulong> AttackMasks;

    static PawnAttacks()
    {
        ulong[] white = new ulong[64];
        ulong[] black = new ulong[64];

        for (int square = 0; square < 64; ++square)
        {
            white[square] = MaskPawnAttack(Colors.White, square);
            black[square] = MaskPawnAttack(Colors.Black, square);
        }

        AttackMasks = [.. black, .. white];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong GetAttackMask(int color, int square)
    {
        DebugAsserts.ValidColor(color);
        DebugAsserts.ValidSquare(square);

        return AttackMasks[(color * 64) + square];
    }

    /// <summary>
    /// Triggers static ctor().
    /// </summary>
    public static void InvokeInit()
    {
        // Touch variable to trigger its init (calls static ctor).
        _ = AttackMasks[0];
    }

    private static ulong MaskPawnAttack(int color, int square)
    {
        DebugAsserts.ValidColor(color);
        DebugAsserts.ValidSquare(square);

        ulong board = SquareOps.ToBitboard(square);
        ulong attacks = 0UL;

        if (color == Colors.White)
        {
            attacks |= board << 7 & SquareMapping.NOT_H_FILE;
            attacks |= board << 9 & SquareMapping.NOT_A_FILE;
        }
        else
        {
            attacks |= board >> 7 & SquareMapping.NOT_A_FILE;
            attacks |= board >> 9 & SquareMapping.NOT_H_FILE;
        }

        return attacks;
    }
}
