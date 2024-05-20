using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.Types;

namespace ChessRealms.ChessEngine2.Tests.Core.PositionManagement;

internal class SetPieceTests
{
    private static int GenerateSquare() => Random.Shared.Next(0, 64);

    [Test]
    public void SetWhitePawn()
    {
        int square = GenerateSquare();
        int color = Colors.White;
        int piece = Pieces.Pawn;

        Position position = new();
        position.SetPieceAt(square, piece, color);
        Piece whiteBishop = position.GetPieceAt(square, color);

        Assert.Multiple(() =>
        {
            Assert.That(whiteBishop.Color, Is.EqualTo(color));
            Assert.That(whiteBishop.Value, Is.EqualTo(piece));
        });
    }

    [Test]
    public void SetBlackPawn()
    {
        int square = GenerateSquare();
        int color = Colors.Black;
        int piece = Pieces.Pawn;

        Position position = new();
        position.SetPieceAt(square, piece, color);
        Piece whiteBishop = position.GetPieceAt(square, color);

        Assert.Multiple(() =>
        {
            Assert.That(whiteBishop.Color, Is.EqualTo(color));
            Assert.That(whiteBishop.Value, Is.EqualTo(piece));
        });
    }

    [Test]
    public void SetWhiteKnight()
    {
        int square = GenerateSquare();
        int color = Colors.White;
        int piece = Pieces.Knight;

        Position position = new();
        position.SetPieceAt(square, piece, color);
        Piece whiteBishop = position.GetPieceAt(square, color);

        Assert.Multiple(() =>
        {
            Assert.That(whiteBishop.Color, Is.EqualTo(color));
            Assert.That(whiteBishop.Value, Is.EqualTo(piece));
        });
    }

    [Test]
    public void SetBlackKnight()
    {
        int square = GenerateSquare();
        int color = Colors.Black;
        int piece = Pieces.Knight;

        Position position = new();
        position.SetPieceAt(square, piece, color);
        Piece whiteBishop = position.GetPieceAt(square, color);

        Assert.Multiple(() =>
        {
            Assert.That(whiteBishop.Color, Is.EqualTo(color));
            Assert.That(whiteBishop.Value, Is.EqualTo(piece));
        });
    }

    [Test]
    public void SetWhiteBishop()
    {
        int square = GenerateSquare();
        int color = Colors.White;
        int piece = Pieces.Bishop;

        Position position = new();
        position.SetPieceAt(square, piece, color);
        Piece whiteBishop = position.GetPieceAt(square, color);

        Assert.Multiple(() =>
        {
            Assert.That(whiteBishop.Color, Is.EqualTo(color));
            Assert.That(whiteBishop.Value, Is.EqualTo(piece));
        });
    }

    [Test]
    public void SetBlackBishop()
    {
        int square = GenerateSquare();
        int color = Colors.Black;
        int piece = Pieces.Knight;

        Position position = new();
        position.SetPieceAt(square, piece, color);
        Piece whiteBishop = position.GetPieceAt(square, color);

        Assert.Multiple(() =>
        {
            Assert.That(whiteBishop.Color, Is.EqualTo(color));
            Assert.That(whiteBishop.Value, Is.EqualTo(piece));
        });
    }

    [Test]
    public void SetWhiteRook()
    {
        int square = GenerateSquare();
        int color = Colors.White;
        int piece = Pieces.Rook;

        Position position = new();
        position.SetPieceAt(square, piece, color);
        Piece whiteBishop = position.GetPieceAt(square, color);

        Assert.Multiple(() =>
        {
            Assert.That(whiteBishop.Color, Is.EqualTo(color));
            Assert.That(whiteBishop.Value, Is.EqualTo(piece));
        });
    }

    [Test]
    public void SetBlackRook()
    {
        int square = GenerateSquare();
        int color = Colors.Black;
        int piece = Pieces.Rook;

        Position position = new();
        position.SetPieceAt(square, piece, color);
        Piece whiteBishop = position.GetPieceAt(square, color);

        Assert.Multiple(() =>
        {
            Assert.That(whiteBishop.Color, Is.EqualTo(color));
            Assert.That(whiteBishop.Value, Is.EqualTo(piece));
        });
    }

    [Test]
    public void SetWhiteQueen()
    {
        int square = GenerateSquare();
        int color = Colors.White;
        int piece = Pieces.Queen;

        Position position = new();
        position.SetPieceAt(square, piece, color);
        Piece whiteBishop = position.GetPieceAt(square, color);

        Assert.Multiple(() =>
        {
            Assert.That(whiteBishop.Color, Is.EqualTo(color));
            Assert.That(whiteBishop.Value, Is.EqualTo(piece));
        });
    }

    [Test]
    public void SetBlackQueen()
    {
        int square = GenerateSquare();
        int color = Colors.Black;
        int piece = Pieces.Queen;

        Position position = new();
        position.SetPieceAt(square, piece, color);
        Piece whiteBishop = position.GetPieceAt(square, color);

        Assert.Multiple(() =>
        {
            Assert.That(whiteBishop.Color, Is.EqualTo(color));
            Assert.That(whiteBishop.Value, Is.EqualTo(piece));
        });
    }

    [Test]
    public void SetWhiteKing()
    {
        int square = GenerateSquare();
        int color = Colors.White;
        int piece = Pieces.King;

        Position position = new();
        position.SetPieceAt(square, piece, color);
        Piece whiteBishop = position.GetPieceAt(square, color);

        Assert.Multiple(() =>
        {
            Assert.That(whiteBishop.Color, Is.EqualTo(color));
            Assert.That(whiteBishop.Value, Is.EqualTo(piece));
        });
    }

    [Test]
    public void SetBlackKing()
    {
        int square = GenerateSquare();
        int color = Colors.White;
        int piece = Pieces.King;

        Position position = new();
        position.SetPieceAt(square, piece, color);
        Piece whiteBishop = position.GetPieceAt(square, color);

        Assert.Multiple(() =>
        {
            Assert.That(whiteBishop.Color, Is.EqualTo(color));
            Assert.That(whiteBishop.Value, Is.EqualTo(piece));
        });
    }
}
