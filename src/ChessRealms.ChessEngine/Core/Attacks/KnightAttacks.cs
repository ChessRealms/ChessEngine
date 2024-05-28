using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.Math;
using System.Collections.Immutable;

namespace ChessRealms.ChessEngine2.Core.Attacks;

internal static class KnightAttacks
{
    public static readonly ImmutableArray<ulong> AttackMasks;

    static KnightAttacks()
    {
        var attackMasks = new ulong[64];

        for (int square = 0; square < 64; ++square)
        {
            attackMasks[square] = MaskKnightAttack(square);
        }

        AttackMasks = [.. attackMasks];
    }

    /// <summary>
    /// Triggers static ctor().
    /// </summary>
    public static void InvokeInit()
    {
        // Touch variable to trigger static ctor.
        _ = AttackMasks[0];
    }

    public static ulong MaskKnightAttack(int square)
    {
        ulong board = SquareOps.ToBitboard(square);
        ulong attacks = 0UL;

        attacks |= board >> 17 & SquareMapping.NOT_H_FILE;
        attacks |= board >> 15 & SquareMapping.NOT_A_FILE;
        attacks |= board >> 10 & SquareMapping.NOT_HG_FILE;
        attacks |= board >> 6 & SquareMapping.NOT_AB_FILE;

        attacks |= board << 17 & SquareMapping.NOT_A_FILE;
        attacks |= board << 15 & SquareMapping.NOT_H_FILE;
        attacks |= board << 10 & SquareMapping.NOT_AB_FILE;
        attacks |= board << 6 & SquareMapping.NOT_HG_FILE;

        return attacks;
    }
}
