using ChessRealms.ChessEngine.Core.Constants;
using ChessRealms.ChessEngine.Core.Types;
using ChessRealms.ChessEngine.Core.Types.Enums;

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
        int type = symbol switch 
        {
            'p' => ChessConstants.PIECE_PAWN,
            'n' => ChessConstants.PIECE_KNIGHT,
            'b' => ChessConstants.PIECE_BISHOP,
            'r' => ChessConstants.PIECE_ROOK,
            'q' => ChessConstants.PIECE_QUEEN,
            'k' => ChessConstants.PIECE_KING,
            _ => ChessConstants.PIECE_NONE
        };

        int color = char.IsUpper(symbol) ? ChessConstants.COLOR_WHITE : ChessConstants.COLOR_BLACK;
        
        return new Piece(type, color);
    }
}