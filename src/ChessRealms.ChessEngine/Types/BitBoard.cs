namespace ChessRealms.ChessEngine.Types;

public struct BitBoard(ulong value)
{
    public ulong Value { get; private set; } = value;

    public BitBoard() : this(0UL) {}

    public void SetBitAt(SquareIndex index)
    {
        Value |= index.BitBoard;
    }

    public void SetBitsAt(IEnumerable<SquareIndex> indicies)
    {
        Value = indicies.Select(x => x.BitBoard).Aggregate((b1, b2) => b1 | b2);
    }

    public static implicit operator BitBoard(ulong value) => new(value);

    public static implicit operator ulong(BitBoard board) => board.Value;
}
