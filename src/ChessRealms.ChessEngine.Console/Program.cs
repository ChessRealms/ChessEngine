using ChessRealms.ChessEngine;
using ChessRealms.ChessEngine.Core.Math;
using ChessRealms.ChessEngine.Parsing;

MoveResult lastMoveResult = MoveResult.None;
ChessGame chessGame = new();

while (true)
{
    Console.Clear();
    Console.WriteLine("Last move: {0}", lastMoveResult);
    Console.WriteLine("Color to move: {0}", chessGame.CurrentColor);
    Console.WriteLine();

    PrintBoard(ref chessGame);
    Console.WriteLine();

    if (chessGame.IsFinished)
    {
        break;
    }

    Console.Write("Enter move: ");
    string? inputMove = Console.ReadLine();
    if (string.IsNullOrEmpty(inputMove) || inputMove.Length < 4)
    {
        Console.WriteLine("invalid move");
        Console.Write("Press Enter");
        Console.ReadLine();
        continue;
    }

    var (src, trg) = AlgebraicNotation.ParseMove(inputMove);
    lastMoveResult = chessGame.MakeMove(src, trg);
}

Console.WriteLine("{0} win!!! (press any key to exit)", chessGame.CurrentColor);
Console.ReadKey();

static void PrintBoard(ref ChessGame chessGame)
{
    Span<ChessPiece> pieces = stackalloc ChessPiece[64];
    chessGame.GetBoardToSpan(pieces);

    Console.WriteLine("   a b c d e f g h");

    for (int r = 7; r >= 0; --r)
    {
        Console.Write(" {0} ", r + 1);

        for (int f = 0; f < 8; ++f)
        {
            int square = SquareOps.FromFileRank(f, r);

            if (pieces[square].IsEmpty()) 
            {
                Console.Write('.');
            }
            else
            {
                Console.Write(PieceToString(ref pieces[square]));
            }

            Console.Write(' ');
        }

        Console.WriteLine();
    }
}

static char PieceToString(ref ChessPiece piece)
{
    char p = piece.Value switch
    {
        PieceValue.Pawn => 'p',
        PieceValue.Knight => 'k',
        PieceValue.Bishop => 'b',
        PieceValue.Rook => 'r',
        PieceValue.Queen => 'q',
        PieceValue.King => 'k',
        _ => '\0'
    };

    return piece.Color == PieceColor.White ? char.ToUpper(p) : p;
}
