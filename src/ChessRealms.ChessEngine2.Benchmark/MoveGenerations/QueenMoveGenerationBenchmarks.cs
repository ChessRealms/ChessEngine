using BenchmarkDotNet.Attributes;
using ChessRealms.ChessEngine2.Core.Attacks;
using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.MoveGeneration;
using ChessRealms.ChessEngine2.Core.Types;
using System.Runtime.InteropServices;

namespace ChessRealms.ChessEngine2.Benchmark.MoveGenerations;

public unsafe class QueenMoveGenerationBenchmarks : MoveGenerationBenckmarksBase
{


    public QueenMoveGenerationBenchmarks() : base()
    {
    }

    [Benchmark]
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
    public void Queen_WriteSlidingMovesToUnsafePtr()
    {
        fixed (Position* posPtr = &position)
        fixed (int* movesPtr = movesArr)
        {
            SlidingMovement.WriteMovesToPtrUnsafe(
                posPtr,
                Colors.Black,
                Pieces.Queen,
                QueenAttacks.GetSliderAttack,
                movesPtr);
        }
    }

    [Benchmark]
    public void Queen_WriteQueenMovesToSpanArray()
    {
        QueenMovement.WriteMovesToSpan(
            ref position,
            Colors.Black,
            movesArr);
    }

    [Benchmark]
    public void Queen_WriteQueenMovesToSpanList()
    {
        QueenMovement.WriteMovesToSpan(
            ref position,
            Colors.Black,
            CollectionsMarshal.AsSpan(moveList));
    }

    [Benchmark]
    public void Queen_WriteQueenMovesToUnsafePtr()
    {
        fixed (Position* posPtr = &position)
        fixed (int* movesPtr = movesArr)
        {
            QueenMovement.WriteMovesToPtrUnsafe(
                posPtr,
                Colors.Black,
                movesPtr);
        }
    }

#if LEGACY_FUNC
    private static readonly ChessEngine.Core.Types.Piece bQueen = new(
        type: Pieces.Queen,
        color: Colors.Black);

    [Benchmark]
    public void Queen_WriteSlidingMovesToSpanArray_LEGACY()
    {
        position_Legacy.AddSlidingMoves(
            movesArr_Legacy,
            ChessEngine.Core.Attacks.QueenAttacks.GetSliderAttack,
            bQueen,
            offset: 0);
    }

    [Benchmark]
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
