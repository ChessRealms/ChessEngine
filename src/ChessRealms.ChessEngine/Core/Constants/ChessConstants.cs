namespace ChessRealms.ChessEngine.Core.Constants;

public static class ChessConstants
{
    #region Color
    public const int COLOR_BLACK = 0;

    public const int COLOR_WHITE = 1;

    public const int COLOR_NONE = 2;
    #endregion

    #region Piece Type
    public const int PIECE_PAWN = 0;

    public const int PIECE_KNIGHT = 1;

    public const int PIECE_BISHOP = 2;

    public const int PIECE_ROOK = 3;

    public const int PIECE_QUEEN = 4;

    public const int PIECE_KING = 5;

    public const int PIECE_NONE = 6;
    #endregion

    #region Promotion Piece
    public const int PROMOTE_NONE = 0;

    public const int PROMOTE_KNIGHT = 1;

    public const int PROMOTE_BISHOP = 2;

    public const int PROMOTE_ROOK = 3;

    public const int PROMOTE_QUEEN = 4;
    #endregion

    #region Castlings
    public const int CASTLING_NONE = 0;

    public const int CASTLING_WK = 1;
    
    public const int CASTLING_WQ = 2;
    
    public const int CASTLING_BK = 4;

    public const int CASTLING_BQ = 8;
    #endregion
}
