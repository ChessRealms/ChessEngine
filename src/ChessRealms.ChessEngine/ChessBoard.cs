using ChessRealms.ChessEngine.Core.Types;

namespace ChessRealms.ChessEngine;

public struct ChessBoard
{
    private BitBoard _whitePawn;
    private BitBoard _whiteKnight;
    private BitBoard _whiteBishop;
    private BitBoard _whiteRook;
    private BitBoard _whiteQueen;
    private BitBoard _whiteKing;

    private BitBoard _blackPawn;
    private BitBoard _blackKnight;
    private BitBoard _blackBishop;
    private BitBoard _blackRook;
    private BitBoard _blackQueen;
    private BitBoard _blackKing;
    
    private BitBoard _whiteOccupancy;
    private BitBoard _blackOccupancy;
    private BitBoard _allOccupancy;

    public SquareIndex Enpassant { get; set; }

    public PieceColor CurrentColor { get; set; }

    public Castling Castling { get; set; }

    public int HalfMoveClock { get; set; }

    public int FullMoveNumber { get; set; }

    public ChessBoard()
    {
        Enpassant = SquareIndex.None;
        CurrentColor = PieceColor.White;
        Castling = 0;
    }

    public void SetPieceAt(SquareIndex square, Piece piece)
    {
        SetPieceAt(square, piece.Color, piece.Type);
    }

    public readonly Piece? GetPieceAt(SquareIndex square)
    {
        if (_allOccupancy.GetBitAt(square) == 0)
        {
            return null;
        }

        PieceColor pieceColor = _whiteOccupancy.GetBitAt(square) > 0 ? PieceColor.White : PieceColor.Black;
        PieceType pieceType;

        if (pieceColor == PieceColor.White)
        {
            pieceType = _whitePawn.GetBitAt(square) > 0
                ? PieceType.Pawn
                : _whiteKnight.GetBitAt(square) > 0
                ? PieceType.Knight
                : _whiteBishop.GetBitAt(square) > 0
                ? PieceType.Bishop
                : _whiteRook.GetBitAt(square) > 0
                ? PieceType.Rook
                : _whiteQueen.GetBitAt(square) > 0
                ? PieceType.Queen
                : PieceType.King;
        }
        else
        {
            pieceType = _blackPawn.GetBitAt(square) > 0
                ? PieceType.Pawn
                : _blackKnight.GetBitAt(square) > 0
                ? PieceType.Knight
                : _blackBishop.GetBitAt(square) > 0
                ? PieceType.Bishop
                : _blackRook.GetBitAt(square) > 0
                ? PieceType.Rook
                : _blackQueen.GetBitAt(square) > 0
                ? PieceType.Queen
                : PieceType.King;
        }

        return new Piece
        {
            Color = pieceColor,
            Type = pieceType
        };
    }

    public void SetPieceAt(SquareIndex square, PieceColor color, PieceType piece)
    {
        switch (color)
        {
            case PieceColor.White: 
                switch (piece)
                {
                    case PieceType.Pawn: _whitePawn.SetBitAt(square); break;
                    case PieceType.Knight: _whiteKnight.SetBitAt(square); break; 
                    case PieceType.Bishop: _whiteBishop.SetBitAt(square); break;
                    case PieceType.Rook: _whiteRook.SetBitAt(square); break;
                    case PieceType.Queen: _whiteQueen.SetBitAt(square); break;
                    case PieceType.King: _whiteKing.SetBitAt(square); break; 
                    default: break;
                }

                _whiteOccupancy.SetBitAt(square);
                _allOccupancy.SetBitAt(square);
                break;

            case PieceColor.Black:
                switch (piece)
                {
                    case PieceType.Pawn: _blackPawn.SetBitAt(square); break;
                    case PieceType.Knight: _blackKnight.SetBitAt(square); break; 
                    case PieceType.Bishop: _blackBishop.SetBitAt(square); break;
                    case PieceType.Rook: _blackRook.SetBitAt(square); break;
                    case PieceType.Queen: _blackQueen.SetBitAt(square); break;
                    case PieceType.King: _blackKing.SetBitAt(square); break; 
                    default: break;
                }

                _blackOccupancy.SetBitAt(square);
                _allOccupancy.SetBitAt(square);
                break;

            default: break;
        }
    }

    public void RemovePieceAt(SquareIndex square)
    {
        Piece? piece = GetPieceAt(square);
        if (piece == null)
        {
            return;
        }

        switch (piece.Color)
        {
            case PieceColor.White:
                switch (piece.Type)
                {
                    case PieceType.Pawn: _whitePawn.PopBitAt(square); break;
                    case PieceType.Knight: _whiteKnight.PopBitAt(square); break;
                    case PieceType.Bishop: _whiteBishop.PopBitAt(square); break;
                    case PieceType.Rook: _whiteRook.PopBitAt(square); break;
                    case PieceType.Queen: _whiteQueen.PopBitAt(square); break;
                    case PieceType.King: _whiteKing.PopBitAt(square); break;
                    default: break;
                }
                _whiteOccupancy.PopBitAt(square); 
                break;
            case PieceColor.Black:
                switch (piece.Type)
                {
                    case PieceType.Pawn: _blackPawn.PopBitAt(square); break;
                    case PieceType.Knight: _blackKnight.PopBitAt(square); break;
                    case PieceType.Bishop: _blackBishop.PopBitAt(square); break;
                    case PieceType.Rook: _blackRook.PopBitAt(square); break;
                    case PieceType.Queen: _blackQueen.PopBitAt(square); break;
                    case PieceType.King: _blackKing.PopBitAt(square); break;
                    default: break;
                }
                _blackOccupancy.PopBitAt(square); 
                break;

            default: break;
        }

        _allOccupancy.PopBitAt(square);
    }
}
