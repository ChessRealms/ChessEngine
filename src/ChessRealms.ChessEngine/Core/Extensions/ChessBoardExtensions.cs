using ChessRealms.ChessEngine.Core.Types;

namespace ChessRealms.ChessEngine.Core.Extensions;

internal static class ChessBoardExtensions
{
    public static int ToIndex(this PieceColor color)
    {
        return ((int)color) - 1;
    }

    public static PieceColor ToColor(this int index)
    {
        return (PieceColor)(index + 1);
    }

    public static int ToIndex(this PieceType type)
    {
        return ((int)type) - 1;
    }

    public static PieceType ToPiece(this int index)
    {
        return (PieceType)(index + 1);
    }
}
