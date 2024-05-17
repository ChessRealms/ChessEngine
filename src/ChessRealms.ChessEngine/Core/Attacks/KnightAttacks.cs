using ChessRealms.ChessEngine.Core.Constants;
using ChessRealms.ChessEngine.Core.Types;
using System.Collections.Immutable;

namespace ChessRealms.ChessEngine.Core.Attacks;

internal static class KnightAttacks
{
    internal static readonly ImmutableArray<ulong> AttackMasks;

    static KnightAttacks()
    {
        ulong[] masks = new ulong[64];

        for (int square = 0; square < 64; ++square)
        {
            masks[square] = MaskKnightAttack(square);
        }

        AttackMasks = [.. masks];
    }

    internal static ulong MaskKnightAttack(SquareIndex square)
    {
        ulong board = square.Board;
        ulong attacks = 0UL;

        attacks |= board >> 17 & SquareMapping.NOT_H_FILE;
        attacks |= board >> 15 & SquareMapping.NOT_A_FILE;
        attacks |= board >> 10 & SquareMapping.NOT_HG_FILE;
        attacks |= board >> 6 & SquareMapping.NOT_AB_FILE;

        attacks |= board << 17 & SquareMapping.NOT_A_FILE;
        attacks |= board << 15 & SquareMapping.NOT_H_FILE;
        attacks |= board << 10 & SquareMapping.NOT_AB_FILE;
        attacks |= board << 6 & SquareMapping.NOT_HG_FILE;

        return attacks;
    }
}
