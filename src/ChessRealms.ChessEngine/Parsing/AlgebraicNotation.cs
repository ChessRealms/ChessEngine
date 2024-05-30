using ChessRealms.ChessEngine.Core.Constants;
using ChessRealms.ChessEngine.Core.Math;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ChessRealms.ChessEngine.Parsing;

public static class AlgebraicNotation
{
    public static (int src, int trg) ParseMove(ReadOnlySpan<char> moveSpan)
    {
        Debug.Assert(moveSpan.Length > 3);

        int src = ParseSquare(moveSpan[..2]);
        int trg = ParseSquare(moveSpan[2..]);

        return (src, trg);
    }

    /// <summary>
    /// Try parse move.
    /// </summary>
    /// <param name="moveSpan"> Move as text. Example: <c>"a2a4"</c>. </param>
    /// <param name="move"> Output move. </param>
    /// <returns> 
    /// <see langword="true"/> if text was successfully parsed; otherwise, <see langword="false"/>
    /// </returns>
    public static bool TryParseMove(ReadOnlySpan<char> moveSpan, out (int src, int trg) move)
    {
        bool validate = moveSpan.Length > 3
            && SquareOps.ValidateFile(moveSpan[0])
            && SquareOps.ValidateRank(moveSpan[1])
            && SquareOps.ValidateFile(moveSpan[2])
            && SquareOps.ValidateRank(moveSpan[3]);
        
        if (validate)
        {
            move.src = ParseSquare(moveSpan[..2]);
            move.trg = ParseSquare(moveSpan[2..]);
            return true;
        }

        move.src = Squares.Empty;
        move.trg = Squares.Empty;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ParseSquare(ReadOnlySpan<char> fileRankSpan)
    {
        Debug.Assert(fileRankSpan.Length > 1);
        return SquareOps.FromFileRank(fileRankSpan[0] - 'a', fileRankSpan[1] - '1');
    }
}
