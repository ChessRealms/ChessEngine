using BenchmarkDotNet.Attributes;
using ChessRealms.ChessEngine2.Core.Attacks;
using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.MoveGeneration;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ChessRealms.ChessEngine2.Benchmark.MoveGenerations;

public unsafe class BishopMoveGenerationBenchmarks : MoveGenerationBenckmarksBase
{
    public BishopMoveGenerationBenchmarks() : base()
    {
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Bishop_WriteSlidingMovesToSpanArray()
    {
        SlidingMovement.WriteMovesToSpan(
            ref position,
            Colors.Black,
            Pieces.Bishop,
            BishopAttacks.GetSliderAttack,
            movesArr);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Bishop_WriteSlidingMovesToSpanList()
    {
        SlidingMovement.WriteMovesToSpan(
            ref position,
            Colors.Black,
            Pieces.Bishop,
            BishopAttacks.GetSliderAttack,
            CollectionsMarshal.AsSpan(moveList));
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Bishop_WriteSlidingMovesToUnsafePtr()
    {
        SlidingMovement.WriteMovesToPtrUnsafe(
            positionPtr,
            Colors.Black,
            Pieces.Bishop,
            BishopAttacks.GetSliderAttack,
            movesArrPtr);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Bishop_WriteBishopMovesToSpanArray()
    {
        BishopMovement.WriteMovesToSpan(
            ref position,
            Colors.Black,
            movesArr);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Bishop_WriteBishopMovesToSpanList()
    {
        BishopMovement.WriteMovesToSpan(
            ref position,
            Colors.Black,
            CollectionsMarshal.AsSpan(moveList));
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Bishop_WriteBishopMovesToUnsafePtr()
    {
        BishopMovement.WriteMovesToPtrUnsafe(
            positionPtr,
            Colors.Black,
            movesArrPtr);
    }

#if LEGACY_FUNC
    private static readonly ChessEngine.Core.Types.Piece bBishop = new(
        type: Pieces.Bishop,
        color: Colors.Black);

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Bishop_WriteSlidingMovesToSpanArray_LEGACY()
    {
        position_Legacy.AddSlidingMoves(
            movesArr_Legacy,
            ChessEngine.Core.Attacks.BishopAttacks.GetSliderAttack,
            bBishop,
            offset: 0);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Bishop_WriteSlidingMovesToSpanList_LEGACY()
    {
        position_Legacy.AddSlidingMoves(
            CollectionsMarshal.AsSpan(movesList_Legacy),
            ChessEngine.Core.Attacks.BishopAttacks.GetSliderAttack,
            bBishop,
            offset: 0);
    }
#endif
}
