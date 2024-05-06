﻿using ChessRealms.ChessEngine.Core.Types;
using System.Collections.Immutable;

namespace ChessRealms.ChessEngine.Core.Attacks;

internal static class RookLookups
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
        0x0C80108004400020,
        0x0240002000100042,
        0x0880088020001001,
        0x0080100080080004,
        0x0600102004520008,
        0x2200102200081114,
        0x0080020001000080,
        0x0200089200402401,
        0x0002002041020080,
        0x2002002080410204,
        0x0002004020820014,
        0x0080801000080080,
        0x0002808004000800,
        0x000A000410020088,
        0x004A004802004104,
        0x0082000044008201,
        0x0040018002402880,
        0x0040004040201000,
        0x0000808020001000,
        0x4400090021001004,
        0x0012020020100408,
        0x0802008002800400,
        0x4500040001308208,
        0x001002000110408C,
        0x0120400080208001,
        0x0000200240100040,
        0x0010200080100080,
        0x0130008280080013,
        0x0001000500080010,
        0x2800020080800400,
        0x0001000100020004,
        0x0010008200010044,
        0x4021400061800883,
        0x4050002000C00040,
        0x0910204082001200,
        0x0000201001000900,
        0x0A00800402800800,
        0x0400800200800400,
        0x0802000182000804,
        0x0100009112000044,
        0x0880004020004004,
        0x0940500020004001,
        0x0008200049010010,
        0x0058041000808008,
        0x1005000801710024,
        0x0801000204010008,
        0x00205001C2040008,
        0x000C064320920004,
        0x080A805022030200,
        0x0240200840100440,
        0x0010002000801080,
        0x120842208A001200,
        0x1008001104090100,
        0x0400040080020080,
        0x2000100208810400,
        0x0000104900840A00,
        0x0041002810800043,
        0x00008900400032A1,
        0x0110402000090011,
        0x0430005100C82005,
        0x200A001004200802,
        0x640100040002080B,
        0x5100408802101104,
        0x0821002C81040042
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
