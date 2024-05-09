using static ChessRealms.ChessEngine.Core.Builders.BinaryMoveBuilder;

namespace ChessRealms.ChessEngine.Core.Types;

public readonly struct BinaryMove(int encodedValue)
{
    public readonly int EncodedValue = encodedValue;

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

    public PieceType TargetPieceType
    {
        get => (PieceType)((EncodedValue & TRG_PIECE_TYPE) >> 16);
    }

    public PieceColor TargetPieceColor
    {
        get => (PieceColor)((EncodedValue & TRG_PIECE_COLOR) >> 19);
    }

    public PromotePiece Promote
    {
        get => (PromotePiece)((EncodedValue & PROMOTE) >> 20);
    }

    public bool IsCapture
    {
        get => ((EncodedValue & CAPTURE) >> 23) != 0;
    }

    public bool IsDoublePush
    {
        get => ((EncodedValue & DOUBLE_PUSH) >> 24) != 0;
    }

    public bool IsEnpassant
    {
        get => ((EncodedValue & ENPASSANT) >> 25) != 0;
    }

    public bool IsCastling
    {
        get => ((EncodedValue & CASTLING) >> 26) != 0;
    }

    public BinaryMove() : this(0)
    {
    }

    public static implicit operator int(BinaryMove move) => move.EncodedValue;

    public static implicit operator BinaryMove(int value) => new(value);
}
