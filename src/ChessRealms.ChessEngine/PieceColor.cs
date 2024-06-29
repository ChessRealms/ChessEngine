using System.Runtime.CompilerServices;

namespace ChessRealms.ChessEngine;

public enum PieceColor
{
    Black = 0,
    White = 1,
    None = 2
}

public static class PieceColorExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBlackOrWhite(this PieceColor color)
    {
        return color == PieceColor.Black || color == PieceColor.White;
    }
}