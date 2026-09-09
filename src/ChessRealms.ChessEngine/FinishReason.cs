namespace ChessRealms.ChessEngine;

public enum FinishReason
{
    None,
    Checkmate,
    Stalemate,
    DeadPosition,
    ThreefoldRepetition,
    FiftyMoveRule,
    FivefoldRepetition,
    SeventyFiveMoveRule
}
