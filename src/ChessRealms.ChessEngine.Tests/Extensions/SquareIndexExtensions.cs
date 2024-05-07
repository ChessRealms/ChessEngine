using ChessRealms.ChessEngine.Core.Types;

namespace ChessRealms.ChessEngine.Tests.Extensions;

internal static class SquareIndexExtensions
{
    public static BitBoard ToBitBoard(this IEnumerable<SquareIndex> indicies)
    {
        return indicies.Select(x => x.Board).Aggregate((b1, b2) => b1 | b2);
    }
}
