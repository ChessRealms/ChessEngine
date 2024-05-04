using ChessRealms.ChessEngine.Types;
using System.Collections.Immutable;

namespace ChessRealms.ChessEngine.AttackMasks;

public static class BishopAttackMask
{
    public static readonly ImmutableArray<ulong> Instance;

    static BishopAttackMask()
    {
        ulong[] masks = new ulong[64];

        for (int square = 0; square < 64; ++square)
        {
            masks[square] = MaskBishopAttack(square);
        }

        Instance = [.. masks];
    }

    private static ulong MaskBishopAttack(SquareIndex square)
    {
        ulong attacks = 0UL;

        for (int r = square.Rank + 1, f = square.File + 1; r <= 6 && f <= 6; ++r, ++f)
        {
            attacks |= SquareIndex.FromFileRank(f, r).BitBoard;
        }

        for (int r = square.Rank - 1, f = square.File + 1; r >= 1 && f <= 6; --r, ++f)
        {
            attacks |= SquareIndex.FromFileRank(f, r).BitBoard;
        }

        for (int r = square.Rank + 1, f = square.File - 1; r <= 6 && f >= 1; ++r, --f)
        {
            attacks |= SquareIndex.FromFileRank(f, r).BitBoard;
        }

        for (int r = square.Rank - 1, f = square.File - 1; r >= 1 && f >= 1; --r, --f)
        {
            attacks |= SquareIndex.FromFileRank(f, r).BitBoard;
        }
        
        return attacks;
    }

    internal static ulong MaskBishopAttackOnTheFly(SquareIndex square, ulong blockers)
    {
        ulong attacks = 0UL;

        for (int r = square.Rank + 1, f = square.File + 1; r <= 7 && f <= 7; ++r, ++f)
        {
            ulong board = SquareIndex.FromFileRank(f, r).BitBoard;
            
            attacks |= board;
            
            if ((board & blockers) > 0)
            {
                break;
            }
        }

        for (int r = square.Rank - 1, f = square.File + 1; r >= 0 && f <= 7; --r, ++f)
        {
            ulong board = SquareIndex.FromFileRank(f, r).BitBoard;
            
            attacks |= board;

            if ((board & blockers) > 0)
            {
                break;
            }
        }

        for (int r = square.Rank + 1, f = square.File - 1; r <= 7 && f >= 0; ++r, --f)
        {
            ulong board = SquareIndex.FromFileRank(f, r).BitBoard;

            attacks |= board;

            if ((board & blockers) > 0)
            {
                break;
            }
        }

        for (int r = square.Rank - 1, f = square.File - 1; r >= 0 && f >= 0; --r, --f)
        {
            ulong board = SquareIndex.FromFileRank(f, r).BitBoard;

            attacks |= board;

            if ((board & blockers) > 0)
            {
                break;
            }
        }

        return attacks;
    }
}
