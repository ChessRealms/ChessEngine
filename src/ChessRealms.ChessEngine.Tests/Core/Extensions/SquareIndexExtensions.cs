using ChessRealms.ChessEngine.Core.Math;

namespace ChessRealms.ChessEngine.Tests.Extensions;

internal static class SquareIndexExtensions
{
    public static ulong ToBitboard(this IEnumerable<int> squares)
    {
        return squares
            .Select(SquareOps.ToBitboard)
            .Aggregate((b1, b2) => b1 | b2);
    }
}
