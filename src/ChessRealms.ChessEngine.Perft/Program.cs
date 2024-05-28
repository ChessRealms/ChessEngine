using ChessRealms.ChessEngine2.Core.Attacks;
using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.Math;
using ChessRealms.ChessEngine2.Core.Movements;
using ChessRealms.ChessEngine2.Core.Types;
using ChessRealms.ChessEngine2.Parsing;
using System.Diagnostics;
using System.Text;

string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
int depth = 6;

Console.WriteLine("Fen: {0}", fen);
Console.WriteLine("Depth: {0}", depth);
Console.WriteLine();

AttackLookups.InvokeInit();

_ = FenStrings.TryParse(fen, out Position pos);

Stopwatch stopwatch = Stopwatch.StartNew();

var nodes = Perft.PerftDriver.Test(pos, depth);

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

namespace Perft
{
    public static class PerftDriver
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

        public static unsafe PerftResult Test(Position pos, int depth, bool upper = true)
        {
            Position tmpPos = new();
            int* moves = stackalloc int[218];

            int written = MoveGen.WriteMovesToPtrUnsafe(
                &pos, pos.color, moves);

            if (depth == 1)
            {
                PerftResult perftResult = new();

                for (int i = 0; i < written; ++i)
                {
                    pos.CopyTo(&tmpPos);

                    MoveDriver.MakeMove(ref tmpPos, moves[i]);

                    if (tmpPos.IsKingChecked(tmpPos.color))
                    {
                        continue;
                    }

                    perftResult.Nodes += 1;

                    if (BinaryMoveOps.DecodeCapture(moves[i]) != 0)
                        perftResult.Captures += 1;

                    if (BinaryMoveOps.DecodeEnpassant(moves[i]) != 0)
                    {
                        perftResult.Ep += 1;
                    }

                    if (BinaryMoveOps.DecodeCastling(moves[i]) != 0)
                        perftResult.Castles += 1;

                    if (BinaryMoveOps.DecodePromotion(moves[i]) != 0)
                        perftResult.Promotions += 1;
                }

                return perftResult;
            }

            PerftResult finalRes = new();

            for (int i = 0; i < written; ++i)
            {
                pos.CopyTo(&tmpPos);

                MoveDriver.MakeMove(ref tmpPos, moves[i]);

                if (tmpPos.IsKingChecked(tmpPos.color))
                {
                    continue;
                }

                tmpPos.color = Colors.Mirror(tmpPos.color);

                var tmpRes = Test(tmpPos, depth - 1, false);

                finalRes.Nodes += tmpRes.Nodes;
                finalRes.Captures += tmpRes.Captures;
                finalRes.Ep += tmpRes.Ep;
                finalRes.Castles += tmpRes.Castles;
                finalRes.Promotions += tmpRes.Promotions;

                if (upper)
                {
                    var src = SquareOps.ToAbbreviature(BinaryMoveOps.DecodeSrc(moves[i]));
                    var trg = SquareOps.ToAbbreviature(BinaryMoveOps.DecodeTrg(moves[i]));

                    Console.WriteLine("{0}{1} {2:n0}", src, trg, tmpRes.Nodes);
                }
            }

            return finalRes;
        }
    }
}
