using ChessRealms.ChessEngine2.Core.Attacks;
using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Tests.Extensions;

namespace ChessRealms.ChessEngine2.Tests.Core.Attacks;

public class BishopAttackMaskTests
{
    [Test]
    public void From_D4()
    {
        int attackFrom = Squares.d4;

        int[] attacks =
        [
            Squares.b2,
            Squares.c3,
            Squares.e5,
            Squares.f6,
            Squares.g7,
            Squares.f2,
            Squares.e3,
            Squares.c5,
            Squares.b6
        ];
        
        ulong attackMask = BishopAttacks.AttackMasks[attackFrom];
        ulong expected = attacks.ToBitboard();

        Assert.That(attackMask, Is.EqualTo(expected));
    }
}
