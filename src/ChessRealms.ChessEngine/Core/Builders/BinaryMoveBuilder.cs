using ChessRealms.ChessEngine.Core.Types;

namespace ChessRealms.ChessEngine.Core.Builders;

public class BinaryMoveBuilder()
{
    public const int SRC_SQUARE             = 0x3f;         // 6 first bits filled with ones (63 as hexdecimal)
    public const int TRG_SQUARE             = 0xfc0;        // 6 bits (63 << 6)

    public const int SRC_PIECE_TYPE         = 0x7000;       // 3 bits (7 << 12)
    public const int SRC_PIECE_COLOR        = 0x8000;       // 1 bit (1 << 15)

    public const int TRG_PIECE_TYPE         = 0x70000;      // 3 bits (7 << 16)
    public const int TRG_PIECE_COLOR        = 0x80000;      // 1 bit (1 << 19)

    public const int PROMOTE                = 0x700000;     // 3 bits (7 << 20)
    public const int CAPTURE                = 0x800000;     // 1 bit (1 << 23)

    public const int DOUBLE_PUSH            = 0x1000000;    // 1 bit (1 << 24)
    public const int ENPASSANT              = 0x2000000;    // 1 bit (1 << 25)
    public const int CASTLING               = 0x4000000;    // 1 bit (1 << 26)

    private int _encodedMove = 0;

    public BinaryMoveBuilder WithSource(SquareIndex square)
    {
        _encodedMove |= square;
        return this;
    }

    public BinaryMoveBuilder WithTarget(SquareIndex square)
    {
        _encodedMove |= square << 6;
        return this;
    }

    public BinaryMoveBuilder WithSourcePiece(PieceType piece, PieceColor color)
    {
        _encodedMove |= ((int)piece) << 12;
        _encodedMove |= ((int)color) << 15;
        return this;
    }

    public BinaryMoveBuilder WithTargetPiece(PieceType piece, PieceColor color)
    {
        _encodedMove |= ((int)piece) << 16;
        _encodedMove |= ((int)color) << 19;
        return this;
    }

    public BinaryMoveBuilder WithPromote(PromotePiece promote)
    {
        _encodedMove |= (int)promote << 20;
        return this;
    }

    public BinaryMoveBuilder WithCapture()
    {
        _encodedMove |= 1 << 23;
        return this;
    }

    public BinaryMoveBuilder WithDoublePush()
    {
        _encodedMove |= 1 << 24;
        return this;
    }

    public BinaryMoveBuilder WithEnpassant()
    {
        _encodedMove |= 1 << 25;
        return this;
    }

    public BinaryMoveBuilder WithCastling()
    {
        _encodedMove |= 1 << 26;
        return this;
    }

    public void Reset()
    {
        _encodedMove &= 0;
    }

    public BinaryMove Build()
    {
        return new BinaryMove(_encodedMove);
    }
}
