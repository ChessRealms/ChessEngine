﻿namespace ChessRealms.ChessEngine2.Core.Attacks;

public static class AttackLookups
{
    public static void InvokeInit()
    {
        PawnAttacks.InvokeInit();
        KnightAttacks.InvokeInit();
        BishopAttacks.InvokeInit();
        RookAttacks.InvokeInit();
        KingAttacks.InvokeInit();
    }
}