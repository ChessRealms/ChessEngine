using ChessRealms.ChessEngine.Types;
using System.Collections.Immutable;

namespace ChessRealms.ChessEngine.AttackMasks;

public static class KnightAttackMask
{
    public static readonly ImmutableArray<ulong> Instance;

    static KnightAttackMask()
    {
        ulong[] masks = new ulong[64];

        for (int square = 0; square < 64; ++square)
        {
            masks[square] = MaskKnightAttack(square);
        }

        Instance = [.. masks];
    }

    private static ulong MaskKnightAttack(SquareIndex square)
    {
        ulong board = square.BitBoard;
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
