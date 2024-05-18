using ChessRealms.ChessEngine.Core.Attacks;
using ChessRealms.ChessEngine.Core.Types;
using ChessRealms.ChessEngine.Core.Types.Enums;
using ChessRealms.ChessEngine.Tests.Extensions;

namespace ChessRealms.ChessEngine.Tests.Core.Attacks;
public class RookSliderAttackLookupTests
{
    [Test]
    public void From_E4()
    {
        // Expected Attacks at Board
        //
        //   a b c d e f g h
        // 1 . . . . * . . .
        // 2 . . . . * . . .
        // 3 . . . . * . . .
        // 4 . * * * r * * .
        // 5 . . . . * . . .
        // 6 . . . . . . . .
        // 7 . . . . . . . .
        // 8 . . . . . . . .

        SquareIndex attackFrom = EnumSquare.e4;

        SquareIndex[] blockerSquares =
        [
            EnumSquare.b4,
            EnumSquare.g4,
            EnumSquare.e5
        ];

        SquareIndex[] expectedAttacks =
        [
            EnumSquare.e1,
            EnumSquare.e2,
            EnumSquare.e3,
            EnumSquare.e5,
            EnumSquare.b4,
            EnumSquare.c4,
            EnumSquare.d4,
            EnumSquare.f4,
            EnumSquare.g4
        ];

        ulong blockers = blockerSquares.ToBitBoard();
        ulong attackMask = RookAttacks.GetSliderAttack(attackFrom, blockers);

        ulong matchAttack = attackMask ^ expectedAttacks.ToBitBoard();

        Assert.That(matchAttack, Is.EqualTo(0));
    }

    [Test]
    public void From_A1_NoBlockers()
    {
        // Expected Attacks at Board
        //
        //   a b c d e f g h
        // 1 r * * * * * * *
        // 2 * . . . . . . .
        // 3 * . . . . . . .
        // 4 * . . . . . . .
        // 5 * . . . . . . .
        // 6 * . . . . . . .
        // 7 * . . . . . . .
        // 8 * . . . . . . .

        SquareIndex attackFrom = EnumSquare.a1;

        SquareIndex[] expectedAttacks =
        [
            EnumSquare.a2, EnumSquare.b1,
            EnumSquare.a3, EnumSquare.c1,
            EnumSquare.a4, EnumSquare.d1,
            EnumSquare.a5, EnumSquare.e1,
            EnumSquare.a6, EnumSquare.f1,
            EnumSquare.a7, EnumSquare.g1,
            EnumSquare.a8, EnumSquare.h1
        ];

        ulong blockers = 0UL;
        ulong attackMask = RookAttacks.GetSliderAttack(attackFrom, blockers);

        ulong matchAttack = attackMask ^ expectedAttacks.ToBitBoard();

        Assert.That(matchAttack, Is.EqualTo(0));
    }

    [Test]
    public void From_H1()
    {
        // Expected Attacks at Board
        //
        //   a b c d e f g h
        // 1 . . . * * * * r
        // 2 . . . . . . . *
        // 3 . . . . . . . *
        // 4 . . . . . . . *
        // 5 . . . . . . . *
        // 6 . . . . . . . .
        // 7 . . . . . . . .
        // 8 . . . . . . . .

        SquareIndex attackFrom = EnumSquare.h1;

        SquareIndex[] blockerSquares =
        [
            EnumSquare.d1,
            EnumSquare.h5
        ];

        SquareIndex[] expectedAttacks =
        [
            EnumSquare.d1, EnumSquare.h2,
            EnumSquare.e1, EnumSquare.h3,
            EnumSquare.f1, EnumSquare.h4,
            EnumSquare.g1, EnumSquare.h5
        ];

        ulong blockers = blockerSquares.ToBitBoard();
        ulong attackMask = RookAttacks.GetSliderAttack(attackFrom, blockers);

        ulong matchAttack = attackMask ^ expectedAttacks.ToBitBoard();

        Assert.That(matchAttack, Is.EqualTo(0));
    }
}
