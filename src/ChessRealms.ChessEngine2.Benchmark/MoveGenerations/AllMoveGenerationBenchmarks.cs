using BenchmarkDotNet.Attributes;
using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.Movements;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ChessRealms.ChessEngine2.Benchmark.MoveGenerations;

public unsafe class AllMoveGenerationBenchmarks : MoveGenerationBenckmarksBase
{
    public AllMoveGenerationBenchmarks() : base()
    {
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void ALL_1_WriteMovesToSpanArray()
    {
        MoveGen.WriteMovesToSpan_v1(
            ref position,
            Colors.Black,
            movesArr);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void ALL_1_WriteMovesToUnsafePtr()
    {
        MoveGen.WriteMovesToUnsafePtr_v1(positionPtr, Colors.Black, movesArrPtr);
    }

#if LEGACY_FUNC

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void ALL_WriteMovesToSpanArray_LEGACY()
    {
        position_Legacy.GetMoves(
            movesArr_Legacy, 
            ChessEngine.Core.Types.Enums.PieceColor.Black);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void ALL_WriteMovesToSpanList_LEGACY()
    {
        position_Legacy.GetMoves(
            CollectionsMarshal.AsSpan(movesList_Legacy),
            ChessEngine.Core.Types.Enums.PieceColor.Black);
    }
#endif
}
