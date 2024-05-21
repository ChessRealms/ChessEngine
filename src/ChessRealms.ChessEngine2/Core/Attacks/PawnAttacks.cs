using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.Math;
using ChessRealms.ChessEngine2.Debugs;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace ChessRealms.ChessEngine2.Core.Attacks;

internal unsafe static class PawnAttacks
{
    private static readonly ulong[] attackMasksUnsafe;
    public static readonly ulong* AttackMasksUnsafe;

    public static readonly ImmutableArray<ImmutableArray<ulong>> AttackMasks;

    static PawnAttacks()
    {
        attackMasksUnsafe = new ulong[64 * 2];
        
        ulong[] white = new ulong[64];
        ulong[] black = new ulong[64];

        for (int square = 0; square < 64; ++square)
        {
            white[square] = MaskPawnAttack(Colors.White, square);
            black[square] = MaskPawnAttack(Colors.Black, square);

            attackMasksUnsafe[square * Colors.White + square] = MaskPawnAttack(Colors.White, square);
            attackMasksUnsafe[square * Colors.Black + square] = MaskPawnAttack(Colors.Black, square);
        }

        fixed (ulong* ptr = attackMasksUnsafe)
            AttackMasksUnsafe = ptr;

        AttackMasks = [[.. black], [.. white]];
    }

    /// <summary>
    /// Triggers static ctor().
    /// </summary>
    public static void InvokeInit()
    {
        // Touch variable to trigger its init (calls static ctor).
        _ = AttackMasks[0];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong GetMask(int color, int square)
    {
        DebugAsserts.ValidColor(color);
        DebugAsserts.ValidSquare(square);

        return AttackMasks[color][square];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong GetMaskUnsafe(int color, int square)
    {
        DebugAsserts.ValidColor(color);
        DebugAsserts.ValidSquare(square);

        return AttackMasksUnsafe[(color * 64) + square];
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
