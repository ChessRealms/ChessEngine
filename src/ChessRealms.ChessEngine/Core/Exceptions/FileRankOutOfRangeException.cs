namespace ChessRealms.ChessEngine.Core.Exceptions;

public sealed class FileRankOutOfRangeException(
    string? paramName,
    object? actualValue,
    string? message
    ) : ArgumentOutOfRangeException(
        paramName: paramName, 
        actualValue: actualValue,
        message: message)
{
    internal const string InvalidRankMessage = "Invalid 'rank' value. Rank must be in range [0 ... 7].";
    internal const string InvalidFileMessage = "Invalid 'file' value. File must be in range ['a' ... 'h'].";
}