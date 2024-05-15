using ChessRealms.ChessEngine;
using ChessRealms.ChessEngine.Console;
using ChessRealms.ChessEngine.Core.Builders;
using ChessRealms.ChessEngine.Core.Types;
using ChessRealms.ChessEngine.Parsing;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

JsonSerializerOptions jsonOptions = new(JsonSerializerDefaults.Web)
{
    IncludeFields = false,
    WriteIndented = true,
    Converters = 
    { 
        new SquareIndexConverter(), new PieceTypeConverter(), 
        new PieceColorConverter(), new PromotePieceConverter(),
        new CastlingConverter(), new PieceConverter()
    },
};

if (!FenStrings.TryParse("r3k2r/p1ppq1b1/bn2pn2/3PN3/Pp2P3/2N2Q1p/1PPBBPpP/R3K2R b KQkq a4 0 1", out ChessBoard chessBoard))
{
    return;
}

var moveBuilder = new BinaryMoveBuilder();
var castlingMove = moveBuilder
    .WithSourceSquare(EnumSquare.e8)
    .WithSourcePiece(PieceType.King, PieceColor.Black)
    .WithTargetSquare(EnumSquare.g8)
    .WithCastling(Castling.BK)
    .Build();

if (chessBoard.TryMakeLegalMove(castlingMove))
{
    Console.WriteLine("BK castling was applied");
}
else
{
    Console.WriteLine("Cant apply move");
}

Console.WriteLine();
Print.Board(chessBoard);

IEnumerable<BinaryMove> moves = chessBoard.GetMoves(chessBoard.CurrentColor);

string JSON = JsonSerializer.Serialize(
    moves,
    options: jsonOptions);

var promoteMoves = moves
    .Where(x => x.Promote != PromotePiece.None)
    .Select(x => string.Format("{0}{1}{2}->{3}", 
        x.SourceSquare,
        x.IsCapture ? 'x' : '-',
        x.TargetSquare,
        x.Promote));

Console.WriteLine(JSON);
Console.WriteLine();
Console.WriteLine(string.Join(' ', promoteMoves));
Console.WriteLine();
Console.WriteLine("Total moves: {0}", moves.Count());

var sb = new StringBuilder();

if (chessBoard.CastlingState.HasFlag(Castling.BK)) sb.Append('k');
if (chessBoard.CastlingState.HasFlag(Castling.BQ)) sb.Append('q');
if (chessBoard.CastlingState.HasFlag(Castling.WK)) sb.Append('K');
if (chessBoard.CastlingState.HasFlag(Castling.WQ)) sb.Append('Q');

Console.WriteLine();
Console.WriteLine("Available Castlings: {0}", sb);

class SquareIndexConverter : JsonConverter<SquareIndex>
{
    public override SquareIndex Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return SquareIndex.Parse(reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, SquareIndex value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

class PieceTypeConverter : JsonConverter<PieceType>
{
    public override PieceType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        _ = Enum.TryParse<PieceType>(reader.GetString(), out var type);
        return type;
    }

    public override void Write(Utf8JsonWriter writer, PieceType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

class PieceColorConverter : JsonConverter<PieceColor>
{
    public override PieceColor Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        _ = Enum.TryParse<PieceColor>(reader.GetString(), out var color);
        return color;
    }

    public override void Write(Utf8JsonWriter writer, PieceColor value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

class PromotePieceConverter : JsonConverter<PromotePiece>
{
    public override PromotePiece Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        _ = Enum.TryParse<PromotePiece>(reader.GetString(), out var piece);
        return piece;
    }

    public override void Write(Utf8JsonWriter writer, PromotePiece value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

class CastlingConverter : JsonConverter<Castling>
{
    public override Castling Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        _ = Enum.TryParse<Castling>(reader.GetString(), out var castling);
        return castling;
    }

    public override void Write(Utf8JsonWriter writer, Castling value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

class PieceConverter : JsonConverter<Piece>
{
    public override Piece Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? value = reader.GetString();

        if (string.IsNullOrEmpty(value))
        {
            return Piece.Empty;
        }

        return PieceCharset.GetPieceFromAscii(value.FirstOrDefault());
    }

    public override void Write(Utf8JsonWriter writer, Piece value, JsonSerializerOptions options)
    {
        char piece = value.Type switch
        {
            PieceType.Pawn => 'p',
            PieceType.Knight => 'n',
            PieceType.Bishop => 'b',
            PieceType.Rook => 'r',
            PieceType.Queen => 'q',
            PieceType.King => 'k',
            _ => '-'
        };

        if (value.Color == PieceColor.White)
        {
            piece = char.ToUpper(piece);
        }

        writer.WriteStringValue(piece.ToString());
    }
}