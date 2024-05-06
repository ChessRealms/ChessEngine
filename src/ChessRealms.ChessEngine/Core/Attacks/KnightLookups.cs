﻿using ChessRealms.ChessEngine.Core.Types;
using System.Collections.Immutable;

namespace ChessRealms.ChessEngine.Core.Attacks;

public static class KnightLookups
{
    public static readonly ImmutableArray<ulong> AttackMasks;

    static KnightLookups()
    {
        ulong[] masks = new ulong[64];

        for (int square = 0; square < 64; ++square)
        {
            masks[square] = MaskKnightAttack(square);
        }

        AttackMasks = [.. masks];
    }

    private static ulong MaskKnightAttack(SquareIndex square)
    {
        ulong board = square.BitBoard;
        ulong attacks = 0UL;

        attacks |= board >> 17 & LerfConstants.NOT_H_FILE;
        attacks |= board >> 15 & LerfConstants.NOT_A_FILE;
        attacks |= board >> 10 & LerfConstants.NOT_HG_FILE;
        attacks |= board >> 6 & LerfConstants.NOT_AB_FILE;

        attacks |= board << 17 & LerfConstants.NOT_A_FILE;
        attacks |= board << 15 & LerfConstants.NOT_H_FILE;
        attacks |= board << 10 & LerfConstants.NOT_AB_FILE;
        attacks |= board << 6 & LerfConstants.NOT_HG_FILE;

        return attacks;
    }
}