using ChessRealms.ChessEngine2.Core.Math;

namespace ChessRealms.ChessEngine2.Tests.Extensions;

internal static class SquareIndexExtensions
{
    public static ulong ToBitboard(this IEnumerable<int> squares)
    {
        return squares
            .Select(SquareOps.ToBitboard)
            .Aggregate((b1, b2) => b1 | b2);
    }
}
