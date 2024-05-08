using ChessRealms.ChessEngine.Core.Types;

namespace ChessRealms.ChessEngine.Core.Extensions;

public static class TypesExtensions
{
    public static PieceColor Opposite(this PieceColor color)
    {
        return color == PieceColor.White ? PieceColor.Black : PieceColor.Black;
    }
}
