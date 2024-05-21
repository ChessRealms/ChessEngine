using BenchmarkDotNet.Attributes;
using ChessRealms.ChessEngine2.Core.Attacks;
using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.MoveGeneration;
using ChessRealms.ChessEngine2.Core.Types;
using System.Runtime.InteropServices;

namespace ChessRealms.ChessEngine2.Benchmark.MoveGenerations;

public unsafe class MoveGenerationBenchmarks : MoveGenerationBenchBase
{
    public MoveGenerationBenchmarks() 
        : base("rnbqkbnr/1pppp1p1/8/pP6/5pPp/4P3/P1PP1P1P/RNBQKBNR b KQkq g3 0 1", 100)
    {
        PawnAttacks.InvokeInit();
    }

    [Benchmark]
    public void Pawn_WriteMovesToSpanArray()
    {
        PawnMovement.WriteMovesToSpan(ref position1, Colors.Black, moves1);
    }

    [Benchmark]
    public void Pawn_WriteMovesToUnsafePtr()
    { 
        fixed (Position* posPtr = &position2)
        fixed (int* movesPtr = moves2)
        {
            PawnMovement.WriteMovesUnsafe(posPtr, Colors.Black, movesPtr);
        }
    }

    [Benchmark]
    public void Pawn_WriteMovesToSpanList()
    {         
        PawnMovement.WriteMovesToSpan(ref position3, Colors.Black, CollectionsMarshal.AsSpan(moveList));
    }

    [Benchmark]
    public void Knight_WriteMovesToSpanArray()
    {
        KnightMovement.WriteMovesToSpan(ref position1, Colors.Black, moves1);
    }

    [Benchmark]
    public void Knight_WriteMovesToUnsafePtr()
    { 
        fixed (Position* posPtr = &position2)
        fixed (int* movesPtr = moves2)
        {
            KnightMovement.WriteMovesUnsafe(posPtr, Colors.Black, movesPtr);
        }
    }

    [Benchmark]
    public void Knight_WriteMovesToSpanList()
    {         
        KnightMovement.WriteMovesToSpan(ref position3, Colors.Black, CollectionsMarshal.AsSpan(moveList));
    }
}
