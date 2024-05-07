using ChessRealms.ChessEngine.Core.Types;
using System.Collections.Immutable;

namespace ChessRealms.ChessEngine.Core.Attacks;

internal static class KingAttacks
{
    internal static readonly ImmutableArray<ulong> AttackMasks;

    static KingAttacks()
    {
        ulong[] masks = new ulong[64];

        for (int square = 0; square < 64; ++square)
        {
            masks[square] = MaskKingAttack(square);
        }

        AttackMasks = [.. masks];
    }

    private static ulong MaskKingAttack(SquareIndex square)
    {
        ulong board = square.Board;
        ulong attacks = 0UL;

        attacks |= board << 8;
        attacks |= board >> 8;

        attacks |= board << 9 & LerfConstants.NOT_A_FILE;
        attacks |= board << 1 & LerfConstants.NOT_A_FILE;
        attacks |= board >> 7 & LerfConstants.NOT_A_FILE;

        attacks |= board >> 9 & LerfConstants.NOT_H_FILE;
        attacks |= board >> 1 & LerfConstants.NOT_H_FILE;
        attacks |= board << 7 & LerfConstants.NOT_H_FILE;

        return attacks;
    }
}
