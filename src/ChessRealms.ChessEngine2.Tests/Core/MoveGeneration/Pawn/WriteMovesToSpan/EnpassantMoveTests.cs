using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.MoveGeneration;
using ChessRealms.ChessEngine2.Core.Types;
using ChessRealms.ChessEngine2.Parsing;

namespace ChessRealms.ChessEngine2.Tests.Core.MoveGeneration.Pawn.WriteMovesToSpan;

public class EnpassantMoveTests
{
    [Test]
    public void Test1()
    {
        //     ASCII Board (as white)
        //
        // '*' - potential pawn moves
        //
        //   a b c d e f g h
        // 8 r n b q k b n r
        // 7 p p p p p p p p
        // 6 * * . . . . . .
        // 5 p P . . . . * .
        // 4 * . * * * p P p
        // 3 * . * * P * . *
        // 2 P . P P . P . P
        // 1 R N B Q K B N R

        string fen = "rnbqkbnr/1pppp1p1/8/pP6/5pPp/4P3/P1PP1P1P/RNBQKBNR w KQkq a6 0 1";

        if (!FenStrings.TryParse(fen, out Position position))
        {
            Assert.Fail();
            return;
        }

        #region Assert by moves count
        Span<int> moves = stackalloc int[40];
        int writen = PawnMovement.WriteMovesToSpan(ref position, Colors.White, moves, offset: 0);
        Assert.That(writen, Is.EqualTo(13));
        #endregion
    }
}
