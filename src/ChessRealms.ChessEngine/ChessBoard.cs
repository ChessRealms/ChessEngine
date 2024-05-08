using ChessRealms.ChessEngine.Core.Types;

namespace ChessRealms.ChessEngine;

public struct ChessBoard
{
    private readonly BitBoard[,] _pieces;    
    private readonly BitBoard[] _occupancies;

    private BitBoard _allOccupancy;

    public SquareIndex Enpassant { get; set; }

    public PieceColor CurrentColor { get; set; }

    public Castling Castling { get; set; }

    public int HalfMoveClock { get; set; }

    public int FullMoveNumber { get; set; }

    public ChessBoard()
    {
        _pieces = new BitBoard[2, 6];
        _occupancies = new BitBoard[2];

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

        BitBoard whiteOcc = _occupancies[(int)PieceColor.White];

        int color = (int)(whiteOcc.GetBitAt(square) > 0 
            ? PieceColor.White 
            : PieceColor.Black);
        
        int piece = -1;

        for (int pieceIndex = 0; pieceIndex < 6; ++pieceIndex)
        {
            if (_pieces[color, pieceIndex].GetBitAt(square) > 0)
            {
                piece = pieceIndex;
                break;
            }
        }

        if (piece == -1)
        {
            return null;
        }

        return new Piece
        {
            Color = (PieceColor)color,
            Type = (PieceType)piece
        };
    }

    public void SetPieceAt(SquareIndex square, PieceColor color, PieceType piece)
    {
        int colorIndex = (int)color;
        int pieceIndex = (int)piece;
        
        _pieces[colorIndex, pieceIndex].SetBitAt(square);
        _occupancies[colorIndex].SetBitAt(square);
        _allOccupancy.SetBitAt(square);
    }

    public void RemovePieceAt(SquareIndex square)
    {
        Piece? piece = GetPieceAt(square);

        if (piece == null)
        {
            return;
        }

        int colorIndex = (int)piece.Color;
        int pieceIndex = (int)piece.Type;

        _pieces[colorIndex, pieceIndex].PopBitAt(square);
        _occupancies[colorIndex].PopBitAt(square);
        _allOccupancy.PopBitAt(square);
    }
}
