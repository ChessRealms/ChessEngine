using ChessRealms.ChessEngine.Core.Types.Enums;

namespace ChessRealms.ChessEngine.Core.Extensions;

public static class TypesExtensions
{
    public static PieceColor Opposite(this PieceColor color)
    {
        return color == PieceColor.White ? PieceColor.Black : PieceColor.White;
    }
}
