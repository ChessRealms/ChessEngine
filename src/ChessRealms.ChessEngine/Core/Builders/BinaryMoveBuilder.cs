using ChessRealms.ChessEngine.Core.Types;

namespace ChessRealms.ChessEngine.Core.Builders;

public class BinaryMoveBuilder()
{
    public const uint SRC_SQUARE                 = 0x3f;         // 6 first bits filled with ones (63 as hexdecimal)
    public const uint TRG_SQUARE                 = 0xfc0;        // 6 bits (63 << 6)

    public const uint SRC_PIECE_TYPE             = 0x7000;       // 3 bits (7 << 12)
    public const uint SRC_PIECE_COLOR            = 0x18000;      // 2 bit (3 << 15)

    public const uint TRG_PIECE_TYPE             = 0xe0000;      // 3 bits (7 << 17)
    public const uint TRG_PIECE_COLOR            = 0x300000;     // 2 bit (3 << 20)

    public const uint PROMOTE_TO                 = 0x1c00000;    // 3 bits (7 << 22)
    public const uint IS_CAPTURE                 = 0x2000000;    // 1 bit (1 << 25)

    public const uint IS_DOUBLE_PUSH             = 0x4000000;    // 1 bit (1 << 26)
    public const uint IS_ENPASSANT               = 0x8000000;    // 1 bit (1 << 27)
    public const uint CASTLING                   = 0xf0000000;   // 4 bit (15 << 28)
    
    private uint _encodedMove = 0u;

    public BinaryMoveBuilder WithSourceSquare(SquareIndex square)
    {
        _encodedMove |= square;
        return this;
    }

    public BinaryMoveBuilder WithTargetSquare(SquareIndex square)
    {
        _encodedMove |= (uint)square << 6;
        return this;
    }

    public BinaryMoveBuilder WithSourcePiece(PieceType piece, PieceColor color)
    {
        _encodedMove |= unchecked((uint)piece) << 12;
        _encodedMove |= unchecked((uint)color) << 15;
        return this;
    }

    public BinaryMoveBuilder WithSourcePiece(in Piece piece)
    {
        _encodedMove |= unchecked((uint)piece.Type) << 12;
        _encodedMove |= unchecked((uint)piece.Color) << 15;
        return this;
    }

    public BinaryMoveBuilder WithTargetPiece(PieceType piece, PieceColor color)
    {
        _encodedMove |= unchecked((uint)piece) << 17;
        _encodedMove |= unchecked((uint)color) << 20;
        return this;
    }

    public BinaryMoveBuilder WithTargetPiece(in Piece piece)
    {
        _encodedMove |= unchecked((uint)piece.Type) << 17;
        _encodedMove |= unchecked((uint)piece.Color) << 20;
        return this;
    }

    public BinaryMoveBuilder WithPromote(PromotePiece promote)
    {
        _encodedMove |= unchecked((uint)promote) << 22;
        return this;
    }

    public BinaryMoveBuilder ResetPromote()
    {
        _encodedMove ^= _encodedMove & PROMOTE_TO;
        return this;
    }

    public BinaryMoveBuilder WithCapture()
    {
        _encodedMove |= 1u << 25;
        return this;
    }

    public BinaryMoveBuilder WithDoublePush()
    {
        _encodedMove |= 1u << 26;
        return this;
    }

    public BinaryMoveBuilder WithEnpassant()
    {
        _encodedMove |= 1u << 27;
        return this;
    }

    public BinaryMoveBuilder WithCastling(Castling castling)
    {
        _encodedMove |= (unchecked((uint)castling)) << 28;
        return this;
    }

    public void Reset()
    {
        _encodedMove &= 0u;
    }

    public BinaryMove Build()
    {
        return new BinaryMove(_encodedMove);
    }

    public BinaryMoveBuilder Build(out BinaryMove move)
    {
        move = new BinaryMove(_encodedMove);
        return this;
    }
}
