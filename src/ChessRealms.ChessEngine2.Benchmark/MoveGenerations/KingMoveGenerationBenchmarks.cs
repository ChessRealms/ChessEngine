using BenchmarkDotNet.Attributes;
using ChessRealms.ChessEngine2.Core.Attacks;
using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.MoveGeneration;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ChessRealms.ChessEngine2.Benchmark.MoveGenerations;

public unsafe class KingMoveGenerationBenchmarks : MoveGenerationBenckmarksBase
{
    public KingMoveGenerationBenchmarks() : base()
    {
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void King_WriteLeapingMovesToSpanArray()
    {
        LeapingMovement.WriteMovesToSpan(
            ref position, 
            Colors.Black,
            Pieces.King,
            KingAttacks.AttackMasks, 
            movesArr);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void King_WriteLeapingMovesToSpanLing()
    {
        LeapingMovement.WriteMovesToSpan(
            ref position, 
            Colors.Black,
            Pieces.King,
            KingAttacks.AttackMasks, 
            CollectionsMarshal.AsSpan(moveList));
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void King_WriteLeapingMovesToUnsafePtr()
    {
        LeapingMovement.WriteMovesToPtrUnsafe(
            positionPtr, 
            Colors.Black,
            Pieces.King,
            KingAttacks.AttackMasks, 
            movesArrPtr);
    }

#if LEGACY_FUNC
    private static readonly ChessEngine.Core.Types.Piece bKing = new(
        type: Pieces.King,
        color: Colors.Black);

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void King_WriteMovesToSpanArray_LEGACY()
    {
        position_Legacy.AddLeapingMoves(
            movesArr_Legacy,
            ChessEngine.Core.Attacks.KingAttacks.AttackMasks,
            bKing,
            0);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void King_WriteMovesToSpanList_LEGACY()
    {
        position_Legacy.AddLeapingMoves(
            CollectionsMarshal.AsSpan(movesList_Legacy),
            ChessEngine.Core.Attacks.KingAttacks.AttackMasks,
            bKing,
            offset: 0);
    }
#endif
}
