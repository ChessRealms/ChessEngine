﻿using ChessRealms.ChessEngine2.Core.Attacks;
using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Tests.Extensions;

namespace ChessRealms.ChessEngine2.Tests.Core.Attacks;

public class RookAttackMaskTests
{
    [Test]
    public void From_D4()
    {
        int attackFrom = Squares.d4;

        int[] attacks =
        [
            Squares.d2,
            Squares.d3,
            Squares.d5,
            Squares.d6,
            Squares.d7,
            Squares.b4,
            Squares.c4,
            Squares.e4,
            Squares.f4,
            Squares.g4
        ];

        ulong attackMask = RookAttacks.AttackMasks[attackFrom];
        ulong expected = attacks.ToBitboard();

        Assert.That(attackMask, Is.EqualTo(expected));
    }

    [Test]
    public void From_D1()
    {
        int attackFrom = Squares.d1;

        int[] attacks =
        [
            Squares.d2,
            Squares.d3,
            Squares.d4,
            Squares.d5,
            Squares.d6,
            Squares.d7,
            Squares.b1,
            Squares.c1,
            Squares.e1,
            Squares.f1,
            Squares.g1
        ];

        ulong attackMask = RookAttacks.AttackMasks[attackFrom];
        ulong expected = attacks.ToBitboard();

        Assert.That(attackMask, Is.EqualTo(expected));
    }
}
