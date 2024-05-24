using BenchmarkDotNet.Attributes;
using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.MoveGeneration;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ChessRealms.ChessEngine2.Benchmark.MoveGenerations;

public unsafe class PawnMoveGenerationBenchmarks : MoveGenerationBenckmarksBase
{
    public PawnMoveGenerationBenchmarks() : base()
    {
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Pawn_WriteMovesToSpanArray()
    {
        PawnMovement.WriteMovesToSpan(ref position, Colors.Black, movesArr);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Pawn_WriteMovesToUnsafePtr()
    { 
        PawnMovement.WriteMovesToPtrUnsafe(positionPtr, Colors.Black, movesArrPtr);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Pawn_WriteMovesToSpanList()
    {         
        PawnMovement.WriteMovesToSpan(ref position, Colors.Black, CollectionsMarshal.AsSpan(moveList));
    }

#if LEGACY_FUNC
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Pawn_WriteMovesToSpanArray_LEGACY()
    {
        position_Legacy.AddPawnMoves(movesArr_Legacy, Colors.Black, offset: 0);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Pawn_WriteMovesToSpanList_LEGACY()
    {
        position_Legacy.AddPawnMoves(CollectionsMarshal.AsSpan(movesList_Legacy), Colors.Black, offset: 0);
    }
#endif
}
