using ChessRealms.ChessEngine.Core.Constants;
using ChessRealms.ChessEngine.Core.Math;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ChessRealms.ChessEngine.Parsing;

public static class AlgebraicNotation
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="moveSpan"></param>
    /// <returns></returns>
    public static AlgebraicMove ParseAlgebraicMove(ReadOnlySpan<char> moveSpan)
    {
        Debug.Assert(moveSpan.Length > 3);

        int src = ParseSquare(moveSpan[..2]);
        int trg = ParseSquare(moveSpan[2..]);

        return new(src, trg);
    }

    /// <summary>
    /// Try parse move.
    /// </summary>
    /// <param name="moveSpan"> Move as text. Example: <c>"a2a4"</c>. </param>
    /// <param name="move"> Output move. </param>
    /// <returns> 
    /// <see langword="true"/> if text was successfully parsed; otherwise, <see langword="false"/>
    /// </returns>
    public static bool TryParseAlgebraicMove(ReadOnlySpan<char> moveSpan, out AlgebraicMove move)
    {
        bool validate = moveSpan.Length > 3
            && SquareOps.ValidateFile(moveSpan[0])
            && SquareOps.ValidateRank(moveSpan[1])
            && SquareOps.ValidateFile(moveSpan[2])
            && SquareOps.ValidateRank(moveSpan[3]);
        
        if (validate)
        {
            move = new AlgebraicMove(
                src: ParseSquare(moveSpan[..2]),
                trg: ParseSquare(moveSpan[2..]));
            return true;
        }

        move = AlgebraicMove.Empty;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ParseSquare(ReadOnlySpan<char> fileRankSpan)
    {
        Debug.Assert(fileRankSpan.Length > 1);
        Debug.Assert(SquareOps.ValidateFile(fileRankSpan[0]));
        Debug.Assert(SquareOps.ValidateRank(fileRankSpan[1]));

        return SquareOps.FromFileRank(fileRankSpan[0] - 'a', fileRankSpan[1] - '1');
    }

    public static bool TryParseSquare(ReadOnlySpan<char> fileRankSpan, out int square)
    {
        bool validate = fileRankSpan.Length > 1
            && SquareOps.ValidateFile(fileRankSpan[0])
            && SquareOps.ValidateRank(fileRankSpan[1]);

        if (validate)
        {
            square = ParseSquare(fileRankSpan);
            return true;
        }

        square = Squares.Empty;
        return false;
    }
}
