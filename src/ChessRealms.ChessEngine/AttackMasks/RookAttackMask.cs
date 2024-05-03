using ChessRealms.ChessEngine.Types;
using System.Collections.Immutable;

namespace ChessRealms.ChessEngine.AttackMasks;

public static class RookAttackMask
{
    public static readonly ImmutableArray<ulong> Instance;

    static RookAttackMask()
    {
        ulong[] masks = new ulong[64];

        for (int square = 0; square < 64; ++square)
        {
            masks[square] = MaskRookAttack(square);
        }

        Instance = [.. masks];
    }

    private static ulong MaskRookAttack(SquareIndex square)
    {
        ulong attacks = 0UL;

        for (int r = square.Rank + 1; r <= 6; ++r)
        {
            attacks |= SquareIndex.FromFileRank(square.File, r).BitBoard;
        }

        for (int r = square.Rank - 1; r >= 1; --r)
        {
            attacks |= SquareIndex.FromFileRank(square.File, r).BitBoard;
        }

        for (int f = square.File + 1; f <= 6; ++f)
        {
            attacks |= SquareIndex.FromFileRank(f, square.Rank).BitBoard;
        }

        for (int f = square.File - 1; f >= 1; --f)
        {
            attacks |= SquareIndex.FromFileRank(f, square.Rank).BitBoard;
        }

        return attacks;
    }
}
