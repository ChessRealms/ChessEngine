using ChessRealms.ChessEngine2.Core.Attacks;
using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Tests.Extensions;

namespace ChessRealms.ChessEngine2.Tests.Core.Attacks;

public class PawnAttackMaskTests
{
    [Test]
    public void From_A4_AsWhitePawn()
    {
        int attackFrom = Squares.a4;
        int[] attacks = [Squares.b5];

        ulong attackMask = PawnAttacks.AttackMasks[Colors.White][attackFrom];
        ulong expected = attacks.ToBitboard();

        Assert.That(attackMask, Is.EqualTo(expected));
    }

    [Test]
    public void From_A4_AsBlackPawn()
    {
        int attackFrom = Squares.a4;
        int[] attacks = [Squares.b3];

        ulong attackMask = PawnAttacks.AttackMasks[Colors.Black][attackFrom];
        ulong expected = attacks.ToBitboard();

        Assert.That(attackMask, Is.EqualTo(expected));
    }

    [Test]
    public void From_H4_AsWhitePawn()
    {
        int attackFrom = Squares.h4;
        int[] attacks = [Squares.g5];

        ulong attackMask = PawnAttacks.AttackMasks[Colors.White][attackFrom];
        ulong expected = attacks.ToBitboard();

        Assert.That(attackMask, Is.EqualTo(expected));
    }

    [Test]
    public void From_H4_AsBlackPawn()
    {
        int attackFrom = Squares.h4;
        int[] attacks = [Squares.g3];

        ulong attackMask = PawnAttacks.AttackMasks[Colors.Black][attackFrom];
        ulong expected = attacks.ToBitboard();

        Assert.That(attackMask, Is.EqualTo(expected));
    }

    [Test]
    public void From_E5_AsWhitePawn()
    {
        int attackFrom = Squares.e5;
        int[] attacks = [Squares.d6, Squares.f6];
        
        ulong attackMask = PawnAttacks.AttackMasks[Colors.White][attackFrom];
        ulong expected = attacks.ToBitboard();

        Assert.That(attackMask, Is.EqualTo(expected));
    }

    [Test]
    public void From_E5_AsBlackPawn()
    {
        int attackFrom = Squares.e5;
        int[] attacks = [Squares.d4, Squares.f4];
        
        ulong attackMask = PawnAttacks.AttackMasks[Colors.Black][attackFrom];
        ulong expected = attacks.ToBitboard();

        Assert.That(attackMask, Is.EqualTo(expected));
    }
}
