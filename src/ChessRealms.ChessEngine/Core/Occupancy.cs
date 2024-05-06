using ChessRealms.ChessEngine.Core.Types;

namespace ChessRealms.ChessEngine.Core;

public static class Occupancy
{
    /// <summary>
    /// Set occupancy by specified occupancy index and attackMask. 
    /// More details: <see href="https://www.chessprogramming.org/Sliding_Piece_Attacks#By_Occupancy_Lookup"/>
    /// </summary>
    /// <param name="occupancyIndex"> Occupancy zero-based index. </param>
    /// <param name="bitsCount"> Bits count of <paramref name="attackMask"/>. </param>
    /// <param name="attackMask"> Attack mask. </param>
    /// <returns> Occupancy BitBoard. </returns>
    public static BitBoard CreateAtIndex(int occupancyIndex, int bitsCount, BitBoard attackMask)
    {
        BitBoard occupancy = 0;

        for (int i = 0; i < bitsCount; ++i)
        {
            SquareIndex s = attackMask.LeastSignificantFirstBit();

            attackMask.PopBitAt(s);

            if ((occupancyIndex & 1 << i) > 0)
            {
                occupancy.SetBitAt(s);
            }
        }

        return occupancy;
    }
}
