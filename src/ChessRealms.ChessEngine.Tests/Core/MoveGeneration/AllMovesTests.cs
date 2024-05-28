using ChessRealms.ChessEngine2.Core.Movements;
using ChessRealms.ChessEngine2.Core.Types;
using ChessRealms.ChessEngine2.Parsing;

namespace ChessRealms.ChessEngine2.Tests.Core.MoveGeneration;

internal unsafe class AllMovesTests
{
    const string fen = "r3k2r/p1ppqpb1/bn1Ppnp1/4N3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b KQkq - 1 1";

    [Test]
    public void Test1_AsBlack()
    {
        Assert.That(FenStrings.TryParse(fen, out Position position), Is.True);
        Position* positionPtr = &position;
        int* moves = stackalloc int[218];

        int written = MoveGen.WriteMovesToPtrUnsafe(positionPtr, position.color, moves);
        Assert.That(written, Is.EqualTo(39));
    }
}
