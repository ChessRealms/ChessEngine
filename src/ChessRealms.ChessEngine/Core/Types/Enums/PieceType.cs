using static ChessRealms.ChessEngine.Core.Constants.ChessConstants;

namespace ChessRealms.ChessEngine.Core.Types.Enums;

public enum PieceType
{
    Pawn    = PIECE_PAWN,
    Knight  = PIECE_KNIGHT,
    Bishop  = PIECE_BISHOP,
    Rook    = PIECE_ROOK,
    Queen   = PIECE_QUEEN,
    King    = PIECE_KING,
    None    = PIECE_NONE
}