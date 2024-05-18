using static ChessRealms.ChessEngine.Core.Constants.ChessConstants;

namespace ChessRealms.ChessEngine.Core.Types.Enums;

[Flags]
public enum Castling
{
    None = CASTLING_NONE,
    WK = CASTLING_WK,
    WQ = CASTLING_WQ,
    BK = CASTLING_BK,
    BQ = CASTLING_BQ
}
