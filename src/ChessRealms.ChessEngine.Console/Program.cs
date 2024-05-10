using ChessRealms.ChessEngine.Core.Builders;
using ChessRealms.ChessEngine.Core.Types;
using System.Text.Json;
using System.Text.Json.Serialization;

JsonSerializerOptions jsonOptions = new(JsonSerializerDefaults.Web)
{
    IncludeFields = false,
    WriteIndented = true,
    Converters = 
    { 
        new SquareIndexConverter(), new PieceTypeConverter(), 
        new PieceColorConverter(), new PromotePieceConverter()
    },
};

BinaryMove move = new BinaryMoveBuilder()
    .WithSourceSquare(EnumSquare.a1)
    .WithSourcePiece(PieceType.Queen, PieceColor.White)
    .WithTargetSquare(EnumSquare.h8)
    .WithTargetPiece(PieceType.Bishop, PieceColor.Black)
    .WithCapture()
    .Build();

string JSON = JsonSerializer.Serialize(
    move,
    options: jsonOptions);

Console.WriteLine(JSON);
Console.WriteLine();

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