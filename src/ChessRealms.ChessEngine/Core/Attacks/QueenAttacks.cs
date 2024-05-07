using ChessRealms.ChessEngine.Core.Types;

namespace ChessRealms.ChessEngine.Core.Attacks;

internal static class QueenAttacks
{
    /// <summary>
    /// Get queen slider attack for specified square and occupancy.
    /// </summary>
    /// <param name="square"> Index of square at chessboard. </param>
    /// <param name="occupancy"> Occupancy at chessboard. </param>
    /// <returns> Attack mask. </returns>
    internal static BitBoard GetSliderAttack(SquareIndex square, ulong occupancy)
    {
        BitBoard attacks = RookAttacks.GetSliderAttack(square, occupancy);
        attacks |= BishopAttacks.GetSliderAttack(square, occupancy);
        return attacks;
    }
}
