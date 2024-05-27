using BenchmarkDotNet.Attributes;
using ChessRealms.ChessEngine;
using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.Movements;
using ChessRealms.ChessEngine2.Core.Types;
using ChessRealms.ChessEngine2.Parsing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ChessRealms.ChessEngine2.Benchmark.MoveGenerations;

public unsafe class CastlingMoveGenerationBenchmarks : MoveGenerationBenckmarksBase
{
    private const string StartFenNoCastling = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w - - 0 1";
    private Position noCastlingPosition;
#if LEGACY_FUNC
    private ChessBoard noCastlingPositionLegacy;
#endif

    public CastlingMoveGenerationBenchmarks() : base()
    {
        _ = FenStrings.TryParse(StartFenNoCastling, out noCastlingPosition);
#if LEGACY_FUNC
        _ = ChessEngine.Parsing.FenStrings.TryParse(StartFenNoCastling, ref noCastlingPositionLegacy);
#endif
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Castling_WriteMovesToSpanArray()
    {
        CastlingMovement.WriteMovesToSpan(ref position, Colors.Black, movesArr);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Castling_WriteMovesToSpanList()
    {
        CastlingMovement.WriteMovesToSpan(ref position, Colors.Black, CollectionsMarshal.AsSpan(moveList));
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Castling_WriteMovesToUnsafePtr()
    {
        fixed (Position* posPtr = &position)
        fixed (int* movesPtr = movesArr)
        {
            CastlingMovement.WriteMovesToUnsafePtr(posPtr, Colors.Black, movesPtr);
        }
    }

#if LEGACY_FUNC
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Castling_WriteMovesToSpanArray_LEGACY()
    {
        position_Legacy.AddCastlingMoves(movesArr_Legacy, Colors.Black, offset: 0);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Castling_WriteMovesToSpanList_LEGACY()
    {
        position_Legacy.AddCastlingMoves(CollectionsMarshal.AsSpan(movesList_Legacy), Colors.Black, offset: 0);
    }
#endif
}
