using ChessRealms.ChessEngine.AttackMasks;
using ChessRealms.ChessEngine.Types;
using ChessRealms.ChessEngine.Types.Enums;

namespace ChessRealms.ChessEngine.Tests.AttackMasks;

public class KinghtAttackMaskTests
{
    [Test]
    public void From_D5()
    {
        SquareIndex attackFrom = EnumSquare.d5;

        SquareIndex attack1 = EnumSquare.e7;
        SquareIndex attack2 = EnumSquare.f6;
        SquareIndex attack3 = EnumSquare.f4;
        SquareIndex attack4 = EnumSquare.e3;

        SquareIndex attack5 = EnumSquare.c3;
        SquareIndex attack6 = EnumSquare.b4;
        SquareIndex attack7 = EnumSquare.b6;
        SquareIndex attack8 = EnumSquare.c7;

        ulong attackMask = KnightAttackMask.Instance[attackFrom];
        ulong matchAttack = attackMask;

        matchAttack ^= attack1.BitBoard | attack2.BitBoard | attack3.BitBoard | attack4.BitBoard;
        matchAttack ^= attack5.BitBoard | attack6.BitBoard | attack7.BitBoard | attack8.BitBoard;

        Assert.That(matchAttack, Is.EqualTo(0));
    }

    [Test]
    public void From_A1()
    {
        SquareIndex attackFrom = EnumSquare.a1;
        SquareIndex attack1 = EnumSquare.b3;
        SquareIndex attack2 = EnumSquare.c2;

        ulong attackMask = KnightAttackMask.Instance[attackFrom];
        ulong matchAttack = attackMask ^ (attack1.BitBoard | attack2.BitBoard);

        Assert.That(matchAttack, Is.EqualTo(0));
    }

    [Test]
    public void From_A8() 
    {   
        SquareIndex attackFrom = EnumSquare.a8;
        SquareIndex attack1 = EnumSquare.b6;
        SquareIndex attack2 = EnumSquare.c7;

        ulong attackMask = KnightAttackMask.Instance[attackFrom];
        ulong matchAttack = attackMask ^ (attack1.BitBoard | attack2.BitBoard);

        Assert.That(matchAttack, Is.EqualTo(0));
    }

    [Test]
    public void From_H1()
    {
        SquareIndex attackFrom = EnumSquare.h1;
        SquareIndex attack1 = EnumSquare.f2;
        SquareIndex attack2 = EnumSquare.g3;

        ulong attackMask = KnightAttackMask.Instance[attackFrom];
        ulong matchAttack = attackMask ^ (attack1.BitBoard | attack2.BitBoard);

        Assert.That(matchAttack, Is.EqualTo(0));
    }

    [Test]
    public void From_H8()
    {
        SquareIndex attackFrom = EnumSquare.h8;
        SquareIndex attack1 = EnumSquare.f7;
        SquareIndex attack2 = EnumSquare.g6;

        ulong attackMask = KnightAttackMask.Instance[attackFrom];
        ulong matchAttack = attackMask ^ (attack1.BitBoard | attack2.BitBoard);

        Assert.That(matchAttack, Is.EqualTo(0));
    }
}
