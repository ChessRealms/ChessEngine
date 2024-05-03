namespace ChessRealms.ChessEngine.Types;

public struct BitBoard(ulong value)
{
    public ulong Value { get; private set; } = value;

    public static implicit operator BitBoard(ulong value) => new(value);

    public static implicit operator ulong(BitBoard board) => board.Value;
}
