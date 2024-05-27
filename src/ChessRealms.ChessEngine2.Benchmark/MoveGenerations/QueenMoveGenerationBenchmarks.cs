using BenchmarkDotNet.Attributes;
using ChessRealms.ChessEngine2.Core.Attacks;
using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.Movements;
using ChessRealms.ChessEngine2.Core.Types;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ChessRealms.ChessEngine2.Benchmark.MoveGenerations;

public unsafe class QueenMoveGenerationBenchmarks : MoveGenerationBenckmarksBase
{
    public QueenMoveGenerationBenchmarks() : base()
    {
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Queen_WriteSlidingMovesToSpanArray()
    {
        SlidingMovement.WriteMovesToSpan(
            ref position,
            Colors.Black,
            Pieces.Queen,
            QueenAttacks.GetSliderAttack,
            movesArr);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Queen_WriteSlidingMovesToSpanList()
    {
        SlidingMovement.WriteMovesToSpan(
            ref position,
            Colors.Black,
            Pieces.Queen,
            QueenAttacks.GetSliderAttack,
            CollectionsMarshal.AsSpan(moveList));
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Queen_WriteSlidingMovesToUnsafePtr()
    {
        fixed (Position* posPtr = &position)
        fixed (int* movesPtr = movesArr)
        {
            SlidingMovement.WriteMovesToPtrUnsafe(
                posPtr,
                Colors.Black,
                Pieces.Queen,
                &QueenAttacks.GetSliderAttack,
                movesPtr);
        }
    }

#if LEGACY_FUNC
    private static readonly ChessEngine.Core.Types.Piece bQueen = new(
        type: Pieces.Queen,
        color: Colors.Black);

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Queen_WriteSlidingMovesToSpanArray_LEGACY()
    {
        position_Legacy.AddSlidingMoves(
            movesArr_Legacy,
            ChessEngine.Core.Attacks.QueenAttacks.GetSliderAttack,
            bQueen,
            offset: 0);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Queen_WriteSlidingMovesToSpanList_LEGACY()
    {
        position_Legacy.AddSlidingMoves(
            CollectionsMarshal.AsSpan(movesList_Legacy),
            ChessEngine.Core.Attacks.QueenAttacks.GetSliderAttack,
            bQueen,
            offset: 0);
    }
#endif
}
