using ChessRealms.ChessEngine.Core.Attacks;
using ChessRealms.ChessEngine.Core.Types;
using ChessRealms.ChessEngine.Core.Types.Enums;

using static ChessRealms.ChessEngine.Core.Constants.ChessConstants;

namespace ChessRealms.ChessEngine.Tests.Core.Attacks;

public class PawnAttackMaskTests
{
    [Test]
    public void From_A4_AsWhitePawn()
    {
        SquareIndex attackFrom = EnumSquare.a4;
        SquareIndex attack = EnumSquare.b5;

        ulong attackMask = PawnAttacks.AttackMasks[COLOR_WHITE][attackFrom];
        ulong matchAttack = attackMask ^ attack.Board;

        Assert.That(matchAttack, Is.EqualTo(0));
    }

    [Test]
    public void From_A4_AsBlackPawn()
    {
        SquareIndex attackFrom = EnumSquare.a4;
        SquareIndex attack = EnumSquare.b3;

        ulong attackMask = PawnAttacks.AttackMasks[COLOR_BLACK][attackFrom];
        ulong matchAttack = attackMask ^ attack.Board;

        Assert.That(matchAttack, Is.EqualTo(0));
    }

    [Test]
    public void From_H4_AsWhitePawn()
    {
        SquareIndex attackFrom = EnumSquare.h4;
        SquareIndex attack = EnumSquare.g5;

        ulong attackMask = PawnAttacks.AttackMasks[COLOR_WHITE][attackFrom];
        ulong matchAttack = attackMask ^ attack.Board;

        Assert.That(matchAttack, Is.EqualTo(0));
    }

    [Test]
    public void From_H4_AsBlackPawn()
    {
        SquareIndex attackFrom = EnumSquare.h4;
        SquareIndex attack = EnumSquare.g3;

        ulong attackMask = PawnAttacks.AttackMasks[COLOR_BLACK][attackFrom];
        ulong matchAttack = attackMask ^ attack.Board;

        Assert.That(matchAttack, Is.EqualTo(0));
    }

    [Test]
    public void From_E5_AsWhitePawn()
    {
        SquareIndex attackFrom = EnumSquare.e5;
        SquareIndex attack1 = EnumSquare.d6;
        SquareIndex attack2 = EnumSquare.f6;

        ulong attackMask = PawnAttacks.AttackMasks[COLOR_WHITE][attackFrom];
        ulong matchAttack = attackMask ^ (attack1.Board | attack2.Board);

        Assert.That(matchAttack, Is.EqualTo(0));
    }

    [Test]
    public void From_E5_AsBlackPawn()
    {
        SquareIndex attackFrom = EnumSquare.e5;
        SquareIndex attack1 = EnumSquare.d4;
        SquareIndex attack2 = EnumSquare.f4;

        ulong attackMask = PawnAttacks.AttackMasks[COLOR_BLACK][attackFrom];
        ulong matchAttack = attackMask ^ (attack1.Board | attack2.Board);

        Assert.That(matchAttack, Is.EqualTo(0));
    }
}
