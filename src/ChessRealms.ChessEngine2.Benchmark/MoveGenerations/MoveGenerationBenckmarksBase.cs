using ChessRealms.ChessEngine2.Core.Types;
using ChessRealms.ChessEngine2.Core.Attacks;
using ChessRealms.ChessEngine2.Parsing;

namespace ChessRealms.ChessEngine2.Benchmark.MoveGenerations;

public abstract class MoveGenerationBenckmarksBase
{
    #region For current ChessEngine2
    protected readonly int[] movesArr;
    protected readonly List<int> moveList;

    protected Position position;
    #endregion

#if LEGACY_FUNC
    #region For legacy ChessEngine
    protected ChessBoard position_Legacy = new();
    protected readonly BinaryMove[] movesArr_Legacy;
    protected readonly List<BinaryMove> movesList_Legacy;
    #endregion
#endif

    public MoveGenerationBenckmarksBase(
        string fen = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 1 1",
        int moveBuffersSize = 218) 
    {
        PawnAttacks.InvokeInit();
        KnightAttacks.InvokeInit();
        BishopAttacks.InvokeInit();
        RookAttacks.InvokeInit();
        KingAttacks.InvokeInit();

        movesArr = new int[moveBuffersSize];
        moveList = Enumerable.Range(0, moveBuffersSize).ToList();
        bool parsed1 = FenStrings.TryParse(fen, out position);

#if LEGACY_FUNC
        movesArr_Legacy = new BinaryMove[moveBuffersSize];
        movesList_Legacy = Enumerable.Range(0, moveBuffersSize).Select(x => new BinaryMove()).ToList();
        bool parsed4 = ChessEngine.Parsing.FenStrings.TryParse(fen, ref position_Legacy);

        _ = ChessEngine.Core.Attacks.KnightAttacks.AttackMasks[0];
        _ = ChessEngine.Core.Attacks.PawnAttacks.AttackMasks[0];
        _ = ChessEngine.Core.Attacks.BishopAttacks.AttackMasks[0];
        _ = ChessEngine.Core.Attacks.RookAttacks.AttackMasks[0];
        _ = ChessEngine.Core.Attacks.KingAttacks.AttackMasks[0];
#endif
    }
}
