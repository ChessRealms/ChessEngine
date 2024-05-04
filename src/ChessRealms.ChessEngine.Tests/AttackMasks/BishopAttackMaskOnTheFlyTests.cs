using ChessRealms.ChessEngine.AttackMasks;
using ChessRealms.ChessEngine.Types;
using ChessRealms.ChessEngine.Types.Enums;

namespace ChessRealms.ChessEngine.Tests.AttackMasks;

public class BishopAttackMaskOnTheFlyTests
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
        {
            EnumSquare.d3,
            EnumSquare.b7,
            EnumSquare.c6,
            EnumSquare.d5,
            EnumSquare.f5,
            EnumSquare.f3
        };

        ulong blockers = blockerSquares.Select(x => x.BitBoard).Aggregate((b1, b2) => b1 | b2);
        ulong attackMask = BishopAttackMask.MaskBishopAttackOnTheFly(attackFrom, blockers);

        ulong matchAttack = attackMask ^ expectedAttacks.Select(x => x.BitBoard).Aggregate((b1, b2) => b1 | b2);

        Assert.That(matchAttack, Is.EqualTo(0));
    }
}
