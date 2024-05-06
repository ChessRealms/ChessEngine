using ChessRealms.ChessEngine.Core.Types;
using System.Collections.Immutable;

namespace ChessRealms.ChessEngine.Core.Attacks;

public static class BishopLookups
{
    /// <summary>
    /// Pre-calculated bishop slider attacks. Array have fixed size: SliderAttacks[64][512].
    /// </summary>
    internal static readonly ImmutableArray<ImmutableArray<ulong>> SliderAttacks;

    /// <summary>
    /// Pre-calculated bishop attack masks (no outer squares) for each square index from 0 to 63.
    /// </summary>
    internal static readonly ImmutableArray<ulong> AttackMasks;

    /// <summary>
    /// Pre-calculated bit counts for each bishop attack mask.
    /// </summary>
    internal static readonly ImmutableArray<int> RelevantBits =
    [
         6, 5, 5, 5, 5, 5, 5, 6,
         5, 5, 5, 5, 5, 5, 5, 5,
         5, 5, 7, 7, 7, 7, 5, 5,
         5, 5, 7, 9, 9, 7, 5, 5,
         5, 5, 7, 9, 9, 7, 5, 5,
         5, 5, 7, 7, 7, 7, 5, 5,
         5, 5, 5, 5, 5, 5, 5, 5,
         6, 5, 5, 5, 5, 5, 5, 6
    ];

    /// <summary>
    /// Pre-calculated bishop magic numbers for each square index from 0 to 63.
    /// </summary>
    internal static readonly ImmutableArray<ulong> MagicNumbers =
    [
        0x442024008A850208,
        0x0104010202120040,
        0x6104040082041080,
        0x0108068100008000,
        0x4802021010100200,
        0x0002021084880242,
        0x0040421010080420,
        0x02104100A8200204,
        0x09DC418882141049,
        0x0208020298010900,
        0x0000100082104080,
        0x0000880841102004,
        0x0100A40420100040,
        0x1210C1010841C014,
        0x04000E0104602484,
        0x0840004042505008,
        0x614000A0840400C8,
        0x0002C2A808080890,
        0x0004406802040010,
        0x0080800802004420,
        0x060B000090400201,
        0x2605000200424204,
        0x0010410101503002,
        0x422A081C80808820,
        0x0108400108028880,
        0x0004053020910400,
        0x0004010850050020,
        0x012A002088008060,
        0x0484082004002001,
        0x400804201A009421,
        0x0001284024020800,
        0x1002020021434640,
        0x4408201100285204,
        0x4001280200212405,
        0x1124021300860401,
        0x001C404800458200,
        0x080C080200242008,
        0x0202080200214440,
        0x0102280108084400,
        0x1894004880260080,
        0x0048280885100800,
        0x050200A209002000,
        0x0002E82290040801,
        0x0024092018001100,
        0x2020080100404401,
        0x0840102102080040,
        0x0308212404011080,
        0x20D1040316002040,
        0x0202012402400020,
        0x0159260206203004,
        0x0000224404440000,
        0x2402000042020000,
        0x0028580450441909,
        0x0800881001AA0001,
        0x0510821888009000,
        0x0010300200504200,
        0x4400884042104000,
        0x0100084048149020,
        0x0000800023080886,
        0x4000000050841105,
        0x18C1828084104400,
        0x0000000811040820,
        0x210005E008011500,
        0x0202A02C04118020
    ];

    static BishopLookups()
    {
        ulong[] attackMasks = new ulong[64];
        ulong[][] sliderAttacks = new ulong[64][];

        for (int square = 0; square < 64; ++square)
        {
            attackMasks[square] = MaskBishopAttack(square);
            sliderAttacks[square] = new ulong[512];

            int occupancyIndicies = 1 << RelevantBits[square];

            for (int index = 0; index < occupancyIndicies; ++index)
            {
                ulong occupancy = Occupancy.CreateAtIndex(index, RelevantBits[square], attackMasks[square]);
                
                ulong magicIndex = (occupancy * MagicNumbers[square]) >> (64 - RelevantBits[square]);

                sliderAttacks[square][magicIndex] = MaskBishopSliderAttackOnTheFly(square, occupancy);
            }
        }

        AttackMasks = [.. attackMasks];
        SliderAttacks = sliderAttacks.Select(x => x.ToImmutableArray()).ToImmutableArray();
    }

    /// <summary>
    /// Get bishop slider attack for specified square and occupancy.
    /// </summary>
    /// <param name="square"> Index of square at chessboard. </param>
    /// <param name="occupancy"> Occupancy at chessboard. </param>
    /// <returns> Attack mask. </returns>
    internal static BitBoard GetSliderAttack(SquareIndex square, ulong occupancy)
    {
        occupancy &= AttackMasks[square];
        occupancy *= MagicNumbers[square];
        occupancy >>= 64 - RelevantBits[square];
        return SliderAttacks[square][(int)occupancy];
    }

    /// <summary>
    /// Create bishop mask (without outer squares) for specified index.
    /// </summary>
    /// <param name="square"> Index of square at chessboard. </param>
    /// <returns> Attack mask (no outer squares). </returns>
    internal static ulong MaskBishopAttack(SquareIndex square)
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

    /// <summary>
    /// Create bishop mask (with outer squares) for specified square index and blockers.
    /// </summary>
    /// <param name="square"> Index of square at chessboard. </param>
    /// <param name="blockers"> Board of blockers. </param>
    /// <returns> Attack mask. </returns>
    internal static ulong MaskBishopSliderAttackOnTheFly(SquareIndex square, ulong blockers)
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
