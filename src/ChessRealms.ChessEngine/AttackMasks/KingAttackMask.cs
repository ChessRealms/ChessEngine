using ChessRealms.ChessEngine.Types;
using System.Collections.Immutable;

namespace ChessRealms.ChessEngine.AttackMasks;

public static class KingAttackMask
{
    public static readonly ImmutableArray<ulong> Instance;

    static KingAttackMask()
    {
        ulong[] masks = new ulong[64];

        for (int square = 0; square < 64; ++square)
        {
            masks[square] = MaskKingAttack(square);
        }

        Instance = [.. masks];
    }

    private static ulong MaskKingAttack(SquareIndex square)
    {
        ulong board = square.BitBoard;
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
