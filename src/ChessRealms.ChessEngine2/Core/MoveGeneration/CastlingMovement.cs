using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.Math;
using ChessRealms.ChessEngine2.Core.Types;

namespace ChessRealms.ChessEngine2.Core.MoveGeneration;

internal static unsafe class CastlingMovement
{
    public static int WriteMovesToUnsafePtr(Position* position, int color, int* dest, int offset = 0)
    {
        int* cursor = dest;

        if (color == Colors.Black)
        {
            bool BK_CastlingAvailable = (position->castlings & Castlings.BK) != 0
                && BitboardOps.GetBitAt(position->blockers[Colors.None], Squares.f8) == 0
                && BitboardOps.GetBitAt(position->blockers[Colors.None], Squares.g8) == 0
                && !position->IsSquareAttacked(Squares.e8, Colors.White)
                && !position->IsSquareAttacked(Squares.f8, Colors.White)
                && !position->IsSquareAttacked(Squares.g8, Colors.White);

            if (BK_CastlingAvailable)
            {
                *cursor++ = BinaryMoveOps.EncodeMove(
                    Squares.e8, Pieces.King, Colors.Black, Squares.g8,
                    castling: Castlings.BK);
            }

            bool BQ_CastlingAvailable = (position->castlings & Castlings.BQ) != 0
                && BitboardOps.GetBitAt(position->blockers[Colors.None], Squares.b8) == 0
                && BitboardOps.GetBitAt(position->blockers[Colors.None], Squares.c8) == 0
                && BitboardOps.GetBitAt(position->blockers[Colors.None], Squares.d8) == 0
                && !position->IsSquareAttacked(Squares.e8, Colors.White)
                && !position->IsSquareAttacked(Squares.d8, Colors.White)
                && !position->IsSquareAttacked(Squares.c8, Colors.White);

            if (BQ_CastlingAvailable)
            {
                *cursor++ = BinaryMoveOps.EncodeMove(
                    Squares.e8, Pieces.King, Colors.Black, Squares.c8,
                    castling: Castlings.BQ);
            }
        }
        else
        {
            bool WK_CastlingAvailable = (position->castlings & Castlings.WK) != 0
                && BitboardOps.GetBitAt(position->blockers[Colors.None], Squares.f1) == 0
                && BitboardOps.GetBitAt(position->blockers[Colors.None], Squares.g1) == 0
                && !position->IsSquareAttacked(Squares.e1, Colors.Black)
                && !position->IsSquareAttacked(Squares.f1, Colors.Black)
                && !position->IsSquareAttacked(Squares.g1, Colors.Black);

            if (WK_CastlingAvailable)
            {
                *cursor++ = BinaryMoveOps.EncodeMove(
                    Squares.e1, Pieces.King, Colors.White, Squares.g1,
                    castling: Castlings.WK);
            }

            bool WQ_CastlingAvailable = (position->castlings & Castlings.WQ) != 0
                && BitboardOps.GetBitAt(position->blockers[Colors.None], Squares.b1) == 0
                && BitboardOps.GetBitAt(position->blockers[Colors.None], Squares.c1) == 0
                && BitboardOps.GetBitAt(position->blockers[Colors.None], Squares.d1) == 0
                && !position->IsSquareAttacked(Squares.e1, Colors.Black)
                && !position->IsSquareAttacked(Squares.d1, Colors.Black)
                && !position->IsSquareAttacked(Squares.c1, Colors.Black);

            if (WQ_CastlingAvailable)
            {
                *cursor++ = BinaryMoveOps.EncodeMove(
                    Squares.e1, Pieces.King, Colors.White, Squares.c1,
                    castling: Castlings.WQ);
            }
        }

        return (int)(cursor - dest);
    }

    public static int WriteMovesToSpan(ref Position position, int color, Span<int> dest, int offset = 0)
    {
        int cursor = offset;

        if (color == Colors.Black)
        {
            bool BK_CastlingAvailable = (position.castlings & Castlings.BK) != 0
                && BitboardOps.GetBitAt(position.blockers[Colors.None], Squares.f8) == 0
                && BitboardOps.GetBitAt(position.blockers[Colors.None], Squares.g8) == 0
                && !position.IsSquareAttacked(Squares.e8, Colors.White)
                && !position.IsSquareAttacked(Squares.f8, Colors.White)
                && !position.IsSquareAttacked(Squares.g8, Colors.White);

            if (BK_CastlingAvailable)
            {
                dest[cursor++] = BinaryMoveOps.EncodeMove(
                    Squares.e8, Pieces.King, Colors.Black, Squares.g8,
                    castling: Castlings.BK);
            }

            bool BQ_CastlingAvailable = (position.castlings & Castlings.BQ) != 0
                && BitboardOps.GetBitAt(position.blockers[Colors.None], Squares.b8) == 0
                && BitboardOps.GetBitAt(position.blockers[Colors.None], Squares.c8) == 0
                && BitboardOps.GetBitAt(position.blockers[Colors.None], Squares.d8) == 0
                && !position.IsSquareAttacked(Squares.e8, Colors.White)
                && !position.IsSquareAttacked(Squares.d8, Colors.White)
                && !position.IsSquareAttacked(Squares.c8, Colors.White);

            if (BQ_CastlingAvailable)
            {
                dest[cursor++] = BinaryMoveOps.EncodeMove(
                    Squares.e8, Pieces.King, Colors.Black, Squares.c8,
                    castling: Castlings.BQ);
            }
        }
        else
        {
            bool WK_CastlingAvailable = (position.castlings & Castlings.WK) != 0
                && BitboardOps.GetBitAt(position.blockers[Colors.None], Squares.f1) == 0
                && BitboardOps.GetBitAt(position.blockers[Colors.None], Squares.g1) == 0
                && !position.IsSquareAttacked(Squares.e1, Colors.Black)
                && !position.IsSquareAttacked(Squares.f1, Colors.Black)
                && !position.IsSquareAttacked(Squares.g1, Colors.Black);

            if (WK_CastlingAvailable)
            {
                dest[cursor++] = BinaryMoveOps.EncodeMove(
                    Squares.e1, Pieces.King, Colors.White, Squares.g1,
                    castling: Castlings.WK);
            }

            bool WQ_CastlingAvailable = (position.castlings & Castlings.WQ) != 0
                && BitboardOps.GetBitAt(position.blockers[Colors.None], Squares.b1) == 0
                && BitboardOps.GetBitAt(position.blockers[Colors.None], Squares.c1) == 0
                && BitboardOps.GetBitAt(position.blockers[Colors.None], Squares.d1) == 0
                && !position.IsSquareAttacked(Squares.e1, Colors.Black)
                && !position.IsSquareAttacked(Squares.d1, Colors.Black)
                && !position.IsSquareAttacked(Squares.c1, Colors.Black);

            if (WQ_CastlingAvailable)
            {
                dest[cursor++] = BinaryMoveOps.EncodeMove(
                    Squares.e1, Pieces.King, Colors.White, Squares.c1,
                    castling: Castlings.WQ);
            }
        }

        return cursor - offset;
    }
}
