using ChessRealms.ChessEngine.Core.Types;

SquareIndex index = EnumSquare.d2;
Print.Board(index.BitBoard);

class Print
{
    public static void Board(BitBoard board)
    {
        for (int rank = 0; rank < 8; ++rank)
        {
            Console.Write(" {0}", rank + 1);

            for (int file = 0; file < 8; ++file)
            {
                SquareIndex square = rank * 8 + file;
                Console.Write(" {0}", Convert.ToInt32((board & square.BitBoard) > 0));
            }

            Console.WriteLine();
        }

        Console.WriteLine("   a b c d e f g h");
    }
}