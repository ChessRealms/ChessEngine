using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.Math;
using ChessRealms.ChessEngine2.Core.Types;
using ChessRealms.ChessEngine2.Debugs;
using System.Collections.Immutable;
using System.Diagnostics;

namespace ChessRealms.ChessEngine2.Core.MoveGeneration;

internal unsafe static class LeapingMovement
{
    public static int WriteMovesToPtrUnsafe(
        Position* position, 
        int color, 
        int piece,
        ulong* masksLookup, 
        int* dest, 
        int offset = 0)
    {
        DebugAsserts.ValidColor(color);
        DebugAsserts.LeapingPiece(piece);
        Debug.Assert(offset >= 0);

        int* cursor = dest + offset;

        int enemyColor = Colors.Mirror(color);
        ulong myBlockers = position->blockers[color];
        ulong enemyBlockers = position->blockers[enemyColor];
        ulong pieceBB = position->pieceBBs[Position.BBIndex(piece, color)];

        int srcSquare;
        int trgSquare;

        while (BitboardOps.IsNotEmpty(pieceBB))
        {
            srcSquare = BitboardOps.Lsb(pieceBB);

            ulong movesBB = ~myBlockers & *(masksLookup + srcSquare);
            ulong captures = enemyBlockers & movesBB;
            ulong normalMoves = captures ^ movesBB;

            while (BitboardOps.IsNotEmpty(captures))
            {
                trgSquare = BitboardOps.Lsb(captures);
                *cursor++ = BinaryMoveOps.EncodeMove(
                    srcSquare, piece, color, trgSquare, 
                    capture: 1);
                captures = BitboardOps.PopBitAt(captures, trgSquare);
            }

            while (BitboardOps.IsNotEmpty(normalMoves))
            {
                trgSquare = BitboardOps.Lsb(normalMoves);
                *cursor++ = BinaryMoveOps.EncodeMove(
                    srcSquare, piece, color, trgSquare);
                normalMoves = BitboardOps.PopBitAt(normalMoves, trgSquare);
            }

            pieceBB = BitboardOps.PopBitAt(pieceBB, srcSquare);
        }

        return (int)(cursor - dest);
    }

    public static int WriteMovesToSpan(
        ref Position position, 
        int color, 
        int piece,
        ImmutableArray<ulong> masksLookup, 
        Span<int> dest, 
        int offset = 0)
    {
        DebugAsserts.ValidColor(color);
        DebugAsserts.LeapingPiece(piece);
        Debug.Assert(offset >= 0);

        int cursor = offset;

        int enemyColor = Colors.Mirror(color);
        ulong myBlockers = position.blockers[color];
        ulong enemyBlockers = position.blockers[enemyColor];
        ulong pieceBB = position.pieceBBs[Position.BBIndex(piece, color)];

        int srcSquare;
        int trgSquare;

        while (BitboardOps.IsNotEmpty(pieceBB))
        {
            srcSquare = BitboardOps.Lsb(pieceBB); 

            ulong movesBB = ~myBlockers & masksLookup[srcSquare];
            ulong captures = enemyBlockers & movesBB;
            ulong normalMoves = captures ^ movesBB;

            while (BitboardOps.IsNotEmpty(captures))
            {
                trgSquare = BitboardOps.Lsb(captures);
                dest[cursor++] = BinaryMoveOps.EncodeMove(
                    srcSquare, piece, color, trgSquare, 
                    capture: 1);
                captures = BitboardOps.PopBitAt(captures, trgSquare);
            }

            while (BitboardOps.IsNotEmpty(normalMoves))
            {
                trgSquare = BitboardOps.Lsb(normalMoves);
                dest[cursor++] = BinaryMoveOps.EncodeMove(
                    srcSquare, piece, color, trgSquare);
                normalMoves = BitboardOps.PopBitAt(normalMoves, trgSquare);
            }

            pieceBB = BitboardOps.PopBitAt(pieceBB, srcSquare);
        }

        return cursor - offset;
    }
}
