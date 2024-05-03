using ChessRealms.ChessEngine.AttackMasks;
using ChessRealms.ChessEngine.Types;
using ChessRealms.ChessEngine.Types.Enums;

namespace ChessRealms.ChessEngine.Tests.AttackMasks;

public class KingAttackMaskTests
{
    [Test]
    public void From_D4()
    {
        SquareIndex attackFrom = EnumSquare.d4;

        SquareIndex attack1 = EnumSquare.d3;
        SquareIndex attack2 = EnumSquare.d5;

        SquareIndex attack3 = EnumSquare.c3;
        SquareIndex attack4 = EnumSquare.c4;
        SquareIndex attack5 = EnumSquare.c5;

        SquareIndex attack6 = EnumSquare.e3;
        SquareIndex attack7 = EnumSquare.e4;
        SquareIndex attack8 = EnumSquare.e5;

        ulong attackMask = KingAttackMask.Instance[attackFrom];
        ulong attackMatch = attackMask;

        attackMatch ^= attack1.BitBoard ^ attack2.BitBoard;
        attackMatch ^= attack3.BitBoard ^ attack4.BitBoard ^ attack5.BitBoard;
        attackMatch ^= attack6.BitBoard ^ attack7.BitBoard ^ attack8.BitBoard;

        Assert.That(attackMatch, Is.EqualTo(0));
    }

    [Test]
    public void From_A1()
    {
        SquareIndex attackFrom = EnumSquare.a1;

        SquareIndex attack1 = EnumSquare.a2;
        SquareIndex attack2 = EnumSquare.b1;
        SquareIndex attack3 = EnumSquare.b2;

        ulong attackMask = KingAttackMask.Instance[attackFrom];
        ulong attackMatch = attackMask ^ attack1.BitBoard ^ attack2.BitBoard ^ attack3.BitBoard;

        Assert.That(attackMatch, Is.EqualTo(0));
    }

    [Test]
    public void From_A8()
    {
        SquareIndex attackFrom = EnumSquare.a8;

        SquareIndex attack1 = EnumSquare.a7;
        SquareIndex attack2 = EnumSquare.b7;
        SquareIndex attack3 = EnumSquare.b8;

        ulong attackMask = KingAttackMask.Instance[attackFrom];
        ulong attackMatch = attackMask ^ attack1.BitBoard ^ attack2.BitBoard ^ attack3.BitBoard;

        Assert.That(attackMatch, Is.EqualTo(0));
    }

    [Test]
    public void From_H1()
    {
        SquareIndex attackFrom = EnumSquare.h1;

        SquareIndex attack1 = EnumSquare.h2;
        SquareIndex attack2 = EnumSquare.g1;
        SquareIndex attack3 = EnumSquare.g2;

        ulong attackMask = KingAttackMask.Instance[attackFrom];
        ulong attackMatch = attackMask ^ attack1.BitBoard ^ attack2.BitBoard ^ attack3.BitBoard;

        Assert.That(attackMatch, Is.EqualTo(0));
    }

    [Test]
    public void From_H8()
    {
        SquareIndex attackFrom = EnumSquare.h8;

        SquareIndex attack1 = EnumSquare.h7;
        SquareIndex attack2 = EnumSquare.g7;
        SquareIndex attack3 = EnumSquare.g8;

        ulong attackMask = KingAttackMask.Instance[attackFrom];
        ulong attackMatch = attackMask ^ attack1.BitBoard ^ attack2.BitBoard ^ attack3.BitBoard;

        Assert.That(attackMatch, Is.EqualTo(0));
    }
}
