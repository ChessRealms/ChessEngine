using BenchmarkDotNet.Attributes;
using ChessRealms.ChessEngine;
using ChessRealms.ChessEngine2.Core.Attacks;
using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.Types;
using ChessRealms.ChessEngine2.Parsing;
using System.Runtime.CompilerServices;

namespace ChessRealms.ChessEngine2.Benchmark.MoveGenerations;

public class IsSquareAttackedBenchmarks
{
    private Position position;
    private ChessBoard positionLegacy;

    public IsSquareAttackedBenchmarks()
    {
        _ = FenStrings.TryParse(
            "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 1 1",
            out position);

        _ = ChessEngine.Parsing.FenStrings.TryParse(
            "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 1 1",
            ref positionLegacy);

        AttackLookups.InvokeInit();

        #region Invoke static ctors for 'legacy' engine.
        _ = ChessEngine.Core.Attacks.PawnAttacks.AttackMasks[0];
        _ = ChessEngine.Core.Attacks.KnightAttacks.AttackMasks[0];
        _ = ChessEngine.Core.Attacks.RookAttacks.AttackMasks[0];
        _ = ChessEngine.Core.Attacks.BishopAttacks.AttackMasks[0];
        _ = ChessEngine.Core.Attacks.KingAttacks.AttackMasks[0];
        #endregion
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void IsSquareAttacked_B()
    {
        _ = position.IsSquareAttacked(Squares.e1, Colors.Black);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void IsSquareAttacked_W()
    {
        _ = position.IsSquareAttacked(Squares.e8, Colors.White);
    }

#if LEGACY_FUNC
    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void IsSquareAttacked_LEGACY()
    {
        _ = positionLegacy.IsSquareAttacked(Squares.e1, Colors.Black);
    }
#endif
}
