using ChessRealms.ChessEngine.Core.Attacks;
using ChessRealms.ChessEngine.Core.Types;

namespace ChessRealms.ChessEngine.Tests.Core.Attacks;

public class RookAttackMaskTests
{
    [Test]
    public void From_D4()
    {
        SquareIndex attackFrom = EnumSquare.d4;

        SquareIndex[] attacks =
        [
            EnumSquare.d2,
            EnumSquare.d3,
            EnumSquare.d5,
            EnumSquare.d6,
            EnumSquare.d7,
            EnumSquare.b4,
            EnumSquare.c4,
            EnumSquare.e4,
            EnumSquare.f4,
            EnumSquare.g4
        ];

        ulong attackMask = RookLookups.AttackMasks[attackFrom];
        ulong matchAttack = attackMask ^ attacks.Select(a => a.BitBoard).Aggregate((b1, b2) => b1 | b2);

        Assert.That(matchAttack, Is.EqualTo(0));
    }

    [Test]
    public void From_D1()
    {
        SquareIndex attackFrom = EnumSquare.d1;

        SquareIndex[] attacks =
        [
            EnumSquare.d2,
            EnumSquare.d3,
            EnumSquare.d4,
            EnumSquare.d5,
            EnumSquare.d6,
            EnumSquare.d7,
            EnumSquare.b1,
            EnumSquare.c1,
            EnumSquare.e1,
            EnumSquare.f1,
            EnumSquare.g1
        ];

        ulong attackMask = RookLookups.AttackMasks[attackFrom];
        ulong matchAttack = attackMask ^ attacks.Select(a => a.BitBoard).Aggregate((b1, b2) => b1 | b2);

        Assert.That(matchAttack, Is.EqualTo(0));
    }
}
