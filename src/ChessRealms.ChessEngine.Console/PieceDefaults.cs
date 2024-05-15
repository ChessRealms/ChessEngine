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

    public static Piece GetPieceFromAscii(char symbol)
    {
        PieceType type = symbol switch 
        {
            'p' => PieceType.Pawn,
            'n' => PieceType.Knight,
            'b' => PieceType.Bishop,
            'r' => PieceType.Rook,
            'q' => PieceType.Queen,
            'k' => PieceType.King,
            _ => PieceType.None
        };

        PieceColor color = char.IsUpper(symbol) ? PieceColor.White : PieceColor.Black;
        
        return new Piece(type, color);
    }
}