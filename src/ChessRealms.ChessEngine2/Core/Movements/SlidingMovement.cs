using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.Math;
using ChessRealms.ChessEngine2.Core.Types;
using ChessRealms.ChessEngine2.Debugs;
using System.Diagnostics;

namespace ChessRealms.ChessEngine2.Core.Movements;

internal unsafe static class SlidingMovement
{
    public static int WriteMovesToPtrUnsafe(
        Position* position,
        int color,
        int piece,
        delegate*<int, ulong, ulong> getSlidingMaskFunc,
        int* dest,
        int offset = 0)
    {
        DebugHelper.Assert.IsValidColor(color);
        DebugHelper.Assert.IsSlidingPiece(piece);
        Debug.Assert(offset >= 0);

        int cursor = offset;

        int enemyColor = Colors.Mirror(color);
        ulong myBlockers = position->blockers[color];
        ulong enemyBlockers = position->blockers[enemyColor];
        ulong allBlockers = myBlockers | enemyBlockers;
        ulong pieceBB = position->pieceBBs[Position.BBIndex(piece, color)];

        int srcSquare;
        int trgSquare;

        while (BitboardOps.IsNotEmpty(pieceBB))
        {
            srcSquare = BitboardOps.Lsb(pieceBB);

            ulong movesBB = ~myBlockers & getSlidingMaskFunc(srcSquare, allBlockers);
            ulong captures = enemyBlockers & movesBB;
            ulong normalMoves = captures ^ movesBB;

            while (BitboardOps.IsNotEmpty(captures))
            {
                trgSquare = BitboardOps.Lsb(captures);
                dest[cursor++] = BinaryMoveOps.EncodeMove(
                    srcSquare, piece, color, trgSquare, 
                    capture: 1);
                BitboardOps.PopBitAt(ref captures, trgSquare);
            }

            while (BitboardOps.IsNotEmpty(normalMoves))
            {
                trgSquare = BitboardOps.Lsb(normalMoves);
                dest[cursor++] = BinaryMoveOps.EncodeMove(
                    srcSquare, piece, color, trgSquare);
                BitboardOps.PopBitAt(ref normalMoves, trgSquare);
            }

            BitboardOps.PopBitAt(ref pieceBB, srcSquare);
        }

        return cursor - offset;
    }

    public static int WriteMovesToSpan(
        ref Position position,
        int color,
        int piece,
        Func<int, ulong, ulong> getSlidingMaskFunc,
        Span<int> dest,
        int offset = 0)
    {
        DebugHelper.Assert.IsValidColor(color);
        DebugHelper.Assert.IsSlidingPiece(piece);
        Debug.Assert(offset >= 0);

        int cursor = offset;

        int enemyColor = Colors.Mirror(color);
        ulong myBlockers = position.blockers[color];
        ulong enemyBlockers = position.blockers[enemyColor];
        ulong allBlockers = myBlockers | enemyBlockers;
        ulong pieceBB = position.pieceBBs[Position.BBIndex(piece, color)];

        int srcSquare;
        int trgSquare;

        while (BitboardOps.IsNotEmpty(pieceBB))
        {
            srcSquare = BitboardOps.Lsb(pieceBB);

            ulong movesBB = ~myBlockers & getSlidingMaskFunc.Invoke(srcSquare, allBlockers);
            ulong captures = enemyBlockers & movesBB;
            ulong normalMoves = captures ^ movesBB;

            while (BitboardOps.IsNotEmpty(captures))
            {
                trgSquare = BitboardOps.Lsb(captures);
                dest[cursor++] = BinaryMoveOps.EncodeMove(
                    srcSquare, piece, color, trgSquare, 
                    capture: 1);
                
                BitboardOps.PopBitAt(ref captures, trgSquare);
            }

            while (BitboardOps.IsNotEmpty(normalMoves))
            {
                trgSquare = BitboardOps.Lsb(normalMoves);
                dest[cursor++] = BinaryMoveOps.EncodeMove(
                    srcSquare, piece, color, trgSquare);
                
                BitboardOps.PopBitAt(ref normalMoves, trgSquare);
            }

            BitboardOps.PopBitAt(ref pieceBB, srcSquare);
        }

        return cursor - offset;
    }
}
