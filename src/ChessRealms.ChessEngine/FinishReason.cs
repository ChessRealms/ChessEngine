namespace ChessRealms.ChessEngine;

public enum FinishReason
{
    None = 0,
    Draw = 1,
    Mate = 2,
    VoteForDraw = 3,
    Resign = 4,
    Stalemate = 5
}
