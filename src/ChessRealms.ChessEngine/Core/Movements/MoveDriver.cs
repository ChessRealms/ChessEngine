using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.Extensions;
using ChessRealms.ChessEngine2.Core.Math;
using ChessRealms.ChessEngine2.Core.Types;
using ChessRealms.ChessEngine2.Debugs;

namespace ChessRealms.ChessEngine2.Core.Movements;

internal static class MoveDriver
{
    private static readonly int[] CastlingRightsLookup =
    [
        13, 15, 15, 15, 12, 15, 15, 14,
        15, 15, 15, 15, 15, 15, 15, 15,
        15, 15, 15, 15, 15, 15, 15, 15,
        15, 15, 15, 15, 15, 15, 15, 15,
        15, 15, 15, 15, 15, 15, 15, 15,
        15, 15, 15, 15, 15, 15, 15, 15,
        15, 15, 15, 15, 15, 15, 15, 15,
         7, 15, 15, 15,  3, 15, 15, 11
    ];

    public static unsafe void MakeMove(ref Position position, int move)
    {
        position.enpassant = Squares.Empty;

        int castling = BinaryMoveOps.DecodeCastling(move);
        if (castling != Castlings.None)
        {
            DebugHelper.Assert.IsValidSingleCastling(castling);

            switch (castling)
            {
                case Castlings.WK:
                    position.MovePiece(Squares.e1, Squares.g1, Colors.White, Pieces.King);
                    position.MovePiece(Squares.h1, Squares.f1, Colors.White, Pieces.Rook);
                    position.castlings ^= position.castlings & Castlings.White;
                    break;
                case Castlings.WQ:
                    position.MovePiece(Squares.e1, Squares.c1, Colors.White, Pieces.King);
                    position.MovePiece(Squares.a1, Squares.d1, Colors.White, Pieces.Rook);
                    position.castlings ^= position.castlings & Castlings.White;
                    break;
                case Castlings.BK:
                    position.MovePiece(Squares.e8, Squares.g8, Colors.Black, Pieces.King);
                    position.MovePiece(Squares.h8, Squares.f8, Colors.Black, Pieces.Rook);
                    position.castlings ^= position.castlings & Castlings.Black;
                    break;
                case Castlings.BQ:
                    position.MovePiece(Squares.e8, Squares.c8, Colors.Black, Pieces.King);
                    position.MovePiece(Squares.a8, Squares.d8, Colors.Black, Pieces.Rook);
                    position.castlings ^= position.castlings & Castlings.Black;
                    break;
            }
        }
        else if (BinaryMoveOps.DecodeEnpassant(move) != 0)
        {
            int src = BinaryMoveOps.DecodeSrc(move);
            int trg = BinaryMoveOps.DecodeTrg(move);
            int srcColor = BinaryMoveOps.DecodeSrcColor(move);

            position.MovePiece(src, trg, srcColor, Pieces.Pawn);

            int enemyPawnSquare = srcColor == Colors.White
                ? trg + Directions.South
                : trg + Directions.North;

            position.PopPieceAt(enemyPawnSquare, Pieces.Pawn, Colors.Mirror(srcColor));
        }
        else if (BinaryMoveOps.DecodeDoublePush(move) != 0)
        {
            int src = BinaryMoveOps.DecodeSrc(move);
            int trg = BinaryMoveOps.DecodeTrg(move);
            int srcColor = BinaryMoveOps.DecodeSrcColor(move);

            position.MovePiece(src, trg, srcColor, Pieces.Pawn);

            #region Set EP
            ulong enemyNeighborPawns;
            int stepToBack;

            if (srcColor == Colors.White)
            {
                enemyNeighborPawns = position.pieceBBs[0] & SquareMapping.RANK_4;
                stepToBack = Directions.South;
            }
            else
            {
                enemyNeighborPawns = position.pieceBBs[6] & SquareMapping.RANK_5;
                stepToBack = Directions.North;
            }

            ulong epMask = SquareOps.ToBitboard(trg - 1) | SquareOps.ToBitboard(trg + 1);

            if ((epMask & enemyNeighborPawns) != 0)
            {
                position.enpassant = trg + stepToBack;
            }
            #endregion
        }
        else
        {
            int src = BinaryMoveOps.DecodeSrc(move);
            int trg = BinaryMoveOps.DecodeTrg(move);
            int srcColor = BinaryMoveOps.DecodeSrcColor(move);
            int srcPiece = BinaryMoveOps.DecodeSrcPiece(move);
            int capture = BinaryMoveOps.DecodeCapture(move);

            if (capture.IsTrue())
            {
                position.PopPieceAt(trg, Colors.Mirror(srcColor));
            }

            position.MovePiece(src, trg, srcColor, srcPiece);

            if (srcPiece == Pieces.Pawn)
            {
                int promotion = BinaryMoveOps.DecodePromotion(move);
                if (promotion != Promotions.None)
                {
                    position.PopPieceAt(trg, srcPiece, srcColor);
                    position.SetPieceAt(trg, promotion, srcColor);
                }
            }

            position.castlings &= CastlingRightsLookup[src];
            position.castlings &= CastlingRightsLookup[trg];
        }
    }
}
