using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.Math;
using ChessRealms.ChessEngine2.Core.MoveGeneration;
using ChessRealms.ChessEngine2.Core.Types;
using ChessRealms.ChessEngine2.Parsing;

namespace ChessRealms.ChessEngine2.Tests.Core.MoveGeneration.Pawn;

internal class PromoteMoveTests
{

    //     ASCII Board (as white)
    //
    //   a b c d e f g h
    // 8 . . . . q . . .
    // 7 . . . . . P . .
    // 6 . . . K . . . .
    // 5 . . . . . . . .
    // 4 . . . . . . . .
    // 3 . k . . . . . .
    // 2 . . . p . . . .
    // 1 . . Q . . . . .
    //   a b c d e f g h
    //
    const string fen = "4q3/5P2/3K4/8/8/1k6/3p4/2Q5 b - - 0 1";

    private Position position;

    public PromoteMoveTests()
    {
        _ = FenStrings.TryParse(fen, out position);
    }

    [Test]
    public void Test_Promotes_White() 
    {
        int color = Colors.White;
        int expectedLength = 8;
        Span<int> moves = stackalloc int[expectedLength];

        int written = PawnMovement.WriteMovesToSpan(ref position, color, moves);
        var moveSet = moves.ToArray().ToHashSet();
        Assert.That(written, Is.EqualTo(expectedLength));

        int[] expectedMoves =
        [
            BinaryMoveOps.EncodeMove(
                Squares.f7, Pieces.Pawn, color, Squares.f8,
                promotion: Promotions.Knight),
            BinaryMoveOps.EncodeMove(
                Squares.f7, Pieces.Pawn, color, Squares.f8,
                promotion: Promotions.Bishop),
            BinaryMoveOps.EncodeMove(
                Squares.f7, Pieces.Pawn, color, Squares.f8,
                promotion: Promotions.Rook),
            BinaryMoveOps.EncodeMove(
                Squares.f7, Pieces.Pawn, color, Squares.f8,
                promotion: Promotions.Queen),
            
            BinaryMoveOps.EncodeMove(
                Squares.f7, Pieces.Pawn, color, Squares.e8,
                promotion: Promotions.Knight, capture: 1),
            BinaryMoveOps.EncodeMove(
                Squares.f7, Pieces.Pawn, color, Squares.e8,
                promotion: Promotions.Bishop, capture: 1),
            BinaryMoveOps.EncodeMove(
                Squares.f7, Pieces.Pawn, color, Squares.e8,
                promotion: Promotions.Rook, capture: 1),
            BinaryMoveOps.EncodeMove(
                Squares.f7, Pieces.Pawn, color, Squares.e8,
                promotion: Promotions.Queen, capture: 1),
        ];

        Assert.That(expectedMoves.All(moveSet.Contains), Is.True);
    }

    [Test]
    public void Test_Promotes_Black() 
    {
        int color = Colors.Black;
        int expectedLength = 8;
        Span<int> moves = stackalloc int[expectedLength];

        int written = PawnMovement.WriteMovesToSpan(ref position, color, moves);
        var moveSet = moves.ToArray().ToHashSet();
        Assert.That(written, Is.EqualTo(expectedLength));

        int[] expectedMoves =
        [
            BinaryMoveOps.EncodeMove(
                Squares.d2, Pieces.Pawn, color, Squares.d1,
                promotion: Promotions.Knight),
            BinaryMoveOps.EncodeMove(
                Squares.d2, Pieces.Pawn, color, Squares.d1,
                promotion: Promotions.Bishop),
            BinaryMoveOps.EncodeMove(
                Squares.d2, Pieces.Pawn, color, Squares.d1,
                promotion: Promotions.Rook),
            BinaryMoveOps.EncodeMove(
                Squares.d2, Pieces.Pawn, color, Squares.d1,
                promotion: Promotions.Queen),
            
            BinaryMoveOps.EncodeMove(
                Squares.d2, Pieces.Pawn, color, Squares.c1,
                promotion: Promotions.Knight, capture: 1),
            BinaryMoveOps.EncodeMove(
                Squares.d2, Pieces.Pawn, color, Squares.c1,
                promotion: Promotions.Bishop, capture: 1),
            BinaryMoveOps.EncodeMove(
                Squares.d2, Pieces.Pawn, color, Squares.c1,
                promotion: Promotions.Rook, capture: 1),
            BinaryMoveOps.EncodeMove(
                Squares.d2, Pieces.Pawn, color, Squares.c1,
                promotion: Promotions.Queen, capture: 1),
        ];

        Assert.That(expectedMoves.All(moveSet.Contains), Is.True);
    }
}
