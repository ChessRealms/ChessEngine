using BenchmarkDotNet.Attributes;
using ChessRealms.ChessEngine2.Core.Attacks;
using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.MoveGeneration;
using ChessRealms.ChessEngine2.Core.Types;
using System.Runtime.InteropServices;

namespace ChessRealms.ChessEngine2.Benchmark.MoveGenerations;

public unsafe class RookMoveGenerationBenchmarks : MoveGenerationBenckmarksBase
{
    public RookMoveGenerationBenchmarks() : base()
    {
    }

    [Benchmark]
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
    public void Rook_WriteSlidingMovesToUnsafePtr()
    {
        fixed (Position* posPtr = &position)
        fixed (int* movesPtr = movesArr)
        {
            SlidingMovement.WriteMovesToPtrUnsafe(
                posPtr,
                Colors.Black,
                Pieces.Rook,
                RookAttacks.GetSliderAttack,
                movesPtr);
        }
    }

    [Benchmark]
    public void Rook_WriteRookMovesToSpanArray()
    {
        RookMovement.WriteMovesToSpan(
            ref position,
            Colors.Black,
            movesArr);
    }

    [Benchmark]
    public void Rook_WriteRookMovesToSpanList()
    {
        RookMovement.WriteMovesToSpan(
            ref position,
            Colors.Black,
            CollectionsMarshal.AsSpan(moveList));
    }

    [Benchmark]
    public void Rook_WriteRookMovesToUnsafePtr()
    {
        fixed (Position* posPtr = &position)
        fixed (int* movesPtr = movesArr)
        {
            RookMovement.WriteMovesToPtrUnsafe(
                posPtr,
                Colors.Black,
                movesPtr);
        }
    }

#if LEGACY_FUNC
    private static readonly ChessEngine.Core.Types.Piece bRook = new(
        type: Pieces.Rook, 
        color: Colors.Black);

    [Benchmark]
    public void Rook_WriteSlidingMovesToSpanArray_LEGACY()
    {
        position_Legacy.AddSlidingMoves(
            movesArr_Legacy,
            ChessEngine.Core.Attacks.RookAttacks.GetSliderAttack,
            bRook,
            offset: 0);
    }

    [Benchmark]
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
