using ChessRealms.ChessEngine.Core.Types.Enums;
using ChessRealms.ChessEngine.Parsing;

namespace ChessRealms.ChessEngine.Tests.Core.Moves;

public class GetEnpassantMoves
{
    [Test]
    public void GetEnpassantAsWhite()
    {
        const string fen = "rnbqkbnr/pp2pppp/3p4/1Pp5/8/8/P1PPPPPP/RNBQKBNR w - c6 0 1";

        _ = FenStrings.TryParse(fen, out var board);

        var moves = board.GetMoves(PieceColor.White).Where(x => x.IsEnpassant);

        Assert.That(moves.Any(x => x.IsEnpassant), Is.True);
    }
}
