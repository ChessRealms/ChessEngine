using static ChessRealms.ChessEngine.Core.Constants.ChessConstants;

namespace ChessRealms.ChessEngine.Core.Types;

public readonly struct Piece(int type, int color)
{
    public readonly int Type = type;

    public readonly int Color = color;

    public static readonly Piece Empty = new(PIECE_NONE, COLOR_NONE);
}
