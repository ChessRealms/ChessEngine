using ChessRealms.ChessEngine.Types;
using System.Collections.Immutable;

namespace ChessRealms.ChessEngine.AttackMasks;

public static class KnightAttackMask
{
    public static readonly ImmutableArray<ulong> Instance;

    static KnightAttackMask()
    {
        ulong[] masks = new ulong[64];

        for (int i = 0; i < 64; ++i)
        {
            masks[i] = MaskKnightAttack(i);
        }

        Instance = [.. masks];
    }

    private static ulong MaskKnightAttack(SquareIndex knightIndex)
    {
        ulong board = knightIndex.BitBoard;
        ulong attacks = 0UL;

        attacks |= (board >> 17) & LerfConstants.NOT_H_FILE;
        attacks |= (board >> 15) & LerfConstants.NOT_A_FILE;
        attacks |= (board >> 10) & LerfConstants.NOT_HG_FILE;
        attacks |= (board >> 6) & LerfConstants.NOT_AB_FILE;

        attacks |= (board << 17) & LerfConstants.NOT_A_FILE;
        attacks |= (board << 15) & LerfConstants.NOT_H_FILE;
        attacks |= (board << 10) & LerfConstants.NOT_AB_FILE;
        attacks |= (board << 6) & LerfConstants.NOT_HG_FILE;

        return attacks;
    }
}
