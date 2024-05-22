using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.Math;
using System.Collections.Immutable;

namespace ChessRealms.ChessEngine2.Core.Attacks;

internal unsafe static class KingAttacks
{
    private static readonly ulong[] _attackMasks;
    public static readonly ulong* AttackMasksUnsafe;
    public static readonly ImmutableArray<ulong> AttackMasks;

    static KingAttacks()
    {
        _attackMasks = new ulong[64];

        for (int square = 0; square < 64; ++square)
        {
            _attackMasks[square] = MaskKingAttack(square);
        }

        fixed (ulong* ptr = _attackMasks)
            AttackMasksUnsafe = ptr;

        AttackMasks = [.. _attackMasks];
    }

    public static void InvokeInit()
    {
        _ = _attackMasks[0];
    }

    public static ulong MaskKingAttack(int square)
    {
        ulong board = SquareOps.ToBitboard(square);
        ulong attacks = 0UL;

        attacks |= board << 8;
        attacks |= board >> 8;

        attacks |= board << 9 & SquareMapping.NOT_A_FILE;
        attacks |= board << 1 & SquareMapping.NOT_A_FILE;
        attacks |= board >> 7 & SquareMapping.NOT_A_FILE;

        attacks |= board >> 9 & SquareMapping.NOT_H_FILE;
        attacks |= board >> 1 & SquareMapping.NOT_H_FILE;
        attacks |= board << 7 & SquareMapping.NOT_H_FILE;

        return attacks;
    }
}
