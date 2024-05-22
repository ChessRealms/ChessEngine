using BenchmarkDotNet.Attributes;
using ChessRealms.ChessEngine2.Core.Attacks;
using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.MoveGeneration;
using ChessRealms.ChessEngine2.Core.Types;
using System.Runtime.InteropServices;

namespace ChessRealms.ChessEngine2.Benchmark.MoveGenerations;

public unsafe class BishopMoveGenerationBenchmarks : MoveGenerationBenckmarksBase
{
    public BishopMoveGenerationBenchmarks() : base()
    {
    }

    [Benchmark]
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
    public void Bishop_WriteSlidingMovesToUnsafePtr()
    {
        fixed (Position* posPtr = &position)
        fixed (int* movesPtr = movesArr)
        {
            SlidingMovement.WriteMovesToPtrUnsafe(
                posPtr,
                Colors.Black,
                Pieces.Bishop,
                BishopAttacks.GetSliderAttack,
                movesPtr);
        }
    }

    [Benchmark]
    public void Bishop_WriteBishopMovesToSpanArray()
    {
        BishopMovement.WriteMovesToSpan(
            ref position,
            Colors.Black,
            movesArr);
    }

    [Benchmark]
    public void Bishop_WriteBishopMovesToSpanList()
    {
        BishopMovement.WriteMovesToSpan(
            ref position,
            Colors.Black,
            CollectionsMarshal.AsSpan(moveList));
    }

    [Benchmark]
    public void Bishop_WriteBishopMovesToUnsafePtr()
    {
        fixed (Position* posPtr = &position)
        fixed (int* movesPtr = movesArr)
        {
            BishopMovement.WriteMovesToPtrUnsafe(
                posPtr,
                Colors.Black,
                movesPtr);
        }
    }

#if LEGACY_FUNC
    private static readonly ChessEngine.Core.Types.Piece bBishop = new(
        type: Pieces.Bishop,
        color: Colors.Black);

    [Benchmark]
    public void Bishop_WriteSlidingMovesToSpanArray_LEGACY()
    {
        position_Legacy.AddSlidingMoves(
            movesArr_Legacy,
            ChessEngine.Core.Attacks.BishopAttacks.GetSliderAttack,
            bBishop,
            offset: 0);
    }

    [Benchmark]
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
