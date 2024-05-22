using BenchmarkDotNet.Attributes;
using ChessRealms.ChessEngine2.Core.Attacks;
using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.MoveGeneration;
using ChessRealms.ChessEngine2.Core.Types;
using System.Runtime.InteropServices;

namespace ChessRealms.ChessEngine2.Benchmark.MoveGenerations;

public unsafe class KingMoveGenerationBenchmarks : MoveGenerationBenckmarksBase
{
    public KingMoveGenerationBenchmarks() : base()
    {
    }

    [Benchmark]
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
    public void King_WriteLeapingMovesToUnsafePtr()
    {
        fixed (Position* posPtr = &position)
        fixed (int* movesPtr = movesArr)
        {
            LeapingMovement.WriteMovesToPtrUnsafe(
                posPtr, 
                Colors.Black,
                Pieces.King,
                KingAttacks.AttackMasksUnsafe, 
                movesPtr);
        }
    }

#if LEGACY_FUNC
    private static readonly ChessEngine.Core.Types.Piece bKing = new(
        type: Pieces.King,
        color: Colors.Black);

    [Benchmark]
    public void King_WriteMovesToSpanArray_LEGACY()
    {
        position_Legacy.AddLeapingMoves(
            movesArr_Legacy,
            ChessEngine.Core.Attacks.KingAttacks.AttackMasks,
            bKing,
            0);
    }

    [Benchmark]
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
