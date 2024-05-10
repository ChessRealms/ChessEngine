namespace ChessRealms.ChessEngine.Core.Types;

[Flags]
public enum Castling
{
    None    = 0,
    WK      = 1,
    WQ      = 2,
    BK      = 4,
    BQ      = 8
}
