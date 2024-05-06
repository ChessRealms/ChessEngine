using ChessRealms.ChessEngine.Core.Attacks;
using ChessRealms.ChessEngine.Core.Types;

namespace ChessRealms.ChessEngine.Tests.Core.Attacks;

public class RookSliderAttackOnTheFlyTests
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
        {
            EnumSquare.b4,
            EnumSquare.g4,
            EnumSquare.e5
        };

        SquareIndex[] expectedAttacks =
        {
            EnumSquare.e1,
            EnumSquare.e2,
            EnumSquare.e3,
            EnumSquare.e5,
            EnumSquare.b4,
            EnumSquare.c4,
            EnumSquare.d4,
            EnumSquare.f4,
            EnumSquare.g4
        };

        ulong blockers = blockerSquares.Select(x => x.BitBoard).Aggregate((b1, b2) => b1 | b2);
        ulong attackMask = RookLookups.MaskRookSliderAttackOnTheFly(attackFrom, blockers);

        ulong matchAttack = attackMask ^ expectedAttacks.Select(x => x.BitBoard).Aggregate((b1, b2) => b1 | b2);

        Assert.That(matchAttack, Is.EqualTo(0));
    }
}
