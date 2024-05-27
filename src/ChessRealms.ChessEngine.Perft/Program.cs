using ChessRealms.ChessEngine;
using ChessRealms.ChessEngine.Core.Extensions;
using ChessRealms.ChessEngine.Core.Types;
using ChessRealms.ChessEngine.Core.Types.Enums;
using ChessRealms.ChessEngine.Parsing;
using System.Diagnostics;
using System.Text;

if (args.Length != 2)
{
    Console.Error.WriteLine("Invalid arguments amount.");
    Console.Error.WriteLine("Expected input: [depth] [fen]");
    Environment.Exit(1);
    return;
}

if (!int.TryParse(args[0], out var depth) || depth < 1)
{
    Console.Error.WriteLine("Invalid 'depth'.");
    Environment.Exit(1);
    return;
}

ChessBoard board = new();

if (!FenStrings.TryParse(args[1], ref board))
{
    Console.Error.WriteLine("Invalid 'fen'.");
    Console.Error.WriteLine("FEN: {0}", args[1]);
    Environment.Exit(1);
    return;
}


Console.WriteLine("Depth: {0}", args[0]);
Console.WriteLine("Fen:   {0}", args[1]);
Console.WriteLine();

Stopwatch stopwatch = Stopwatch.StartNew();

var nodes = Perft.Test(board, depth);

stopwatch.Stop();

Console.WriteLine();
Console.WriteLine();
Console.WriteLine("{0}", nodes);
Console.WriteLine();
Console.WriteLine("Seconds: {0}", stopwatch.Elapsed.TotalSeconds);
Console.WriteLine("Elapsed: {0}", stopwatch.Elapsed);
Console.WriteLine("ElapsedMilliseconds: {0}", stopwatch.ElapsedMilliseconds);
Console.WriteLine();
Console.WriteLine("Nodes/s: {0}", nodes.Nodes / stopwatch.Elapsed.TotalSeconds);
Console.WriteLine();

static class Perft
{
    public struct PerftResult
    {
        public ulong Nodes;
        public int Captures;
        public int Ep;
        public int Castles;
        public int Promotions;

        public override readonly string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("Nodes: {0:n0}", Nodes));
            sb.AppendLine(string.Format("Captures: {0:n0}", Captures));
            sb.AppendLine(string.Format("Ep: {0:n0}", Ep));
            sb.AppendLine(string.Format("Castles: {0:n0}", Castles));
            sb.AppendLine(string.Format("Promotions: {0:n0}", Promotions));
            return sb.ToString();
        }
    }

    public static PerftResult Test(ChessBoard chessBoard, int depth, bool upper = true)
    {
        ChessBoard tmp = new();

        Span<BinaryMove> moves = stackalloc BinaryMove[218];
        int written = chessBoard.GetMoves(moves, chessBoard.CurrentColor);

        if (depth == 1)
        {
            PerftResult perftResult = new();

            for (int i = 0; i < written; ++i)
            {
                chessBoard.CopyTo(ref tmp);

                tmp.MakeMove(moves[i]);

                if (tmp.IsChecked())
                {
                    continue;
                }

                perftResult.Nodes += 1;

                if (moves[i].IsCapture) perftResult.Captures += 1;
                
                if (moves[i].IsEnpassant)
                {
                    perftResult.Ep += 1;
                }

                if (moves[i].Castling != Castling.None) perftResult.Castles += 1;
                
                if (moves[i].Promote != PromotePiece.None) perftResult.Promotions += 1;
            }

            return perftResult;
        }

        PerftResult finalRes = new();

        for (int i = 0; i < written; ++i)
        {
            chessBoard.CopyTo(ref tmp);
            
            tmp.MakeMove(moves[i]);
            
            if (tmp.IsChecked())
            {
                continue;
            }

            tmp.CurrentColor = chessBoard.CurrentColor.Opposite();
            
            var tmpRes = Test(tmp, depth - 1, false);

            finalRes.Nodes += tmpRes.Nodes;
            finalRes.Captures += tmpRes.Captures;
            finalRes.Ep += tmpRes.Ep;
            finalRes.Castles += tmpRes.Castles;
            finalRes.Promotions += tmpRes.Promotions;

            if (upper)
            {
                Console.WriteLine("{0} {1:n0}", moves[i].ToString(), tmpRes.Nodes);
            }
        }

        return finalRes;
    }
}