namespace ChessRealms.ChessEngine2.Core.Attacks;

internal static class QueenAttacks
{
    /// <summary>
    /// Get queen slider attack for specified square and occupancy.
    /// </summary>
    /// <param name="square"> Index of square at chessboard. </param>
    /// <param name="occupancy"> Occupancy at chessboard. </param>
    /// <returns> Attack mask. </returns>
    public static ulong GetSliderAttack(int square, ulong occupancy)
    {
        ulong attacks = RookAttacks.GetSliderAttack(square, occupancy);
        attacks |= BishopAttacks.GetSliderAttack(square, occupancy);
        return attacks;
    }
}
