using ChessRealms.ChessEngine.Core.Types;
using System.Collections.Immutable;

namespace ChessRealms.ChessEngine.Core.Attacks;

public static class BishopLookups
{
    /// <summary>
    /// Pre-calculated bishop slider attacks. Shape of array is <c>[64][512].</c>
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
        0x08480290920A0200,
        0x0044A80220420000,
        0x0004812401000101,
        0x0002208210000010,
        0x2006121000050003,
        0x03082404200001A0,
        0x0900A09420201010,
        0x00021202010440A0,
        0x01000850A9064402,
        0x4280201210B20080,
        0x1140100C04405000,
        0x000802208204AAF0,
        0x1802011140028000,
        0x0010020110890400,
        0x000901010822C180,
        0x05042082009A2021,
        0x0011420822280800,
        0x00444A0204040400,
        0x2021015000560040,
        0x0008000422012450,
        0x0014010210220020,
        0x4000806410140100,
        0x0424000044440408,
        0x2000402084008884,
        0x4110540150212A22,
        0x00990D0060084200,
        0x0012022041040400,
        0x1101004024040002,
        0x1001001101004000,
        0x4000810022012080,
        0x0301204044024800,
        0x0101011042004100,
        0x0008021020082040,
        0x0808268220081800,
        0x000A080401020028,
        0x0100220280080080,
        0x0004040400001100,
        0x4008080808024100,
        0x080200921D040204,
        0x1104004048920100,
        0x0101104904002043,
        0x4020422820000404,
        0x340090C128001008,
        0x0400206018000109,
        0x0004400092004902,
        0x01200810046008C3,
        0x02A00C04004624A2,
        0x0A88821046010A44,
        0x2045045002084048,
        0x100484140202A028,
        0x0200020202490072,
        0x0000888042022041,
        0x0400004208220040,
        0x0048429002008800,
        0x100404C404040754,
        0x0020040122042000,
        0x4001005100884000,
        0x004C0421A8082802,
        0x0900214454040410,
        0x0800000040840404,
        0x0590010090202200,
        0x08480820C4011201,
        0x00814A1001380300,
        0x0808012108010100
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
