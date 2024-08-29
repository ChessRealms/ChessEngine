using ChessRealms.ChessEngine.Core.Constants;
using ChessRealms.ChessEngine.Parsing;

namespace ChessRealms.ChessEngine;

public readonly struct AlgebraicMove(Square src, Square trg)
{
    public readonly Square Src = src;
    public readonly Square Trg = trg;

    public static readonly AlgebraicMove Empty = new(Squares.Empty, Squares.Empty);

    public bool IsValid()
    {
        return Squares.IsValid(Src) && Squares.IsValid(Trg);
    }

    public static AlgebraicMove Parse(ReadOnlySpan<char> span)
    {
        return AlgebraicNotation.ParseAlgebraicMove(span);
    }

    public static bool TryParse(ReadOnlySpan<char> span, out AlgebraicMove move)
    {
        return AlgebraicNotation.TryParseAlgebraicMove(span, out move);
    }
}
