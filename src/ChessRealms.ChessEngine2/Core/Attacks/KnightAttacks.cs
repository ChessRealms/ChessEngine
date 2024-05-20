using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.Math;
using System.Collections.Immutable;

namespace ChessRealms.ChessEngine2.Core.Attacks;

internal static class KnightAttacks
{
    public static readonly ImmutableArray<ulong> AttackMasks;

    static KnightAttacks()
    {
        ulong[] masks = new ulong[64];

        for (int square = 0; square < 64; ++square)
        {
            masks[square] = MaskKnightAttack(square);
        }

        AttackMasks = [.. masks];
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
