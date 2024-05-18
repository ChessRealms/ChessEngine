using ChessRealms.ChessEngine.Core.Constants;
using ChessRealms.ChessEngine.Core.Extensions;
using ChessRealms.ChessEngine.Core.Types;
using ChessRealms.ChessEngine.Core.Types.Enums;
using static System.Console;

namespace ChessRealms.ChessEngine.Console;

public static class Print
{
    public static void Board(in ChessBoard chessBoard)
    {
        for (int rank = 7; rank >= 0; --rank)
        {
            Write(" {0}", rank + 1);

            for (int file = 0; file < 8; ++file)
            {
                SquareIndex square = SquareIndex.FromFileRank(file, rank);
                
                if (!chessBoard.TryGetPieceAt(square, out Piece piece))
                {
                    Write(" .");
                }
                else
                {
                    int index = piece.Type + (piece.Color == ChessConstants.COLOR_BLACK ? 6 : 0);

                    Write(" {0}", PieceCharset.ASCII[index]);
                }
            }

            WriteLine();
        }

        WriteLine("   a b c d e f g h");
    }

    public static void Board(ulong bitboard)
    {
        for (int rank = 7; rank >= 0; --rank)
        {
            Write(" {0}", rank + 1);

            for (int file = 0; file < 8; ++file)
            {
                var bit = (bitboard & SquareIndex.FromFileRank(file, rank).Board) != 0 ? 1 : 0;
                Write(" {0}", bit);
            }

            WriteLine();
        }

        WriteLine("   a b c d e f g h");
    }
}
