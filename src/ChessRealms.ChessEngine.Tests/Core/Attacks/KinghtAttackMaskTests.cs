using ChessRealms.ChessEngine.Core.Attacks;
using ChessRealms.ChessEngine.Core.Types;
using ChessRealms.ChessEngine.Tests.Extensions;

namespace ChessRealms.ChessEngine.Tests.Core.Attacks;

public class KinghtAttackMaskTests
{
    [Test]
    public void From_D5()
    {
        SquareIndex attackFrom = EnumSquare.d5;

        SquareIndex[] attacks =
        [
            EnumSquare.e7, EnumSquare.c3,
            EnumSquare.f6, EnumSquare.b4,
            EnumSquare.f4, EnumSquare.b6,
            EnumSquare.e3, EnumSquare.c7
        ];

        ulong attackMask = KnightAttacks.AttackMasks[attackFrom];
        ulong matchAttack = attackMask ^ attacks.ToBitBoard();

        Assert.That(matchAttack, Is.EqualTo(0));
    }

    [Test]
    public void From_A1()
    {
        SquareIndex attackFrom = EnumSquare.a1;
        SquareIndex attack1 = EnumSquare.b3;
        SquareIndex attack2 = EnumSquare.c2;

        ulong attackMask = KnightAttacks.AttackMasks[attackFrom];
        ulong matchAttack = attackMask ^ (attack1.Board | attack2.Board);

        Assert.That(matchAttack, Is.EqualTo(0));
    }

    [Test]
    public void From_A8()
    {
        SquareIndex attackFrom = EnumSquare.a8;
        SquareIndex attack1 = EnumSquare.b6;
        SquareIndex attack2 = EnumSquare.c7;

        ulong attackMask = KnightAttacks.AttackMasks[attackFrom];
        ulong matchAttack = attackMask ^ (attack1.Board | attack2.Board);

        Assert.That(matchAttack, Is.EqualTo(0));
    }

    [Test]
    public void From_H1()
    {
        SquareIndex attackFrom = EnumSquare.h1;
        SquareIndex attack1 = EnumSquare.f2;
        SquareIndex attack2 = EnumSquare.g3;

        ulong attackMask = KnightAttacks.AttackMasks[attackFrom];
        ulong matchAttack = attackMask ^ (attack1.Board | attack2.Board);

        Assert.That(matchAttack, Is.EqualTo(0));
    }

    [Test]
    public void From_H8()
    {
        SquareIndex attackFrom = EnumSquare.h8;
        SquareIndex attack1 = EnumSquare.f7;
        SquareIndex attack2 = EnumSquare.g6;

        ulong attackMask = KnightAttacks.AttackMasks[attackFrom];
        ulong matchAttack = attackMask ^ (attack1.Board | attack2.Board);

        Assert.That(matchAttack, Is.EqualTo(0));
    }
}
