using ChessRealms.ChessEngine.Core.Types;
using System.Collections.Immutable;

namespace ChessRealms.ChessEngine.Core.Attacks;

public static class RookLookups
{
    /// <summary>
    /// Pre-calculated rook slider attacks. Shape of array is <c>[64][4096]</c>.
    /// </summary>
    internal static readonly ImmutableArray<ImmutableArray<ulong>> SliderAttacks;

    /// <summary>
    /// Pre-calculated rook attack masks (no outer squares) for each square index from 0 to 63.
    /// </summary>
    internal static readonly ImmutableArray<ulong> AttackMasks;

    /// <summary>
    /// Pre-calculated bit counts for each rook attack mask.
    /// </summary>
    internal static readonly ImmutableArray<int> RelevantBits =
    [
         12, 11, 11, 11, 11, 11, 11, 12,
         11, 10, 10, 10, 10, 10, 10, 11,
         11, 10, 10, 10, 10, 10, 10, 11,
         11, 10, 10, 10, 10, 10, 10, 11,
         11, 10, 10, 10, 10, 10, 10, 11,
         11, 10, 10, 10, 10, 10, 10, 11,
         11, 10, 10, 10, 10, 10, 10, 11,
         12, 11, 11, 11, 11, 11, 11, 12
    ];
        
    /// <summary>
    /// Pre-calculated rook magic numbers for each square index from 0 to 63.
    /// </summary>
    internal static readonly ImmutableArray<ulong> MagicNumbers =
    [
        0x508000802040001C,
        0x2040004020001004,
        0x0200104020820008,
        0x0100081000200500,
        0x4200040810020020,
        0x0300020814000100,
        0x0400302A08010884,
        0x008002A500004580,
        0x0001800080400020,
        0x0820804000200085,
        0x0004802000100089,
        0x4001801800100082,
        0x0201000408001100,
        0x0400808002000400,
        0x2184000830420144,
        0x0A01001080510002,
        0x0840008000204090,
        0x0801060020420080,
        0x0228820020104200,
        0x0108008008801000,
        0x4001010010080005,
        0x0201010004000802,
        0x0900040002100108,
        0x0400220001008044,
        0x0240800080204000,
        0x01400020A0100803,
        0x0140200080801000,
        0x0010100080080480,
        0x0400050100080011,
        0x4088040080020080,
        0x0002000200010804,
        0x60220A8200240141,
        0x0800400028800C84,
        0x08A0005000400028,
        0x0200802000801008,
        0x0004100101000824,
        0x0100814402800800,
        0x1009841008012040,
        0x00C0100804008241,
        0x0400A10042000084,
        0x1000420100820020,
        0x0460004030004004,
        0x2201002000110044,
        0x0800100008008080,
        0x0240880100110005,
        0x009A001104020008,
        0x002B000200010004,
        0x40010000A0410002,
        0x02008002244B0100,
        0x20C0070A40208100,
        0x000B802040120200,
        0x0000800800100080,
        0x1400100408010100,
        0x0102800200040080,
        0x00109A0819300400,
        0x0A008002E9000980,
        0x0000220010408902,
        0x030040802102001A,
        0x0001002000100A41,
        0x0001002048100095,
        0x0041000402080011,
        0x4011000208140003,
        0x1800100812408104,
        0x00A8010044008022
    ];

    static RookLookups()
    {
        ulong[] attackMasks = new ulong[64];
        ulong[][] sliderAttacks = new ulong[64][];

        for (int square = 0; square < 64; ++square)
        {
            attackMasks[square] = MaskRookAttack(square);
            sliderAttacks[square] = new ulong[4096];

            int occupancyIndicies = 1 << RelevantBits[square];

            for (int index = 0; index < occupancyIndicies; ++index)
            {
                ulong occupancy = Occupancy.CreateAtIndex(index, RelevantBits[square], attackMasks[square]);
                
                ulong magicIndex = (occupancy * MagicNumbers[square]) >> (64 - RelevantBits[square]);

                sliderAttacks[square][magicIndex] = MaskRookSliderAttackOnTheFly(square, occupancy);
            }
        }

        AttackMasks = [.. attackMasks];
        SliderAttacks = sliderAttacks.Select(x => x.ToImmutableArray()).ToImmutableArray();
    }

    /// <summary>
    /// Get rook slider attack for specified square and occupancy.
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
    /// Create rook mask (without outer squares) for specified index.
    /// </summary>
    /// <param name="square"> Index of square at chessboard. </param>
    /// <returns> Attack mask (no outer squares). </returns>
    internal static BitBoard MaskRookAttack(SquareIndex square)
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

    /// <summary>
    /// Create rook mask (with outer squares) for specified square index and blockers.
    /// </summary>
    /// <param name="square"> Index of square at chessboard. </param>
    /// <param name="blockers"> Board of blockers. </param>
    /// <returns> Attack mask. </returns>
    internal static ulong MaskRookSliderAttackOnTheFly(SquareIndex square, BitBoard blockers)
    {
        ulong attacks = 0UL;

        for (int r = square.Rank + 1; r <= 7; ++r)
        {
            ulong board = SquareIndex.FromFileRank(square.File, r).BitBoard;

            attacks |= board;

            if ((board & blockers) > 0)
            {
                break;
            }
        }

        for (int r = square.Rank - 1; r >= 0; --r)
        {
            ulong board = SquareIndex.FromFileRank(square.File, r).BitBoard;

            attacks |= board;

            if ((board & blockers) > 0)
            {
                break;
            }
        }

        for (int f = square.File + 1; f <= 7; ++f)
        {
            ulong board = SquareIndex.FromFileRank(f, square.Rank).BitBoard;

            attacks |= board;

            if ((board & blockers) > 0)
            {
                break;
            }
        }

        for (int f = square.File - 1; f >= 0; --f)
        {
            ulong board = SquareIndex.FromFileRank(f, square.Rank).BitBoard;

            attacks |= board;

            if ((board & blockers) > 0)
            {
                break;
            }
        }

        return attacks;
    }
}
