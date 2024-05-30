using ChessRealms.ChessEngine.Core.Attacks;
using ChessRealms.ChessEngine.Core.Constants;
using ChessRealms.ChessEngine.Core.Extensions;
using ChessRealms.ChessEngine.Core.Math;
using ChessRealms.ChessEngine.Debugs;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ChessRealms.ChessEngine.Core.Types;

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
        BitboardOps.PopBitAt(ref pieceBBs[i + 5], square);

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

    public bool IsKingChecked(int kingColor)
    {
        if (kingColor == Colors.Black)
        {
            int ks = BitboardOps.Lsb(pieceBBs[BitboardIndicies.BKing]);

            return IsAttackedByWhitePawn(ks)
                || IsAttackedByWhiteKnight(ks)
                || IsAttackedByWhiteBishop(ks)
                || IsAttackedByWhiteRook(ks)
                || IsAttackedByWhiteKing(ks);
        }
        else
        {
            int ks = BitboardOps.Lsb(pieceBBs[BitboardIndicies.WKing]);

            return IsAttackedByBlackPawn(ks)
                || IsAttackedByBlackKnight(ks)
                || IsAttackedByBlackBishop(ks)
                || IsAttackedByBlackRook(ks)
                || IsAttackedByBlackKing(ks);
        }
    }

    // Should run a bit faster than universal version
    // that calculates Bitboard indicies.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSquareAttackedByWhite(int square)
    {
        return IsAttackedByBlackPawn(square)
            || IsAttackedByBlackKnight(square)
            || IsAttackedByBlackBishop(square)
            || IsAttackedByBlackRook(square)
            || IsAttackedByBlackKing(square);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSquareAttackedByBlack(int square)
    {
        return IsAttackedByWhitePawn(square)
            || IsAttackedByWhiteKnight(square)
            || IsAttackedByWhiteBishop(square)
            || IsAttackedByWhiteRook(square)
            || IsAttackedByWhiteKing(square);
    }

    #region Is Attacked By Pawn
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool IsAttackedByWhitePawn(int square)
    {
        ulong enemy = pieceBBs[BitboardIndicies.WPawn];
        ulong mask = PawnAttacks.GetAttackMask(Colors.Black, square);
        return (enemy & mask).IsTrue();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool IsAttackedByBlackPawn(int square)
    {
        ulong enemy = pieceBBs[BitboardIndicies.BPawn];
        ulong mask = PawnAttacks.GetAttackMask(Colors.White, square);
        return (enemy & mask).IsTrue();
    }
    #endregion

    #region Is Attacked By Knight
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool IsAttackedByWhiteKnight(int square)
    {
        ulong enemy = pieceBBs[BitboardIndicies.WKnight];
        ulong mask = KnightAttacks.AttackMasks[square];

        return (enemy & mask).IsTrue();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool IsAttackedByBlackKnight(int square)
    {
        ulong enemy = pieceBBs[BitboardIndicies.BKnight];
        ulong mask = KnightAttacks.AttackMasks[square];

        return (enemy & mask).IsTrue();
    }
    #endregion

    #region Is Attacked By Bishop
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool IsAttackedByWhiteBishop(int square)
    {
        ulong enemy = pieceBBs[BitboardIndicies.WBishop] | pieceBBs[BitboardIndicies.WQueen];
        ulong mask = BishopAttacks.GetSliderAttack(square, blockers[All]);

        return (mask & enemy).IsTrue();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool IsAttackedByBlackBishop(int square)
    {
        ulong enemy = pieceBBs[BitboardIndicies.BBishop] | pieceBBs[BitboardIndicies.BQueen];
        ulong mask = BishopAttacks.GetSliderAttack(square, blockers[All]);

        return (mask & enemy).IsTrue();
    }
    #endregion

    #region Is Attacked By Rook
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool IsAttackedByWhiteRook(int square)
    {
        ulong enemy = pieceBBs[BitboardIndicies.WRook] | pieceBBs[BitboardIndicies.WQueen];
        ulong mask = RookAttacks.GetSliderAttack(square, blockers[All]);

        return (mask & enemy).IsTrue();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool IsAttackedByBlackRook(int square)
    {
        ulong enemy = pieceBBs[BitboardIndicies.BRook] | pieceBBs[BitboardIndicies.BQueen];
        ulong mask = RookAttacks.GetSliderAttack(square, blockers[All]);

        return (mask & enemy).IsTrue();
    }
    #endregion

    #region Is Attacked By King
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool IsAttackedByWhiteKing(int square)
    {
        ulong enemy = pieceBBs[BitboardIndicies.WKing];
        ulong mask = KingAttacks.AttackMasks[square];

        return (enemy & mask).IsTrue();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool IsAttackedByBlackKing(int square)
    {
        ulong enemy = pieceBBs[BitboardIndicies.BKing];
        ulong mask = KingAttacks.AttackMasks[square];

        return (enemy & mask).IsTrue();
    }
    #endregion

    public unsafe void CopyTo(Position* dst)
    {
        fixed (Position* src = &this)
        {
            Buffer.MemoryCopy(src, dst, sizeof(Position), sizeof(Position));
        }
    }
}