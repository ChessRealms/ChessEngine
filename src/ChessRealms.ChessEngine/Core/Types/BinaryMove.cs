using static ChessRealms.ChessEngine.Core.Builders.BinaryMoveBuilder;

namespace ChessRealms.ChessEngine.Core.Types;

public readonly struct BinaryMove(uint encodedValue)
{
    public readonly uint EncodedValue = encodedValue;

    public SquareIndex SourceSquare
    {
        get => EncodedValue & SRC_SQUARE;
    }

    public SquareIndex TargetSquare
    {
        get => (EncodedValue & TRG_SQUARE) >> 6;
    }

    public PieceType SourcePieceType
    {
        get => (PieceType)((EncodedValue & SRC_PIECE_TYPE) >> 12);
    }

    public PieceColor SourcePieceColor
    {
        get => (PieceColor)((EncodedValue & SRC_PIECE_COLOR) >> 15);
    }

    public Piece SourcePiece => new(SourcePieceType, SourcePieceColor);

    public PieceType TargetPieceType
    {
        get => (PieceType)((EncodedValue & TRG_PIECE_TYPE) >> 17);
    }

    public PieceColor TargetPieceColor
    {
        get => (PieceColor)((EncodedValue & TRG_PIECE_COLOR) >> 20);
    }

    public Piece TargetPiece => new(TargetPieceType, TargetPieceColor);

    public PromotePiece Promote
    {
        get => (PromotePiece)((EncodedValue & PROMOTE_TO) >> 22);
    }

    public bool IsCapture
    {
        get => ((EncodedValue & IS_CAPTURE) >> 25) != 0;
    }

    public bool IsDoublePush
    {
        get => ((EncodedValue & IS_DOUBLE_PUSH) >> 26) != 0;
    }

    public bool IsEnpassant
    {
        get => ((EncodedValue & IS_ENPASSANT) >> 27) != 0;
    }

    public Castling Castling
    {
        get => (Castling)((EncodedValue & CASTLING) >> 28);
    }

    public BinaryMove() : this(0)
    {
    }

    public BinaryMove(int value) : this(unchecked((uint)value))
    {
    }

    public static implicit operator uint(BinaryMove move) => move.EncodedValue;

    public static implicit operator BinaryMove(uint value) => new(value);

    public static implicit operator int(BinaryMove move) => unchecked((int)move.EncodedValue);

    public static implicit operator BinaryMove(int value) => new(value);
}
