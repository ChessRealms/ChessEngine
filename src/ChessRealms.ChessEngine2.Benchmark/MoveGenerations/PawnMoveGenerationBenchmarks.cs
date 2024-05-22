using BenchmarkDotNet.Attributes;
using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.MoveGeneration;
using ChessRealms.ChessEngine2.Core.Types;
using System.Runtime.InteropServices;

namespace ChessRealms.ChessEngine2.Benchmark.MoveGenerations;

public unsafe class PawnMoveGenerationBenchmarks : MoveGenerationBenckmarksBase
{
    public PawnMoveGenerationBenchmarks() : base()
    {
    }

    [Benchmark]
    public void Pawn_WriteMovesToSpanArray()
    {
        PawnMovement.WriteMovesToSpan(ref position, Colors.Black, movesArr);
    }

    [Benchmark]
    public void Pawn_WriteMovesToUnsafePtr()
    { 
        fixed (Position* posPtr = &position)
        fixed (int* movesPtr = movesArr)
        {
            PawnMovement.WriteMovesToPtrUnsafe(posPtr, Colors.Black, movesPtr);
        }
    }

    [Benchmark]
    public void Pawn_WriteMovesToSpanList()
    {         
        PawnMovement.WriteMovesToSpan(ref position, Colors.Black, CollectionsMarshal.AsSpan(moveList));
    }

#if LEGACY_FUNC
    [Benchmark]
    public void Pawn_WriteMovesToSpanArray_LEGACY()
    {
        position_Legacy.AddPawnMoves(movesArr_Legacy, 0, 0);
    }

    [Benchmark]
    public void Pawn_WriteMovesToSpanList_LEGACY()
    {
        position_Legacy.AddPawnMoves(CollectionsMarshal.AsSpan(movesList_Legacy), 0, 0);
    }
#endif
}
