using ChessRealms.ChessEngine.Core.Constants;
using ChessRealms.ChessEngine.Core.Math;
using ChessRealms.ChessEngine.Debugs;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ChessRealms.ChessEngine.Core.Attacks;

internal static unsafe class PawnAttacks
{
    public static readonly ulong[] AttackMasks;
    public static readonly ulong* AttackMasksPtr;

    static PawnAttacks()
    {
        AttackMasks = new ulong[64 * 2];

        for (int square = 0; square < 64; ++square)
        {
            AttackMasks[square + 64] = MaskPawnAttack(Colors.White, square);
            AttackMasks[square] = MaskPawnAttack(Colors.Black, square);
        }

        AttackMasksPtr = (ulong*)GCHandle.Alloc(AttackMasks, GCHandleType.Pinned).AddrOfPinnedObject().ToPointer();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong GetAttackMask(int color, int square)
    {
        DebugHelper.Assert.IsValidColor(color);
        DebugHelper.Assert.IsValidSquare(square);

        return AttackMasksPtr[(color * 64) + square];
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
        DebugHelper.Assert.IsValidColor(color);
        DebugHelper.Assert.IsValidSquare(square);

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
