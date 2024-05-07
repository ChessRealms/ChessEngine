namespace ChessRealms.ChessEngine.Core.Types;

public record Piece
{
    public PieceType Type { get; init; }
    public PieceColor Color { get; init; }
}
