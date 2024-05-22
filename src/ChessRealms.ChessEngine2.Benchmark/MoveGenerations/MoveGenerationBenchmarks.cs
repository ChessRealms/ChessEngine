using BenchmarkDotNet.Attributes;
using ChessRealms.ChessEngine;
using ChessRealms.ChessEngine.Core.Types;
using ChessRealms.ChessEngine2.Core.Attacks;
using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.MoveGeneration;
using ChessRealms.ChessEngine2.Core.Types;
using ChessRealms.ChessEngine2.Parsing;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ChessRealms.ChessEngine2.Benchmark.MoveGenerations;

public unsafe class MoveGenerationBenchmarks
{
    #region For current ChessEngine2
    private readonly int[] movesArr;
    private readonly List<int> moveList;

    private Position position;
    #endregion

    #region For legacy ChessEngine
    private ChessBoard position_Legacy = new();
    private readonly BinaryMove[] movesArr_Legacy;
    private readonly List<BinaryMove> movesList_Legacy;
    static readonly ChessEngine.Core.Types.Piece bKnight = new(1, 0);
    static readonly ChessEngine.Core.Types.Piece bBishop = new(2, 0);
    static readonly ChessEngine.Core.Types.Piece bRook = new(3, 0);
    static readonly ChessEngine.Core.Types.Piece bQueen = new(4, 0);
    static readonly ChessEngine.Core.Types.Piece bKing = new(5, 0);
    #endregion
    public MoveGenerationBenchmarks() 
    {
        const string fen = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 1 1";
        const int moveBuffersSize = 100;

        PawnAttacks.InvokeInit();
        KnightAttacks.InvokeInit();

        Debug.Assert(moveBuffersSize > 0);
        
        movesArr = new int[moveBuffersSize];
        moveList = Enumerable.Range(0, moveBuffersSize).ToList();
        bool parsed1 = FenStrings.TryParse(fen, out position);

        movesArr_Legacy = new BinaryMove[moveBuffersSize];
        movesList_Legacy = Enumerable.Range(0, moveBuffersSize).Select(x => new BinaryMove()).ToList();
        bool parsed4 = ChessEngine.Parsing.FenStrings.TryParse(fen, ref position_Legacy);

        // call to ChessEngine.Core.Attacks static ctors.
        _ = ChessEngine.Core.Attacks.KnightAttacks.AttackMasks[0];
        _ = ChessEngine.Core.Attacks.PawnAttacks.AttackMasks[0];
        _ = ChessEngine.Core.Attacks.BishopAttacks.AttackMasks[0];
        _ = ChessEngine.Core.Attacks.RookAttacks.AttackMasks[0];
        _ = ChessEngine.Core.Attacks.KingAttacks.AttackMasks[0];
    }

    #region Pawn Movegen
    [Benchmark]
    public void Pawn_WriteMovesToSpanArray()
    {
        PawnMovement.WriteMovesToSpan(ref position, Colors.Black, movesArr);
    }

    [Benchmark]
    public void Pawn_WriteMovesToUnsafePtr()
    { 
        fixed (Position* posPtr = &position)
        fixed (int* movesPtr = movesArr)
        {
            PawnMovement.WriteMovesUnsafe(posPtr, Colors.Black, movesPtr);
        }
    }

    [Benchmark]
    public void Pawn_WriteMovesToSpanList()
    {         
        PawnMovement.WriteMovesToSpan(ref position, Colors.Black, CollectionsMarshal.AsSpan(moveList));
    }

    [Benchmark]
    public void Pawn_WriteMovesToSpanArray_LEGACY()
    {
        position_Legacy.AddPawnMoves(movesArr_Legacy, 0, 0);
    }

    [Benchmark]
    public void Pawn_WriteMovesToSpanList_LEGACY()
    {
        position_Legacy.AddPawnMoves(CollectionsMarshal.AsSpan(movesList_Legacy), 0, 0);
    }
    #endregion

    #region Knight Movegen
    [Benchmark]
    public void Knight_WriteMovesToSpanArray()
    {
        KnightMovement.WriteMovesToSpan(ref position, Colors.Black, movesArr);
    }

    [Benchmark]
    public void Knight_WriteMovesToUnsafePtr()
    { 
        fixed (Position* posPtr = &position)
        fixed (int* movesPtr = movesArr)
        {
            KnightMovement.WriteMovesUnsafe(posPtr, Colors.Black, movesPtr);
        }
    }

    [Benchmark]
    public void Knight_WriteMovesToSpanList()
    {         
        KnightMovement.WriteMovesToSpan(ref position, Colors.Black, CollectionsMarshal.AsSpan(moveList));
    }
    
    [Benchmark]
    public void Knight_WriteMovesToSpanArray_LEGACY()
    {
        position_Legacy.AddLeapingMoves(
            movesArr_Legacy,
            ChessEngine.Core.Attacks.KnightAttacks.AttackMasks,
            bKnight,
            0);
    }

    [Benchmark]
    public void Knight_WriteMovesToSpanList_LEGACY()
    {
        position_Legacy.AddLeapingMoves(
            CollectionsMarshal.AsSpan(movesList_Legacy),
            ChessEngine.Core.Attacks.KnightAttacks.AttackMasks,
            bKnight,
            offset: 0);
    }
    #endregion

    #region Bishop Movegen
    [Benchmark]
    public void Bishop_WriteMovesToSpanArray_LEGACY()
    {
        position_Legacy.AddSlidingMoves(
            movesArr_Legacy,
            ChessEngine.Core.Attacks.BishopAttacks.GetSliderAttack,
            bBishop,
            offset: 0);
    }

    [Benchmark]
    public void Bishop_WriteMovesToSpanList_LEGACY()
    {
        position_Legacy.AddSlidingMoves(
            CollectionsMarshal.AsSpan(movesList_Legacy),
            ChessEngine.Core.Attacks.BishopAttacks.GetSliderAttack,
            bBishop,
            offset: 0);
    }
    #endregion

    #region Rook Movegen
    [Benchmark]
    public void Rook_WriteMovesToSpanArray_LEGACY()
    {
        position_Legacy.AddSlidingMoves(
            movesArr_Legacy,
            ChessEngine.Core.Attacks.RookAttacks.GetSliderAttack,
            bBishop,
            offset: 0);
    }

    [Benchmark]
    public void Rook_WriteMovesToSpanList_LEGACY()
    {
        position_Legacy.AddSlidingMoves(
            CollectionsMarshal.AsSpan(movesList_Legacy),
            ChessEngine.Core.Attacks.RookAttacks.GetSliderAttack,
            bBishop,
            offset: 0);
    }
    #endregion

    #region Queen Movegen
    [Benchmark]
    public void Queen_WriteMovesToSpanArray_LEGACY()
    {
        position_Legacy.AddSlidingMoves(
            movesArr_Legacy,
            ChessEngine.Core.Attacks.QueenAttacks.GetSliderAttack,
            bBishop,
            offset: 0);
    }

    [Benchmark]
    public void Queen_WriteMovesToSpanList_LEGACY()
    {
        position_Legacy.AddSlidingMoves(
            CollectionsMarshal.AsSpan(movesList_Legacy),
            ChessEngine.Core.Attacks.QueenAttacks.GetSliderAttack,
            bBishop,
            offset: 0);
    }
    #endregion

    #region King Movegen
    [Benchmark]
    public void King_WriteMovesToSpanArray_LEGACY()
    {
        position_Legacy.AddLeapingMoves(
            movesArr_Legacy,
            ChessEngine.Core.Attacks.KingAttacks.AttackMasks,
            bKnight,
            0);
    }

    [Benchmark]
    public void King_WriteMovesToSpanList_LEGACY()
    {
        position_Legacy.AddLeapingMoves(
            CollectionsMarshal.AsSpan(movesList_Legacy),
            ChessEngine.Core.Attacks.KingAttacks.AttackMasks,
            bKnight,
            offset: 0);
    }
    #endregion
}
