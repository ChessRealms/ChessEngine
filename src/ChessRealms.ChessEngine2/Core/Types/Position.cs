﻿using ChessRealms.ChessEngine2.Core.Attacks;
using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.Math;
using ChessRealms.ChessEngine2.Debugs;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ChessRealms.ChessEngine2.Core.Types;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct Position
{
    // To access 'all' blockers.
    private const int All = Colors.None;

    internal fixed ulong pieceBBs[12];
    internal fixed ulong blockers[3];

    internal int color;
    internal int castlings;
    internal int enpassant;

    internal int fullMoveCount;
    internal int halfMoveClock;

    public Position()
    {
        castlings = Castlings.None;
        enpassant = Squares.Empty;
        halfMoveClock = 1;
    }

    public Piece GetPieceAt(int square, int color)
    {
        DebugHelper.Assert.IsValidSquare(square);
        DebugHelper.Assert.IsValidColor(color);

        int bbIndex = BBIndex(Pieces.Pawn, color);
        int bbLastIndex = BBIndex(Pieces.King, color);

        Debug.Assert(IsValidBBIndex(bbIndex));
        Debug.Assert(IsValidBBIndex(bbLastIndex));

        while (bbIndex <= bbLastIndex)
        {
            if (BitboardOps.GetBitAt(pieceBBs[bbIndex], square) != 0)
            {
                int piece = PieceFromBBIndex(bbIndex, color);
                return new Piece(piece, color);
            }

            ++bbIndex;
        }

        return Piece.Empty;
    }

    public void SetPieceAt(int square, int piece, int color)
    {
        DebugHelper.Assert.IsValidSquare(square);
        DebugHelper.Assert.IsValidPiece(piece);
        DebugHelper.Assert.IsValidColor(color);

        int bbIndex = BBIndex(piece, color);
        Debug.Assert(IsValidBBIndex(bbIndex));

        BitboardOps.SetBitAt(ref pieceBBs[bbIndex], square);
        BitboardOps.SetBitAt(ref blockers[color], square);
        BitboardOps.SetBitAt(ref blockers[All], square);
    }

    public void PopPieceAt(int square, int piece, int color)
    {
        DebugHelper.Assert.IsValidSquare(square);
        DebugHelper.Assert.IsValidPiece(piece);
        DebugHelper.Assert.IsValidColor(color);

        int bbIndex = BBIndex(piece, color);
        Debug.Assert(IsValidBBIndex(bbIndex));

        BitboardOps.PopBitAt(ref pieceBBs[bbIndex], square);
        BitboardOps.PopBitAt(ref blockers[color], square);
        BitboardOps.PopBitAt(ref blockers[All], square);
    }

    public void PopPieceAt(int square, int color)
    {
        DebugHelper.Assert.IsValidSquare(square);
        DebugHelper.Assert.IsValidColor(color);

        int i = BBIndex(Pieces.Pawn, color);

        BitboardOps.PopBitAt(ref pieceBBs[i], square);
        BitboardOps.PopBitAt(ref pieceBBs[i + 1], square);
        BitboardOps.PopBitAt(ref pieceBBs[i + 2], square);
        BitboardOps.PopBitAt(ref pieceBBs[i + 3], square);
        BitboardOps.PopBitAt(ref pieceBBs[i + 4], square);

        BitboardOps.PopBitAt(ref blockers[color], square);
        BitboardOps.PopBitAt(ref blockers[All], square);
    }

    public void MovePiece(int srcSquare, int trgSquare, int color, int piece)
    {
        PopPieceAt(srcSquare, piece, color);
        SetPieceAt(trgSquare, piece, color);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int BBIndex(int piece, int color)
    {
        DebugHelper.Assert.IsValidPiece(piece);
        DebugHelper.Assert.IsValidColor(color);

        return (color * 6) + piece;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int PieceFromBBIndex(int bbIndex, int color)
    {
        Debug.Assert(IsValidBBIndex(bbIndex));
        DebugHelper.Assert.IsValidColor(color);

        return bbIndex - color * 6;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsValidBBIndex(int bbIndex)
    {
        return bbIndex >= 0 && bbIndex < 12;
    }

    public bool IsKingChecked(int color)
    {
        int i = BBIndex(Pieces.King, color);
        int square = BitboardOps.Lsb(pieceBBs[i]);
        return IsSquareAttacked(square, Colors.Mirror(color));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSquareAttacked(int square, int enemyColor)
    {
        return IsAttackedByPawn(square, enemyColor)
            || IsAttackedByKnight(square, enemyColor)
            || IsAttackedByBishop(square, enemyColor)
            || IsAttackedByRook(square, enemyColor)
            || IsAttackedByKing(square, enemyColor);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool IsAttackedByPawn(int square, int enemyColor)
    {
        int i = BBIndex(Pieces.Pawn, enemyColor);
        ulong enemy = pieceBBs[i];
        ulong mask = PawnAttacks.GetAttackMask(Colors.Mirror(enemyColor), square);

        return (mask & enemy) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool IsAttackedByKnight(int square, int enemyColor)
    {
        int i = BBIndex(Pieces.Knight, enemyColor);
        ulong enemy = pieceBBs[i];
        ulong mask = KnightAttacks.AttackMasks[square];

        return (mask & enemy) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool IsAttackedByBishop(int square, int enemyColor)
    {
        int bb1 = BBIndex(Pieces.Bishop, enemyColor);
        int bb2 = BBIndex(Pieces.Queen, enemyColor);
        ulong enemy = pieceBBs[bb1] | pieceBBs[bb2];
        ulong mask = BishopAttacks.GetSliderAttack(square, blockers[All]);

        return (mask & enemy) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool IsAttackedByRook(int square, int enemyColor)
    {
        int bb1 = BBIndex(Pieces.Rook, enemyColor);
        int bb2 = BBIndex(Pieces.Queen, enemyColor);
        ulong enemy = pieceBBs[bb1] | pieceBBs[bb2];
        ulong mask = RookAttacks.GetSliderAttack(square, blockers[All]);

        return (mask & enemy) != 0;
    }

    internal bool IsAttackedByKing(int square, int enemyColor)
    {
        int i = BBIndex(Pieces.King, enemyColor);
        ulong enemy = pieceBBs[i];
        ulong mask = KingAttacks.AttackMasks[square];

        return (mask & enemy) != 0;
    }

    public unsafe void CopyTo(Position* dst)
    {
        fixed (Position* src = &this)
        {
            Buffer.MemoryCopy(src, dst, sizeof(Position), sizeof(Position));
        }
        //fixed (ulong* srcPieces = pieceBBs)
        //fixed (ulong* dstPieces = dst.pieceBBs)
        //fixed (ulong* srcBlockers = blockers)
        //fixed (ulong* dstBlockers = dst.blockers)
        //{
        //    Buffer.MemoryCopy(srcPieces, dstPieces, sizeof(ulong) * 12, sizeof(ulong) * 12);
        //    Buffer.MemoryCopy(srcBlockers, dstBlockers, sizeof(ulong) * 3, sizeof(ulong) * 3);

        //    dst.castlings = castlings;
        //    dst.color = color;
        //    dst.enpassant = enpassant;
        //    dst.halfMoveClock = halfMoveClock;
        //    dst.fullMoveCount = fullMoveCount;
        //}
    }
}