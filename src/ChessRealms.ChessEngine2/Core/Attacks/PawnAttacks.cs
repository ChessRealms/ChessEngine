using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.Math;
using System.Collections.Immutable;

namespace ChessRealms.ChessEngine2.Core.Attacks;

internal static class PawnAttacks
{
    public static readonly ImmutableArray<ImmutableArray<ulong>> AttackMasks;

    static PawnAttacks()
    {
        ulong[] white = new ulong[64];
        ulong[] black = new ulong[64];

        for (int square = 0; square < 64; ++square)
        {
            white[square] = MaskPawnAttack(Colors.White, square);
            black[square] = MaskPawnAttack(Colors.Black, square);
        }

        ImmutableArray<ulong>[] masks = [[.. black], [.. white]];

        AttackMasks = [.. masks];
    }

    public static ulong MaskPawnAttack(int color, int square)
    {
        ulong board = SquareOps.ToBitboard(square);
        ulong attacks = 0UL;

        if (color == Colors.White)
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
