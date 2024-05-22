using ChessRealms.ChessEngine2.Core.Attacks;
using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.Math;
using ChessRealms.ChessEngine2.Core.Types;
using ChessRealms.ChessEngine2.Debugs;
using System.Diagnostics;

namespace ChessRealms.ChessEngine2.Core.MoveGeneration;

internal static class KnightMovement
{
    public static unsafe int WriteMovesUnsafe(Position* position, int color, int* dest, int offset = 0)
    {
        DebugAsserts.ValidColor(color);
        Debug.Assert(offset >= 0);

        int* cursor = dest + offset;

        int enemyColor = Colors.Mirror(color);
        ulong myBlockers = position->blockers[color];
        ulong enemyBlockers = position->blockers[enemyColor];
        ulong knightBB = position->pieceBBs[Position.BBIndex(Pieces.Knight, color)];

        while (BitboardOps.IsNotEmpty(knightBB))
        {
            int srcSquare = BitboardOps.Lsb(knightBB);
            int trgSquare;

            ulong movesBB = ~myBlockers & *(KnightAttacks.AttackMasksUnsafe + srcSquare);
            ulong captures = enemyBlockers & movesBB;
            ulong normalMoves = captures ^ movesBB;

            while (BitboardOps.IsNotEmpty(captures))
            {
                trgSquare = BitboardOps.Lsb(captures);
                *cursor++ = BinaryMoveOps.EncodeMove(
                    srcSquare, Pieces.Knight, color, trgSquare, 
                    capture: 1);
                captures = BitboardOps.PopBitAt(captures, trgSquare);
            }

            while (BitboardOps.IsNotEmpty(normalMoves))
            {
                trgSquare = BitboardOps.Lsb(normalMoves);
                *cursor++ = BinaryMoveOps.EncodeMove(
                    srcSquare, Pieces.Knight, color, trgSquare);
                normalMoves = BitboardOps.PopBitAt(normalMoves, trgSquare);
            }

            knightBB = BitboardOps.PopBitAt(knightBB, srcSquare);
        }

        return (int)(cursor - dest);
    }

    public static unsafe int WriteMovesToSpan(ref Position position, int color, Span<int> dest, int offset = 0)
    {
        DebugAsserts.ValidColor(color);
        Debug.Assert(offset >= 0);

        int cursor = offset;

        int enemyColor = Colors.Mirror(color);
        ulong myBlockers = position.blockers[color];
        ulong enemyBlockers = position.blockers[enemyColor];
        ulong knightBB = position.pieceBBs[Position.BBIndex(Pieces.Knight, color)];

        while (BitboardOps.IsNotEmpty(knightBB))
        {
            int srcSquare = BitboardOps.Lsb(knightBB);
            int trgSquare;

            ulong movesBB = ~myBlockers & KnightAttacks.AttackMasks[srcSquare];
            ulong captures = enemyBlockers & movesBB;
            ulong normalMoves = captures ^ movesBB;

            while (BitboardOps.IsNotEmpty(captures))
            {
                trgSquare = BitboardOps.Lsb(captures);
                dest[cursor++] = BinaryMoveOps.EncodeMove(
                    srcSquare, Pieces.Knight, color, trgSquare, 
                    capture: 1);
                captures = BitboardOps.PopBitAt(captures, trgSquare);
            }

            while (BitboardOps.IsNotEmpty(normalMoves))
            {
                trgSquare = BitboardOps.Lsb(normalMoves);
                dest[cursor++] = BinaryMoveOps.EncodeMove(
                    srcSquare, Pieces.Knight, color, trgSquare);
                normalMoves = BitboardOps.PopBitAt(normalMoves, trgSquare);
            }

            knightBB = BitboardOps.PopBitAt(knightBB, srcSquare);
        }

        return cursor - offset;
    }
}
