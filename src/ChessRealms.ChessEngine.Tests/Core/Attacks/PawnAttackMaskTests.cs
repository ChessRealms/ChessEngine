using ChessRealms.ChessEngine.Core.Attacks;
using ChessRealms.ChessEngine.Core.Types;

namespace ChessRealms.ChessEngine.Tests.Core.Attacks;

public class PawnAttackMaskTests
{
    [Test]
    public void From_A4_AsWhitePawn()
    {
        SquareIndex attackFrom = EnumSquare.a4;
        SquareIndex attack = EnumSquare.b5;

        ulong attackMask = PawnLookups.AttackMasks[PieceColor.White][attackFrom];
        ulong matchAttack = attackMask ^ attack.BitBoard;

        Assert.That(matchAttack, Is.EqualTo(0));
    }

    [Test]
    public void From_A4_AsBlackPawn()
    {
        SquareIndex attackFrom = EnumSquare.a4;
        SquareIndex attack = EnumSquare.b3;

        ulong attackMask = PawnLookups.AttackMasks[PieceColor.Black][attackFrom];
        ulong matchAttack = attackMask ^ attack.BitBoard;

        Assert.That(matchAttack, Is.EqualTo(0));
    }

    [Test]
    public void From_H4_AsWhitePawn()
    {
        SquareIndex attackFrom = EnumSquare.h4;
        SquareIndex attack = EnumSquare.g5;

        ulong attackMask = PawnLookups.AttackMasks[PieceColor.White][attackFrom];
        ulong matchAttack = attackMask ^ attack.BitBoard;

        Assert.That(matchAttack, Is.EqualTo(0));
    }

    [Test]
    public void From_H4_AsBlackPawn()
    {
        SquareIndex attackFrom = EnumSquare.h4;
        SquareIndex attack = EnumSquare.g3;

        ulong attackMask = PawnLookups.AttackMasks[PieceColor.Black][attackFrom];
        ulong matchAttack = attackMask ^ attack.BitBoard;

        Assert.That(matchAttack, Is.EqualTo(0));
    }

    [Test]
    public void From_E5_AsWhitePawn()
    {
        SquareIndex attackFrom = EnumSquare.e5;
        SquareIndex attack1 = EnumSquare.d6;
        SquareIndex attack2 = EnumSquare.f6;

        ulong attackMask = PawnLookups.AttackMasks[PieceColor.White][attackFrom];
        ulong matchAttack = attackMask ^ (attack1.BitBoard | attack2.BitBoard);

        Assert.That(matchAttack, Is.EqualTo(0));
    }

    [Test]
    public void From_E5_AsBlackPawn()
    {
        SquareIndex attackFrom = EnumSquare.e5;
        SquareIndex attack1 = EnumSquare.d4;
        SquareIndex attack2 = EnumSquare.f4;

        ulong attackMask = PawnLookups.AttackMasks[PieceColor.Black][attackFrom];
        ulong matchAttack = attackMask ^ (attack1.BitBoard | attack2.BitBoard);

        Assert.That(matchAttack, Is.EqualTo(0));
    }
}
