using ChessRealms.ChessEngine.Core.Constants;
using ChessRealms.ChessEngine.Core.Types.Enums;
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

    public int SourcePieceType
    {
        get => (int)((EncodedValue & SRC_PIECE_TYPE) >> 12);
    }

    public int SourcePieceColor
    {
        get => (int)((EncodedValue & SRC_PIECE_COLOR) >> 15);
    }

    public Piece SourcePiece => new(SourcePieceType, SourcePieceColor);

    public int TargetPieceType
    {
        get => (int)((EncodedValue & TRG_PIECE_TYPE) >> 17);
    }

    public int TargetPieceColor
    {
        get => (int)((EncodedValue & TRG_PIECE_COLOR) >> 20);
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

    public override string ToString()
    {
        char piece = SourcePieceType switch
        {
            ChessConstants.PIECE_PAWN => 'p',
            ChessConstants.PIECE_KNIGHT => 'n',
            ChessConstants.PIECE_BISHOP => 'b',
            ChessConstants.PIECE_ROOK => 'r',
            ChessConstants.PIECE_QUEEN => 'q',
            ChessConstants.PIECE_KING => 'k',
            _ => '\0'
        };
        return string.Format("{0}{1}", SourcePieceColor == ChessConstants.COLOR_WHITE ? char.ToUpper(piece) : piece, TargetSquare);
    }
}
