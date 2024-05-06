﻿using ChessRealms.ChessEngine.Core.Attacks;
using ChessRealms.ChessEngine.Core.Types;

namespace ChessRealms.ChessEngine.Tests.Core.Attacks;

public class BishopAttackMaskTests
{
    [Test]
    public void From_D4()
    {
        SquareIndex attackFrom = EnumSquare.d4;

        SquareIndex[] attacks =
        [
            EnumSquare.b2,
            EnumSquare.c3,
            EnumSquare.e5,
            EnumSquare.f6,
            EnumSquare.g7,
            EnumSquare.f2,
            EnumSquare.e3,
            EnumSquare.c5,
            EnumSquare.b6
        ];

        ulong attackMask = BishopLookups.AttackMasks[attackFrom];
        ulong matchAttack = attackMask ^ attacks.Select(a => a.BitBoard).Aggregate((b1, b2) => b1 | b2);

        Assert.That(matchAttack, Is.EqualTo(0));
    }
}