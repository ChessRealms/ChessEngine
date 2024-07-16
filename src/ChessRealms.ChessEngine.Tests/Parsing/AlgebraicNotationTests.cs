using ChessRealms.ChessEngine.Core.Constants;
using ChessRealms.ChessEngine.Parsing;
using System.ComponentModel.DataAnnotations;

namespace ChessRealms.ChessEngine.Tests.Parsing;

internal class AlgebraicNotationTests
{
    [Test]
    public void ParseSquare_A4()
    {
        string a4 = "a4";
        int parsedSquare = AlgebraicNotation.ParseSquare(a4);
        Assert.That(parsedSquare, Is.EqualTo(Squares.a4));
    }

    [Test]
    public void ParseMove_A1H8()
    {
        string a1h8 = "a1h8";
        var (src, trg) = AlgebraicNotation.ParseAlgebraicMove(a1h8);
        Assert.Multiple(() =>
        {
            Assert.That(src, Is.EqualTo(Squares.a1));
            Assert.That(trg, Is.EqualTo(Squares.h8));
        });
    }

    [Test]
    public void TryParseMove_A1A8_Succeed()
    {
        string a1h8 = "a1h8";
        bool parsed = AlgebraicNotation.TryParseMove(a1h8, out (int src, int trg) move);

        Assert.Multiple(() =>
        {
            Assert.That(parsed, Is.True);
            Assert.That(move.src, Is.EqualTo(Squares.a1));
            Assert.That(move.trg, Is.EqualTo(Squares.h8));
        });
    }

    [Test]
    public void TryParseMove_A1J3_Failed()
    {
        string a1h8 = "a1j3";
        bool parsed = AlgebraicNotation.TryParseMove(a1h8, out (int src, int trg) move);

        Assert.Multiple(() =>
        {
            Assert.That(parsed, Is.False);
            Assert.That(move.src, Is.EqualTo(Squares.Empty));
            Assert.That(move.trg, Is.EqualTo(Squares.Empty));
        });
    }
}
