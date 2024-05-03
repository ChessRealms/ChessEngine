using ChessRealms.ChessEngine.Types;
using ChessRealms.ChessEngine.Types.Enums;
using System.Collections.Immutable;

namespace ChessRealms.ChessEngine.AttackMasks;

public static class PawnAttackMask
{
    private const int BOARD_SIZE = 64;
    public static readonly ImmutableDictionary<PieceColor, ImmutableArray<ulong>> Instance;

    static PawnAttackMask()
    {
        ulong[] white = new ulong[BOARD_SIZE];
        ulong[] black = new ulong[BOARD_SIZE];

        for (int square = 0; square < BOARD_SIZE; ++square)
        {
            white[square] = MaskPawnAttack(PieceColor.White, square);
            black[square] = MaskPawnAttack(PieceColor.Black, square);
        }

        Dictionary<PieceColor, ImmutableArray<ulong>> masks = new()
        {
            { PieceColor.White, white.ToImmutableArray() },
            { PieceColor.Black, black.ToImmutableArray() },
        };

        Instance = masks.ToImmutableDictionary();
    }

    private static ulong MaskPawnAttack(PieceColor color, SquareIndex square)
    {
        ulong board = square.BitBoard;
        ulong attacks = 0UL;

        if (color == PieceColor.White)
        {
            attacks |= (board << 7) & LerfConstants.NOT_H_FILE;
            attacks |= (board << 9) & LerfConstants.NOT_A_FILE;
        }
        else
        {
            attacks |= (board >> 7) & LerfConstants.NOT_A_FILE;
            attacks |= (board >> 9) & LerfConstants.NOT_H_FILE;
        }

        return attacks;
    }
}
