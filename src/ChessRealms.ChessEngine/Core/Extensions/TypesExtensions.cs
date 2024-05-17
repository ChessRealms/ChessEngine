using ChessRealms.ChessEngine.Core.Constants;
using ChessRealms.ChessEngine.Core.Types.Enums;

namespace ChessRealms.ChessEngine.Core.Extensions;

public static class TypesExtensions
{
    public static PieceColor Opposite(this PieceColor color)
    {
        return color == PieceColor.White ? PieceColor.Black : PieceColor.White;
    }

    public static int Opposite(this int color)
    {
        return ChessConstants.COLOR_WHITE ^ color;
    }
}
