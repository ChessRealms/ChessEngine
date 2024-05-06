using ChessRealms.ChessEngine.Core.Types;
using System.Collections.Immutable;

namespace ChessRealms.ChessEngine.Core.Attacks;

public static class PawnLookups
{
    public static readonly ImmutableDictionary<PieceColor, ImmutableArray<ulong>> AttackMasks;

    static PawnLookups()
    {
        ulong[] white = new ulong[64];
        ulong[] black = new ulong[64];

        for (int square = 0; square < 64; ++square)
        {
            white[square] = MaskPawnAttack(PieceColor.White, square);
            black[square] = MaskPawnAttack(PieceColor.Black, square);
        }

        Dictionary<PieceColor, ImmutableArray<ulong>> masks = new()
        {
            { PieceColor.White, white.ToImmutableArray() },
            { PieceColor.Black, black.ToImmutableArray() },
        };

        AttackMasks = masks.ToImmutableDictionary();
    }

    private static ulong MaskPawnAttack(PieceColor color, SquareIndex square)
    {
        ulong board = square.BitBoard;
        ulong attacks = 0UL;

        if (color == PieceColor.White)
        {
            attacks |= board << 7 & LerfConstants.NOT_H_FILE;
            attacks |= board << 9 & LerfConstants.NOT_A_FILE;
        }
        else
        {
            attacks |= board >> 7 & LerfConstants.NOT_A_FILE;
            attacks |= board >> 9 & LerfConstants.NOT_H_FILE;
        }

        return attacks;
    }
}
