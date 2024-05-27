using BenchmarkDotNet.Attributes;
using ChessRealms.ChessEngine2.Core.Attacks;
using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.Movements;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ChessRealms.ChessEngine2.Benchmark.MoveGenerations;

public unsafe class KnightMoveGenerationBenckmarks : MoveGenerationBenckmarksBase
{
    public KnightMoveGenerationBenckmarks() : base()
    {
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Knight_WriteLeapingMovesToSpanArray()
    {
        LeapingMovement.WriteMovesToSpan(
            ref position, 
            Colors.Black,
            Pieces.Knight,
            KnightAttacks.AttackMasks,
            movesArr);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Knight_WriteLeapingMovesToSpanList()
    {         
        LeapingMovement.WriteMovesToSpan(
            ref position, 
            Colors.Black,
            Pieces.Knight,
            KnightAttacks.AttackMasks, 
            CollectionsMarshal.AsSpan(moveList));
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Knight_WriteLeapingMovesToUnsafePtr()
    { 
        LeapingMovement.WriteMovesToPtrUnsafe(
            positionPtr, 
            Colors.Black,
            Pieces.Knight,
            KnightAttacks.AttackMasks, 
            movesArrPtr);
    }

#if LEGACY_FUNC
    private static readonly ChessEngine.Core.Types.Piece bKnight = new(
            type: Pieces.Knight,
            color: Colors.Black);

    [Benchmark]
    public void Knight_WriteLeapingMovesToSpanArray_LEGACY()
    {
        position_Legacy.AddLeapingMoves(
            movesArr_Legacy,
            ChessEngine.Core.Attacks.KnightAttacks.AttackMasks,
            bKnight,
            offset: 0);
    }

    [Benchmark]
    public void Knight_WriteLeapingMovesToSpanList_LEGACY()
    {
        position_Legacy.AddLeapingMoves(
            CollectionsMarshal.AsSpan(movesList_Legacy),
            ChessEngine.Core.Attacks.KnightAttacks.AttackMasks,
            bKnight,
            offset: 0);
    }
#endif
}
