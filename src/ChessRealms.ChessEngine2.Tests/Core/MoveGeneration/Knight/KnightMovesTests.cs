using ChessRealms.ChessEngine2.Core.Attacks;
using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.Math;
using ChessRealms.ChessEngine2.Core.Movements;
using ChessRealms.ChessEngine2.Core.Types;
using ChessRealms.ChessEngine2.Parsing;
using System.Diagnostics;

namespace ChessRealms.ChessEngine2.Tests.Core.MoveGeneration.Knight;

internal class KnightMovesTests
{
    //     ASCII Board (as white)
    //
    //   a b c d e f g h
    // 8 . . . . . . P .
    // 7 . . . p . . . P
    // 6 . . . . . n . .
    // 5 . . . p . . . P
    // 4 P . . . p . P .
    // 3 . . N . . . . .
    // 2 P . . . p . . .
    // 1 . P . p . . . .
    //   a b c d e f g h
    //
    private const string fen = "6P1/3p3P/5n2/3p3P/P3p1P1/2N5/P3p3/1P1p4 b - - 0 1";
    private Position position;

    public KnightMovesTests()
    {
        bool parsed = FenStrings.TryParse(fen, out position);
        Debug.Assert(parsed);
    }

    [Test]
    public void Test_AsWhite()
    {
        int color = Colors.White;
        int expectedWritten = 5;
        
        Span<int> moves = stackalloc int[expectedWritten];
        int written = LeapingMovement.WriteMovesToSpan(
            ref position, 
            color, 
            Pieces.Knight,
            KnightAttacks.AttackMasks,
            moves);
        
        Assert.That(written, Is.EqualTo(expectedWritten));

        var moveSet = moves.ToArray().ToHashSet();
        int[] expectedMoves =
        [
            BinaryMoveOps.EncodeMove(
                Squares.c3, Pieces.Knight, color, Squares.b5),
            BinaryMoveOps.EncodeMove(
                Squares.c3, Pieces.Knight, color, Squares.d5, capture: 1),
            BinaryMoveOps.EncodeMove(
                Squares.c3, Pieces.Knight, color, Squares.e4, capture: 1),
            BinaryMoveOps.EncodeMove(
                Squares.c3, Pieces.Knight, color, Squares.e2, capture: 1),
            BinaryMoveOps.EncodeMove(
                Squares.c3, Pieces.Knight, color, Squares.d1, capture: 1),
        ];

        Assert.That(expectedMoves.All(moveSet.Contains), Is.True);
    }

    [Test]
    public void Test_AsBlack()
    {
        int color = Colors.Black;
        int expectedWritten = 5;

        Span<int> moves = stackalloc int[expectedWritten];
        int written = LeapingMovement.WriteMovesToSpan(
            ref position,
            Colors.Black,
            Pieces.Knight,
            KnightAttacks.AttackMasks,
            moves);
        
        Assert.That(written, Is.EqualTo(expectedWritten));

        var moveSet = moves.ToArray().ToHashSet();      
        int[] expectedMoves =
        [
            BinaryMoveOps.EncodeMove(
                Squares.f6, Pieces.Knight, color, Squares.e8),
            BinaryMoveOps.EncodeMove(
                Squares.f6, Pieces.Knight, color, Squares.g8, capture: 1),
            BinaryMoveOps.EncodeMove(
                Squares.f6, Pieces.Knight, color, Squares.h7, capture: 1),
            BinaryMoveOps.EncodeMove(
                Squares.f6, Pieces.Knight, color, Squares.h5, capture: 1),
            BinaryMoveOps.EncodeMove(
                Squares.f6, Pieces.Knight, color, Squares.g4, capture: 1),
        ];

        Assert.That(expectedMoves.All(moveSet.Contains), Is.True);
    }
}
