using ChessRealms.ChessEngine2.Core.Types;
using ChessRealms.ChessEngine2.Parsing;
using System.Diagnostics;

namespace ChessRealms.ChessEngine2.Benchmark.MoveGenerations;

public abstract class MoveGenerationBenchBase
{
    protected readonly int[] moves1;
    protected readonly int[] moves2;
    protected readonly List<int> moveList;

    protected Position position1;
    protected Position position2;
    protected Position position3;

    protected MoveGenerationBenchBase(string fen, int moveBuffersSize)
    {
        Debug.Assert(moveBuffersSize > 0);
        
        moves1 = new int[moveBuffersSize];
        moves2 = new int[moveBuffersSize];
        moveList = Enumerable.Range(0, moveBuffersSize).ToList();

        bool parsed1 = FenStrings.TryParse(fen, out position1);
        bool parsed2 = FenStrings.TryParse(fen, out position2);
        bool parsed3 = FenStrings.TryParse(fen, out position3);

        Debug.Assert(parsed1);
        Debug.Assert(parsed2);
        Debug.Assert(parsed3);
    }
}
