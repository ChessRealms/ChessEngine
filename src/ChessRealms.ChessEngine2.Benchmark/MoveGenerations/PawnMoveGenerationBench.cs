using BenchmarkDotNet.Attributes;
using ChessRealms.ChessEngine2.Core.Attacks;
using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.MoveGeneration;
using ChessRealms.ChessEngine2.Core.Types;
using ChessRealms.ChessEngine2.Parsing;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ChessRealms.ChessEngine2.Benchmark.MoveGenerations;

public unsafe class PawnMoveGenerationBench
{
    private readonly int[] moves1;
    private readonly int[] moves2;
    private readonly List<int> moveList;

    private Position position1;
    private Position position2;
    private Position position3;

    public PawnMoveGenerationBench()
    {
        PawnAttacks.InvokeInit();
        moves1 = new int[100];
        moves2 = new int[100];
        moveList = Enumerable.Range(0, 100).ToList();

        const string fen = "rnbqkbnr/1pppp1p1/8/pP6/5pPp/4P3/P1PP1P1P/RNBQKBNR b KQkq g3 0 1";

        bool parsed1 = FenStrings.TryParse(fen, out position1);
        bool parsed2 = FenStrings.TryParse(fen, out position2);
        bool parsed3 = FenStrings.TryParse(fen, out position3);

        Debug.Assert(parsed1);
        Debug.Assert(parsed2);
        Debug.Assert(parsed3);


    }

    [Benchmark]
    public void WritePawnMovesToSpanArray()
    {
        PawnMovement.WriteMovesToSpan(ref position1, Colors.Black, moves1, offset: 0);
    }

    [Benchmark]
    public void WritePawnMovesToUnsafePtr()
    { 
        fixed (Position* posPtr = &position2)
        fixed (int* movesPtr = moves2)
        {
            PawnMovement.WriteMovesUnsafe(posPtr, Colors.Black, movesPtr, size: 100, offset: 0);
        }
    }

    [Benchmark]
    public void WritePawnMovesToSpanList()
    {         
        PawnMovement.WriteMovesToSpan(ref position3, Colors.Black, CollectionsMarshal.AsSpan(moveList), offset: 0);
    }
}
