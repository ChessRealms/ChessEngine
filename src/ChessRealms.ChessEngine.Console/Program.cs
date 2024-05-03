using ChessRealms.ChessEngine.Types;

SquareIndex a4 = EnumSquare.a4;
Print.Board(a4.BitBoard);

class Print
{
    public static void Board(BitBoard board) => Board((ulong)board);

    public static void Board(ulong board)
    {
        for (int rank = 0; rank < 8; ++rank)
        {
            Console.Write(" {0}", rank + 1);

            for (int file = 0; file < 8; ++file)
            {
                int square = rank * 8 + file;
                Console.Write(" {0}", (board & (1UL << square)) > 0 ? 1 : 0);
            }

            Console.WriteLine();
        }

        Console.WriteLine("   a b c d e f g h");
    }
}