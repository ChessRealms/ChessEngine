using ChessRealms.ChessEngine;
using ChessRealms.ChessEngine.Core.Extensions;
using ChessRealms.ChessEngine.Core.Types.Enums;
using ChessRealms.ChessEngine.Parsing;
using System.Diagnostics;
using System.Text;

Stopwatch stopwatch = Stopwatch.StartNew();

if (!FenStrings.TryParse("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 1 1", out ChessBoard board))
{
    return;
}

var nodes = Perft.Test(board, 7);

stopwatch.Stop();

Console.WriteLine();
Console.WriteLine();
Console.WriteLine("{0}", nodes);
Console.WriteLine();
Console.WriteLine("Elapsed: {0}", stopwatch.Elapsed);


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
        var moves = chessBoard.GetMoves(chessBoard.CurrentColor);

        if (depth == 1)
        {
            PerftResult perftResult = new()
            {
                Nodes = (ulong)moves.Count(),
                Captures = moves.Count(x => x.IsCapture),
                Ep = moves.Count(x => x.IsEnpassant),
                Castles = moves.Count(x => x.Castling != Castling.None),
                Promotions = moves.Count(x => x.Promote != PromotePiece.None)
            };

            return perftResult;
        }

        PerftResult finalRes = new();
        
        ChessBoard tmp = new();

        foreach (var move in moves)
        {
            chessBoard.CopyTo(ref tmp);
            tmp.MakeMove(move);
            tmp.CurrentColor = chessBoard.CurrentColor.Opposite();
            var tmpRes = Test(tmp, depth - 1, false);

            finalRes.Nodes += tmpRes.Nodes;
            finalRes.Captures += tmpRes.Captures;
            finalRes.Ep += tmpRes.Ep;
            finalRes.Castles += tmpRes.Castles;
            finalRes.Promotions += tmpRes.Promotions;

            if (upper)
            {
                Console.WriteLine("{0} {1:n0}", move.ToString(), tmpRes.Nodes);
            }
        }

        return finalRes;
    }
}