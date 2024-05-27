using BenchmarkDotNet.Attributes;
using ChessRealms.ChessEngine2.Core.Attacks;
using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.Movements;
using ChessRealms.ChessEngine2.Core.Types;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ChessRealms.ChessEngine2.Benchmark.MoveGenerations;

public unsafe class RookMoveGenerationBenchmarks : MoveGenerationBenckmarksBase
{
    public RookMoveGenerationBenchmarks() : base()
    {
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Rook_WriteSlidingMovesToSpanArray()
    {
        SlidingMovement.WriteMovesToSpan(
            ref position,
            Colors.Black,
            Pieces.Rook,
            RookAttacks.GetSliderAttack,
            movesArr);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Rook_WriteSlidingMovesToSpanList()
    {
        SlidingMovement.WriteMovesToSpan(
            ref position,
            Colors.Black,
            Pieces.Rook,
            RookAttacks.GetSliderAttack,
            CollectionsMarshal.AsSpan(moveList));
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Rook_WriteSlidingMovesToUnsafePtr()
    {
        fixed (Position* posPtr = &position)
        fixed (int* movesPtr = movesArr)
        {
            SlidingMovement.WriteMovesToPtrUnsafe(
                posPtr,
                Colors.Black,
                Pieces.Rook,
                &RookAttacks.GetSliderAttack,
                movesPtr);
        }
    }

#if LEGACY_FUNC
    private static readonly ChessEngine.Core.Types.Piece bRook = new(
        type: Pieces.Rook, 
        color: Colors.Black);

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Rook_WriteSlidingMovesToSpanArray_LEGACY()
    {
        position_Legacy.AddSlidingMoves(
            movesArr_Legacy,
            ChessEngine.Core.Attacks.RookAttacks.GetSliderAttack,
            bRook,
            offset: 0);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Rook_WriteSlidingMovesToSpanList_LEGACY()
    {
        position_Legacy.AddSlidingMoves(
            CollectionsMarshal.AsSpan(movesList_Legacy),
            ChessEngine.Core.Attacks.RookAttacks.GetSliderAttack,
            bRook,
            offset: 0);
    }
#endif
}
