using ChessRealms.ChessEngine2.Core.Constants;
using System.Diagnostics;

namespace ChessRealms.ChessEngine2.Debugs;

internal static class DebugAsserts
{
    const string DEBUG = "DEBUG";

    [Conditional(DEBUG)]
    public static void ValidColor(int color)
    {
        Debug.Assert(Colors.IsValid(color), "Invalid Color.", "Actual color value: '{0}'.", color);
    }

    [Conditional(DEBUG)]
    public static void ValidSquare(int square)
    {
        Debug.Assert(Squares.IsValid(square), "Invalid Square.", "Actual square value: '{0}'.", square);
    }

    [Conditional(DEBUG)]
    public static void ValidPiece(int piece)
    {
        Debug.Assert(Pieces.IsValid(piece), "Invalid Piece.", "Actual piece value: '{0}'.", piece);
    }
}
