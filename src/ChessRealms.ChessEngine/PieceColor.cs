using ChessRealms.ChessEngine.Core.Constants;
using System.Runtime.CompilerServices;

namespace ChessRealms.ChessEngine;

public enum PieceColor
{
    Black = Colors.Black,
    White = Colors.White,
    None = Colors.None
}

public static class PieceColorExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBlackOrWhite(this PieceColor color)
    {
        return color == PieceColor.Black || color == PieceColor.White;
    }
}