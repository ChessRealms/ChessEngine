using ChessRealms.ChessEngine2.Core.Attacks;
using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.Math;
using ChessRealms.ChessEngine2.Core.Types;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ChessRealms.ChessEngine2.Core.MoveGeneration;

internal static class PawnMovement
{
    public const int HorizontalRotateStep = 8;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong North(ulong bitboard)
    {
        return bitboard << HorizontalRotateStep;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong South(ulong bitboard)
    {
        return bitboard >> HorizontalRotateStep;
    }

    public static unsafe int WriteMovesUnsafe(Position* position, int color, int* dest, int size, int offset)
    {
        int cursor = offset;
        int BBIndex = Position.BBIndex(Pieces.Pawn, color);

        ulong empty = position->blockers[color] ^ SquareMapping.ALL_SQUARES;
        ulong pawns = position->pieceBBs[BBIndex];

        ulong singlePush;
        ulong doublePush;
        ulong promotionRank;

        if (color == Colors.Black)
        {
            singlePush = South(pawns) & empty;
            doublePush = South(singlePush) & empty & SquareMapping.RANK_5;
            promotionRank = SquareMapping.RANK_1;
        }
        else
        {
            singlePush = North(pawns) & empty;
            doublePush = North(singlePush) & empty & SquareMapping.RANK_4;
            promotionRank = SquareMapping.RANK_8;
        }

        while (BitboardOps.IsNotEmpty(singlePush))
        {
            int trgSquare = BitboardOps.Lsb(singlePush);
            singlePush = BitboardOps.PopBitAt(singlePush, trgSquare);
        }

        // TODO:
        return cursor - offset;
    }

    public static unsafe int WriteMovesToSpan(ref Position position, int color, Span<int> moves, int offset)
    {
        Debug.Assert(Colors.IsValid(color));
        Debug.Assert(offset >= 0);
        Debug.Assert(offset < moves.Length);

        int cursor = offset;
        int BBIndex = Position.BBIndex(Pieces.Pawn, color);

        ulong empty = position.blockers[Colors.None] ^ SquareMapping.ALL_SQUARES;
        ulong pawns = position.pieceBBs[BBIndex];

        int enemyColor = Colors.Mirror(color);
        ulong enemyPieces = position.blockers[enemyColor];

        ulong singlePush;
        ulong doublePush;
        ulong promotionRank;

        int stepBack;

        if (color == Colors.Black)
        {
            singlePush = South(pawns) & empty;
            doublePush = South(singlePush) & empty & SquareMapping.RANK_5;
            promotionRank = SquareMapping.RANK_1;
            stepBack = Directions.North;
        }
        else
        {
            singlePush = North(pawns) & empty;
            doublePush = North(singlePush) & empty & SquareMapping.RANK_4;
            promotionRank = SquareMapping.RANK_8;
            stepBack = Directions.South;
        }

        while (BitboardOps.IsNotEmpty(singlePush))
        {
            int trgSquare = BitboardOps.Lsb(singlePush);
            int srcSquare = trgSquare + stepBack;

            if ((SquareOps.ToBitboard(trgSquare) & promotionRank) != 0)
            {
                moves[cursor++] = BinaryMoveOps.EncodeMove(
                    srcSquare, Pieces.Pawn, color, trgSquare,
                    promotion: Promotions.Knight);

                moves[cursor++] = BinaryMoveOps.EncodeMove(
                    srcSquare, Pieces.Pawn, color, trgSquare,
                    promotion: Promotions.Bishop);

                moves[cursor++] = BinaryMoveOps.EncodeMove(
                    srcSquare, Pieces.Pawn, color, trgSquare,
                    promotion: Promotions.Rook);

                moves[cursor++] = BinaryMoveOps.EncodeMove(
                    srcSquare, Pieces.Pawn, color, trgSquare,
                    promotion: Promotions.Queen);
            }
            else
            {
                moves[cursor++] = BinaryMoveOps.EncodeMove(
                    srcSquare, Pieces.Pawn, color, trgSquare);
            }

            singlePush = BitboardOps.PopBitAt(singlePush, trgSquare);
        }

        while (BitboardOps.IsNotEmpty(doublePush))
        {
            int trgSquare = BitboardOps.Lsb(doublePush);
            int srcSquare = trgSquare + (2 * stepBack);

            moves[cursor++] = BinaryMoveOps.EncodeMove(
                srcSquare, Pieces.Pawn, color, trgSquare, 
                doublePush: 1);

            doublePush = BitboardOps.PopBitAt(doublePush, trgSquare);
        }

        if (Squares.IsValid(position.enpassant))
        {
            ulong srcSquares = PawnAttacks.AttackMasks[enemyColor][position.enpassant] 
                & position.pieceBBs[Position.BBIndex(Pieces.Pawn, color)];

            int srcSquare;
            while (BitboardOps.IsNotEmpty(srcSquares))
            {
                srcSquare = BitboardOps.Lsb(srcSquares);

                moves[cursor++] = BinaryMoveOps.EncodeMove(
                    srcSquare, Pieces.Pawn, color, position.enpassant,
                    enpassant: 1, capture: 1);

                srcSquares = BitboardOps.PopBitAt(srcSquares, srcSquare);
            }
        }

        // Generate Captures
        while (BitboardOps.IsNotEmpty(pawns))
        {
            int srcSquare = BitboardOps.Lsb(pawns);
            ulong captures = PawnAttacks.AttackMasks[color][srcSquare] & enemyPieces;

            while (BitboardOps.IsNotEmpty(captures))
            {
                int targetSquare = BitboardOps.Lsb(captures);

                if ((SquareOps.ToBitboard(targetSquare) & promotionRank) != 0)
                {
                    moves[cursor++] = BinaryMoveOps.EncodeMove(
                    srcSquare, Pieces.Pawn, color, targetSquare,
                    capture: 1, promotion: Promotions.Knight);

                moves[cursor++] = BinaryMoveOps.EncodeMove(
                    srcSquare, Pieces.Pawn, color, targetSquare,
                    capture: 1, promotion: Promotions.Bishop);

                moves[cursor++] = BinaryMoveOps.EncodeMove(
                    srcSquare, Pieces.Pawn, color, targetSquare,
                    capture: 1, promotion: Promotions.Rook);

                moves[cursor++] = BinaryMoveOps.EncodeMove(
                    srcSquare, Pieces.Pawn, color, targetSquare,
                    capture: 1, promotion: Promotions.Queen);
                }
                else
                {
                    moves[cursor++] = BinaryMoveOps.EncodeMove(
                        srcSquare, Pieces.Pawn, color, targetSquare, capture: 1);
                }

                BitboardOps.PopBitAt(captures, targetSquare);
            }

            pawns = BitboardOps.PopBitAt(pawns, srcSquare);
        }

        return cursor - offset;
    }
}
