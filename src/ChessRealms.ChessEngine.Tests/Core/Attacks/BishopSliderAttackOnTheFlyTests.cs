using ChessRealms.ChessEngine.Core.Attacks;
using ChessRealms.ChessEngine.Core.Types;
using ChessRealms.ChessEngine.Tests.Extensions;

namespace ChessRealms.ChessEngine.Tests.Core.Attacks;

public class BishopSliderAttackOnTheFlyTests
{
    [Test]
    public void From_E4()
    {
        // Expected Attacks at Board
        //   a b c d e f g h
        // 1 . . . . . . . .
        // 2 . . . . . . . .
        // 3 . . . * . * . .
        // 4 . . . . b . . .
        // 5 . . . * . * . .
        // 6 . . * . . . . .
        // 7 . * . . . . . .
        // 8 . . . . . . . .

        SquareIndex attackFrom = EnumSquare.e4;

        SquareIndex[] blockerSquares =
        [
            EnumSquare.d3,
            EnumSquare.b7,
            EnumSquare.f5,
            EnumSquare.f3
        ];

        SquareIndex[] expectedAttacks =
        [
            EnumSquare.d3,
            EnumSquare.b7,
            EnumSquare.c6,
            EnumSquare.d5,
            EnumSquare.f5,
            EnumSquare.f3
        ];

        ulong blockers = blockerSquares.ToBitBoard();
        ulong attackMask = BishopAttacks.MaskBishopSliderAttackOnTheFly(attackFrom, blockers);

        ulong matchAttack = attackMask ^ expectedAttacks.ToBitBoard();

        Assert.That(matchAttack, Is.EqualTo(0));
    }

    [Test]
    public void From_D4()
    {
        // Expected Attacks at Board
        //   a b c d e f g h
        // 1 . . . . . . . .
        // 2 . . . . . . . .
        // 3 . . * . * . . .
        // 4 . . . b . . . .
        // 5 . . * . * . . .
        // 6 . . . . . * . .
        // 7 . . . . . . . .
        // 8 . . . . . . . .

        SquareIndex attackFrom = EnumSquare.d4;

        SquareIndex[] blockerSquares =
        [
            EnumSquare.c3,
            EnumSquare.e3,
            EnumSquare.c5,
            EnumSquare.f6
        ];

        SquareIndex[] expectedAttacks =
        [
            EnumSquare.c3,
            EnumSquare.e3,
            EnumSquare.c5,
            EnumSquare.e5,
            EnumSquare.f6
        ];

        ulong blockers = blockerSquares.ToBitBoard();
        ulong attackMask = BishopAttacks.MaskBishopSliderAttackOnTheFly(attackFrom, blockers);

        ulong matchAttack = attackMask ^ expectedAttacks.ToBitBoard();

        Assert.That(matchAttack, Is.EqualTo(0));
    }

    [Test]
    public void From_A1_NoBlockers()
    {
        // Expected Attacks at Board
        //   a b c d e f g h
        // 1 b . . . . . . .
        // 2 . * . . . . . .
        // 3 . . * . . . . .
        // 4 . . . * . . . .
        // 5 . . . . * . . .
        // 6 . . . . . * . .
        // 7 . . . . . . * .
        // 8 . . . . . . . *

        SquareIndex attackFrom = EnumSquare.a1;
        SquareIndex[] expectedAttacks =
        [
            EnumSquare.b2,
            EnumSquare.c3,
            EnumSquare.d4,
            EnumSquare.e5,
            EnumSquare.f6,
            EnumSquare.g7,
            EnumSquare.h8
        ];

        ulong blockers = 0UL;
        ulong attackMask = BishopAttacks.MaskBishopSliderAttackOnTheFly(attackFrom, blockers);

        ulong matchAttack = attackMask ^ expectedAttacks.ToBitBoard();

        Assert.That(matchAttack, Is.EqualTo(0));
    }

    [Test]
    public void From_H1()
    {
        // Expected Attacks at Board
        //   a b c d e f g h
        // 1 . . . . . . . b
        // 2 . . . . . . * .
        // 3 . . . . . * . .
        // 4 . . . . * . . .
        // 5 . . . * . . . .
        // 6 . . . . . . . .
        // 7 . . . . . . . .
        // 8 . . . . . . . .

        SquareIndex attackFrom = EnumSquare.h1;

        SquareIndex[] blockerSquares =
        [
            EnumSquare.d5
        ];

        SquareIndex[] expectedAttacks =
        [
            EnumSquare.g2,
            EnumSquare.f3,
            EnumSquare.e4,
            EnumSquare.d5
        ];

        ulong blockers = blockerSquares.ToBitBoard();
        ulong attackMask = BishopAttacks.MaskBishopSliderAttackOnTheFly(attackFrom, blockers);

        ulong matchAttack = attackMask ^ expectedAttacks.ToBitBoard();

        Assert.That(matchAttack, Is.EqualTo(0));
    }
}
