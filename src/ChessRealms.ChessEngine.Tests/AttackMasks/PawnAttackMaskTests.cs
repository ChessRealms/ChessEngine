﻿using ChessRealms.ChessEngine.AttackMasks;
using ChessRealms.ChessEngine.Types;
using ChessRealms.ChessEngine.Types.Enums;

namespace ChessRealms.Tests.AttackMasks;

public class PawnAttackMaskTests
{
    [Test]
    public void MaskAttackFromA4_AsWhitePawn()
    {
        SquareIndex attackFrom = EnumSquare.a4;
        SquareIndex attack = EnumSquare.b5;
       
        ulong attackMask = PawnAttackMask.Instance[PieceColor.White][attackFrom];
        ulong xor = attackMask ^ attack.BitBoard.Value;

        Assert.That(xor, Is.EqualTo(0));
    }

    [Test]
    public void MaskAttackFromA4_AsBlackPawn()
    {
        SquareIndex attackFrom = EnumSquare.a4;
        SquareIndex attack = EnumSquare.b3;
        
        ulong attackMask = PawnAttackMask.Instance[PieceColor.Black][attackFrom];
        ulong xor = attackMask ^ attack.BitBoard.Value;

        Assert.That(xor, Is.EqualTo(0));
    }

    [Test]
    public void MaskAttackFromH4_AsWhitePawn()
    {
        SquareIndex attackFrom = EnumSquare.h4;
        SquareIndex attack = EnumSquare.g5;
       
        ulong attackMask = PawnAttackMask.Instance[PieceColor.White][attackFrom];
        ulong xor = attackMask ^ attack.BitBoard.Value;

        Assert.That(xor, Is.EqualTo(0));
    }

    [Test]
    public void MaskAttackFromH4_AsBlackPawn()
    {
        SquareIndex attackFrom = EnumSquare.h4;
        SquareIndex attack = EnumSquare.g3;
        
        ulong attackMask = PawnAttackMask.Instance[PieceColor.Black][attackFrom];
        ulong xor = attackMask ^ attack.BitBoard.Value;

        Assert.That(xor, Is.EqualTo(0));
    }

    [Test]
    public void MaskAttackFromE5_AsWhitePawn()
    {
        SquareIndex attackFrom = EnumSquare.e5;
        SquareIndex attack1 = EnumSquare.d6;
        SquareIndex attack2 = EnumSquare.f6;
        
        ulong attackMask = PawnAttackMask.Instance[PieceColor.White][attackFrom];
        ulong xor = attackMask ^ attack1.BitBoard.Value ^ attack2.BitBoard.Value;

        Assert.That(xor, Is.EqualTo(0));
    }

    [Test]
    public void MaskAttackFromE5_AsBlackPawn()
    {
        SquareIndex attackFrom = EnumSquare.e5;
        SquareIndex attack1 = EnumSquare.d4;
        SquareIndex attack2 = EnumSquare.f4;
        
        ulong attackMask = PawnAttackMask.Instance[PieceColor.Black][attackFrom];
        ulong xor = attackMask ^ attack1.BitBoard.Value ^ attack2.BitBoard.Value;

        Assert.That(xor, Is.EqualTo(0));
    }
}