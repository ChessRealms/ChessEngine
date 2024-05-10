using ChessRealms.ChessEngine.Core.Types;

namespace ChessRealms.ChessEngine.Console;

public static class PieceCharset
{
    public const string ASCII = "PNBRQKpnbrqk";

    public const string UNICODE = "♙♘♗♖♕♔♟︎♞♝♜♛♚";

    public static char GetAsciiPiece(PieceType type, PieceColor color)
    {
        int index = (int)type - 1 + (color == PieceColor.White ? 0 : 6);
        return ASCII[index];
    }
}