using ChessRealms.ChessEngine2.Core.Attacks;
using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.Math;
using ChessRealms.ChessEngine2.Core.Types;
using ChessRealms.ChessEngine2.Debugs;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ChessRealms.ChessEngine2.Core.Movements;

internal unsafe static class PawnMovement
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

    public static int WriteMovesToPtrUnsafe(Position* position, int color, int* dest, int offset = 0)
    {
        DebugHelper.Assert.IsValidColor(color);
        Debug.Assert(offset >= 0);
        
        int cursor = offset;
        int BBIndex = Position.BBIndex(Pieces.Pawn, color);

        ulong empty = ~position->blockers[Colors.None];
        ulong pawns = position->pieceBBs[BBIndex];
        int enpassant = position->enpassant;

        int enemyColor = Colors.Mirror(color);
        ulong enemyPieces = position->blockers[enemyColor];

        ulong singlePush;
        ulong doublePush;
        ulong promotionRank;

        int stepBack;

        if (color == Colors.White)
        {
            singlePush = North(pawns) & empty;
            doublePush = North(singlePush) & empty & SquareMapping.RANK_4;
            promotionRank = SquareMapping.RANK_8;
            stepBack = Directions.South;
        }
        else
        {
            singlePush = South(pawns) & empty;
            doublePush = South(singlePush) & empty & SquareMapping.RANK_5;
            promotionRank = SquareMapping.RANK_1;
            stepBack = Directions.North;
        }

        while (BitboardOps.IsNotEmpty(singlePush))
        {
            int trgSquare = BitboardOps.Lsb(singlePush);
            int srcSquare = trgSquare + stepBack;

            if ((SquareOps.ToBitboard(trgSquare) & promotionRank) != 0)
            {
                dest[cursor++] = BinaryMoveOps.EncodeMove(
                    srcSquare, Pieces.Pawn, color, trgSquare,
                    promotion: Promotions.Knight);

                dest[cursor++] = BinaryMoveOps.EncodeMove(
                    srcSquare, Pieces.Pawn, color, trgSquare,
                    promotion: Promotions.Bishop);

                dest[cursor++] = BinaryMoveOps.EncodeMove(
                    srcSquare, Pieces.Pawn, color, trgSquare,
                    promotion: Promotions.Rook);

                dest[cursor++] = BinaryMoveOps.EncodeMove(
                    srcSquare, Pieces.Pawn, color, trgSquare,
                    promotion: Promotions.Queen);
            }
            else
            {
                dest[cursor++] = BinaryMoveOps.EncodeMove(
                    srcSquare, Pieces.Pawn, color, trgSquare);
            }

            BitboardOps.PopBitAt(ref singlePush, trgSquare);
        }

        while (BitboardOps.IsNotEmpty(doublePush))
        {
            int trgSquare = BitboardOps.Lsb(doublePush);
            int srcSquare = trgSquare + (2 * stepBack);

            dest[cursor++] = BinaryMoveOps.EncodeMove(
                srcSquare, Pieces.Pawn, color, trgSquare, 
                doublePush: 1);

            BitboardOps.PopBitAt(ref doublePush, trgSquare);
        }

        if (Squares.IsValid(enpassant))
        {
            ulong attack = PawnAttacks.AttackMasks[enemyColor][enpassant];
            ulong srcSquares = attack & pawns;

            int srcSquare;
            while (BitboardOps.IsNotEmpty(srcSquares))
            {
                srcSquare = BitboardOps.Lsb(srcSquares);

                dest[cursor++] = BinaryMoveOps.EncodeMove(
                    srcSquare, Pieces.Pawn, color, enpassant,
                    enpassant: 1, capture: 1);

                BitboardOps.PopBitAt(ref srcSquares, srcSquare);
            }
        }

        while (BitboardOps.IsNotEmpty(pawns))
        {
            int srcSquare = BitboardOps.Lsb(pawns);
            ulong attack = PawnAttacks.AttackMasks[color][srcSquare];
            ulong captures = attack & enemyPieces;

            while (BitboardOps.IsNotEmpty(captures))
            {
                int targetSquare = BitboardOps.Lsb(captures);

                if ((SquareOps.ToBitboard(targetSquare) & promotionRank) != 0)
                {
                    dest[cursor++] = BinaryMoveOps.EncodeMove(
                        srcSquare, Pieces.Pawn, color, targetSquare,
                        capture: 1, promotion: Promotions.Knight);

                    dest[cursor++] = BinaryMoveOps.EncodeMove(
                        srcSquare, Pieces.Pawn, color, targetSquare,
                        capture: 1, promotion: Promotions.Bishop);

                    dest[cursor++] = BinaryMoveOps.EncodeMove(
                        srcSquare, Pieces.Pawn, color, targetSquare,
                        capture: 1, promotion: Promotions.Rook);

                    dest[cursor++] = BinaryMoveOps.EncodeMove(
                        srcSquare, Pieces.Pawn, color, targetSquare,
                        capture: 1, promotion: Promotions.Queen);
                }
                else
                {
                    dest[cursor++] = BinaryMoveOps.EncodeMove(
                        srcSquare, Pieces.Pawn, color, targetSquare, capture: 1);
                }

                BitboardOps.PopBitAt(ref captures, targetSquare);
            }

            BitboardOps.PopBitAt(ref pawns, srcSquare);
        }

        return cursor - offset;
    }
}
