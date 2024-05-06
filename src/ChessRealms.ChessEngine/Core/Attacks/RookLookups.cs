using ChessRealms.ChessEngine.Core.Types;
using System.Collections.Immutable;

namespace ChessRealms.ChessEngine.Core.Attacks;

public static class RookLookups
{
    /// <summary>
    /// Pre-calculated rook slider attacks. Array have fixed size: SliderAttacks[64][4096].
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
        0x0490048408404800,
        0x0108210900280112,
        0x001840840014000E,
        0x002901400404000A,
        0x0100286002C40060,
        0x0011104801D00000,
        0x0000050005C20002,
        0x0088881010421008,
        0x151A102020403888,
        0x442009A00A808000,
        0x0010440100101180,
        0x30C4406094410004,
        0x4A08280284000108,
        0x0100042200400080,
        0x0082108964000800,
        0x004008000420A200,
        0x0028200002802049,
        0x08040030001028D1,
        0x0000890000032594,
        0x2202402050D002A0,
        0x0000100840400000,
        0x0882C48000000100,
        0x0001000080282001,
        0x1080000220820811,
        0x400D404000008000,
        0x0805200803080004,
        0x10800100001E0000,
        0x1008402800240800,
        0x0000204418018288,
        0x2314408A00008881,
        0x060014C001080808,
        0x0100290200040320,
        0x0020407098010003,
        0x2004000302220440,
        0x6018858208504004,
        0x40020000800A0C10,
        0x0001028020001008,
        0x0400810620181010,
        0x0588200400002292,
        0x0602A00008A00600,
        0x020000F002001010,
        0x4084112A04304004,
        0x0006018400140140,
        0x0000002000800241,
        0x0008601088100360,
        0x0080212240010080,
        0x4000101460004882,
        0x00444D8425002280,
        0x10001101A0104004,
        0x0000680210080000,
        0x0000000002624008,
        0x208018200404500C,
        0x0200002100044400,
        0x302208C8D1300001,
        0x00300000604084A0,
        0x0018A09104081104,
        0x78601C0000880600,
        0x40001008110A0290,
        0x00C3D0400B00C120,
        0x4024040008098600,
        0x2800049008102401,
        0x0440400801440408,
        0x4000202401820002,
        0x500010140000200C
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
