using ChessRealms.ChessEngine.Core.Attacks;
using ChessRealms.ChessEngine.Core.Types;
using ChessRealms.ChessEngine.Tests.Extensions;

namespace ChessRealms.ChessEngine.Tests.Core.Attacks;

public class KingAttackMaskTests
{
    [Test]
    public void From_D4()
    {
        SquareIndex attackFrom = EnumSquare.d4;

        SquareIndex[] attacks =
        [
            EnumSquare.c3, EnumSquare.c4, EnumSquare.c5,
            EnumSquare.d3, EnumSquare.d5,
            EnumSquare.e3, EnumSquare.e4, EnumSquare.e5
        ];

        ulong attackMask = KingLookups.AttackMasks[attackFrom];
        ulong attackMatch = attackMask ^ attacks.ToBitBoard();

        Assert.That(attackMatch, Is.EqualTo(0));
    }

    [Test]
    public void From_A1()
    {
        SquareIndex attackFrom = EnumSquare.a1;

        SquareIndex attack1 = EnumSquare.a2;
        SquareIndex attack2 = EnumSquare.b1;
        SquareIndex attack3 = EnumSquare.b2;

        ulong attackMask = KingLookups.AttackMasks[attackFrom];
        ulong attackMatch = attackMask;

        attackMatch ^= attack1.BitBoard | attack2.BitBoard | attack3.BitBoard;

        Assert.That(attackMatch, Is.EqualTo(0));
    }

    [Test]
    public void From_A8()
    {
        SquareIndex attackFrom = EnumSquare.a8;

        SquareIndex attack1 = EnumSquare.a7;
        SquareIndex attack2 = EnumSquare.b7;
        SquareIndex attack3 = EnumSquare.b8;

        ulong attackMask = KingLookups.AttackMasks[attackFrom];
        ulong attackMatch = attackMask;

        attackMatch ^= attack1.BitBoard | attack2.BitBoard | attack3.BitBoard;

        Assert.That(attackMatch, Is.EqualTo(0));
    }

    [Test]
    public void From_H1()
    {
        SquareIndex attackFrom = EnumSquare.h1;

        SquareIndex attack1 = EnumSquare.h2;
        SquareIndex attack2 = EnumSquare.g1;
        SquareIndex attack3 = EnumSquare.g2;

        ulong attackMask = KingLookups.AttackMasks[attackFrom];
        ulong attackMatch = attackMask;

        attackMatch ^= attack1.BitBoard | attack2.BitBoard | attack3.BitBoard;

        Assert.That(attackMatch, Is.EqualTo(0));
    }

    [Test]
    public void From_H8()
    {
        SquareIndex attackFrom = EnumSquare.h8;

        SquareIndex attack1 = EnumSquare.h7;
        SquareIndex attack2 = EnumSquare.g7;
        SquareIndex attack3 = EnumSquare.g8;

        ulong attackMask = KingLookups.AttackMasks[attackFrom];
        ulong attackMatch = attackMask;

        attackMatch ^= attack1.BitBoard | attack2.BitBoard | attack3.BitBoard;

        Assert.That(attackMatch, Is.EqualTo(0));
    }
}
