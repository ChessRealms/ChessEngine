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
        get => (PieceType)((EncodedValue & TRG_PIECE_TYPE) >> 17);
    }

    public PieceColor TargetPieceColor
    {
        get => (PieceColor)((EncodedValue & TRG_PIECE_COLOR) >> 20);
    }

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

    public bool IsCastling
    {
        get => ((EncodedValue & IS_CASTLING) >> 28) != 0;
    }

    public BinaryMove() : this(0)
    {
    }

    public static implicit operator int(BinaryMove move) => move.EncodedValue;

    public static implicit operator BinaryMove(int value) => new(value);
}
