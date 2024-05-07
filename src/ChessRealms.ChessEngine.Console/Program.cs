using ChessRealms.ChessEngine;
using ChessRealms.ChessEngine.Console;
using ChessRealms.ChessEngine.Core.Types;
using ChessRealms.ChessEngine.Parsing;

var fen = "rnbqkbnr/pp1ppppp/8/2p5/4P3/5N2/PPPP1PPP/RNBQKB1R b KQkq - 1 2";

if (!FenStrings.TryParse(fen, out ChessBoard chessBoard))
{
    Console.WriteLine("Cant parse fen.");
    return;
}

Print.Board(in chessBoard!);

class Print
{
    public static void Board(in ChessBoard chessBoard)
    {
        for (int rank = 7; rank >= 0; --rank)
        {
            Console.Write(" {0}", rank + 1);

            for (int file = 7; file >= 0; --file)
            {
                SquareIndex square = SquareIndex.FromFileRank(file, rank);
                Piece? piece = chessBoard.GetPieceAt(square);
                
                if (piece == null)
                {
                    Console.Write(" .");
                }
                else
                {
                    int index = (int)piece.Type + (piece.Color == PieceColor.Black ? 6 : 0);

                    Console.Write(" {0}", PieceCharset.ASCII[index]);
                }
            }

            Console.WriteLine();
        }

        Console.WriteLine("   a b c d e f g h");
    }
}