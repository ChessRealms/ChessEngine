using ChessRealms.ChessEngine.Core.Constants;
using ChessRealms.ChessEngine.Core.Types;
using System.Collections.Immutable;

namespace ChessRealms.ChessEngine.Core.Attacks;

internal static class PawnAttacks
{
    internal static readonly ImmutableArray<ImmutableArray<ulong>> AttackMasks;

    static PawnAttacks()
    {
        ulong[] white = new ulong[64];
        ulong[] black = new ulong[64];

        for (int square = 0; square < 64; ++square)
        {
            white[square] = MaskPawnAttack(ChessConstants.COLOR_WHITE, square);
            black[square] = MaskPawnAttack(ChessConstants.COLOR_BLACK, square);
        }

        ImmutableArray<ulong>[] masks = [[.. black], [.. white]];

        AttackMasks = [.. masks];
    }

    private static ulong MaskPawnAttack(int color, SquareIndex square)
    {
        ulong board = square.Board;
        ulong attacks = 0UL;

        if (color == ChessConstants.COLOR_WHITE)
        {
            attacks |= board << 7 & SquareMapping.NOT_H_FILE;
            attacks |= board << 9 & SquareMapping.NOT_A_FILE;
        }
        else
        {
            attacks |= board >> 7 & SquareMapping.NOT_A_FILE;
            attacks |= board >> 9 & SquareMapping.NOT_H_FILE;
        }

        return attacks;
    }
}
