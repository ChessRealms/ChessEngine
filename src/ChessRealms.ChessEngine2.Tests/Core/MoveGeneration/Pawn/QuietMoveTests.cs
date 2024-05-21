using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.Math;
using ChessRealms.ChessEngine2.Core.MoveGeneration;
using ChessRealms.ChessEngine2.Core.Types;
using ChessRealms.ChessEngine2.Parsing;

namespace ChessRealms.ChessEngine2.Tests.Core.MoveGeneration.Pawn;

internal class QuietMoveTests
{
    private const string ParseFenError = "Cant parse fen string.";

    [Test]
    public void QuietMoves_White_StartPos()
    {
        //     ASCII Board
        //   a b c d e f g h
        // 8 r n b q k b n r
        // 7 p p p p p p p p
        // 6 . . . . . . . .
        // 5 . . . . . . . .
        // 4 * * * * * * * *
        // 3 * * * * * * * *
        // 2 P P P P P P P P
        // 1 R N B Q K B N R

        if (!FenStrings.TryParse(FenStrings.StartPosition, out Position position))
        {
            Assert.Fail(ParseFenError);
            return;
        }

        #region Assert by move count
        Span<int> moves = stackalloc int[40];

        int written = PawnMovement.WriteMovesToSpan(ref position, Colors.White, moves, 0);

        Assert.That(written, Is.EqualTo(16));
        #endregion

        #region Assert by move equals
        var movesSet = moves[..written].ToArray().ToHashSet();

        int a2a4 = BinaryMoveOps.EncodeMove(
            Squares.a2, Pieces.Pawn, Colors.White,
            Squares.a4, doublePush: 1);

        int h2h3 = BinaryMoveOps.EncodeMove(
            Squares.h2, Pieces.Pawn, Colors.White,
            Squares.h3);

        int d2d4 = BinaryMoveOps.EncodeMove(
            Squares.d2, Pieces.Pawn, Colors.White,
            Squares.d4, doublePush: 1);

        Assert.That(movesSet, Does.Contain(a2a4));
        Assert.That(movesSet, Does.Contain(d2d4));
        Assert.That(movesSet, Does.Contain(h2h3));
        #endregion
    }

    [Test]
    public void QuietTest2()
    {

    }

    [Test]
    public void QuietTest3()
    {

    }
}
