using ChessRealms.ChessEngine2.Core.Movements;
using ChessRealms.ChessEngine2.Core.Types;
using ChessRealms.ChessEngine2.Parsing;

namespace ChessRealms.ChessEngine2.Tests.Core.MoveGeneration;

internal class AllMovesTests
{
    const string fen = "r3k2r/p1ppqpb1/bn1Ppnp1/4N3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b KQkq - 1 1";

    [Test]
    public void Test1_AsBlack()
    {
        FenStrings.TryParse(fen, out Position position);
        Span<int> moves = stackalloc int[218];

        int written = MoveGen.WriteMovesToSpan_v1(ref position, position.color, moves);
    }
}
