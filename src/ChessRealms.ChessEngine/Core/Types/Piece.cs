namespace ChessRealms.ChessEngine.Core.Types;

public readonly struct Piece(PieceType type, PieceColor color)
{
    public readonly PieceType Type = type;
    public readonly PieceColor Color = color;

    public static readonly Piece Empty = new(PieceType.None, PieceColor.None);
}
