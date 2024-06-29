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
    internal fixed ulong pieceBBs[12];
    internal fixed ulong blockers[3];

    internal int color;
    internal int castlings;
    internal int enpassant;

    internal int fullMoveCount;
    internal int halfMoveClock;

    public Position()
    {
        color = Colors.White;
        castlings = Castlings.None;
        enpassant = Squares.Empty;
        fullMoveCount = 0;
        halfMoveClock = 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SwitchColor() => color = Colors.Mirror(color);

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
        BitboardOps.SetBitAt(ref blockers[BitboardIndicies.AllBlockers], square);
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
        BitboardOps.PopBitAt(ref blockers[BitboardIndicies.AllBlockers], square);
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
        BitboardOps.PopBitAt(ref blockers[BitboardIndicies.AllBlockers], square);
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

    public bool IsKingChecked() => IsKingChecked(color);

    public bool IsKingChecked(int kingColor)
    {
        if (kingColor == Colors.Black)
        {
            int ks = BitboardOps.Lsb(pieceBBs[BitboardIndicies.BKing]);

            return IsSquareAttackedByWhite(ks);
        }
        else
        {
            int ks = BitboardOps.Lsb(pieceBBs[BitboardIndicies.WKing]);

            return IsSquareAttackedByBlack(ks);
        }
    }

    // Should run a bit faster than universal version
    // that calculates Bitboard indicies.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSquareAttackedByWhite(int square)
    {
        return IsAttackedByWhitePawn(square)
            || IsAttackedByWhiteKnight(square)
            || IsAttackedByWhiteBishop(square)
            || IsAttackedByWhiteRook(square)
            || IsAttackedByWhiteKing(square);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSquareAttackedByBlack(int square)
    {
        return IsAttackedByBlackPawn(square)
            || IsAttackedByBlackKnight(square)
            || IsAttackedByBlackBishop(square)
            || IsAttackedByBlackRook(square)
            || IsAttackedByBlackKing(square);
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
        ulong mask = BishopAttacks.GetSliderAttack(square, blockers[BitboardIndicies.AllBlockers]);

        return (mask & enemy).IsTrue();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool IsAttackedByBlackBishop(int square)
    {
        ulong enemy = pieceBBs[BitboardIndicies.BBishop] | pieceBBs[BitboardIndicies.BQueen];
        ulong mask = BishopAttacks.GetSliderAttack(square, blockers[BitboardIndicies.AllBlockers]);

        return (mask & enemy).IsTrue();
    }
    #endregion

    #region Is Attacked By Rook
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool IsAttackedByWhiteRook(int square)
    {
        ulong enemy = pieceBBs[BitboardIndicies.WRook] | pieceBBs[BitboardIndicies.WQueen];
        ulong mask = RookAttacks.GetSliderAttack(square, blockers[BitboardIndicies.AllBlockers]);

        return (mask & enemy).IsTrue();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool IsAttackedByBlackRook(int square)
    {
        ulong enemy = pieceBBs[BitboardIndicies.BRook] | pieceBBs[BitboardIndicies.BQueen];
        ulong mask = RookAttacks.GetSliderAttack(square, blockers[BitboardIndicies.AllBlockers]);

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

    /// <summary>
    /// Creates default position filled with setuped pieces at its default positions.
    /// </summary>
    /// <returns> Default Position. </returns>
    public static Position CreateDefault()
    {
        const ulong bPawns      = SquareMapping.RANK_8 >> 8;
        const ulong bKnights    = (1ul << Squares.b8) | (1ul << Squares.g8);
        const ulong bBishops    = (1ul << Squares.c8) | (1ul << Squares.f8);
        const ulong bRooks      = (1ul << Squares.a8) | (1ul << Squares.h8);
        const ulong bQueen      = 1ul << Squares.d8;
        const ulong bKing       = 1ul << Squares.e8;
        const ulong bAll        = SquareMapping.RANK_8 | bPawns;

        const ulong wPawns      = SquareMapping.RANK_1 << 8;
        const ulong wKnights    = (1ul << Squares.b1) | (1ul << Squares.g1);
        const ulong wBishops    = (1ul << Squares.c1) | (1ul << Squares.f1);
        const ulong wRooks      = (1ul << Squares.a1) | (1ul << Squares.h1);
        const ulong wQueen      = 1ul << Squares.d1;
        const ulong wKing       = 1ul << Squares.e1;
        const ulong wAll        = SquareMapping.RANK_1 | wPawns;

        Position position = new();

        position.pieceBBs[BitboardIndicies.BPawn]   = bPawns;
        position.pieceBBs[BitboardIndicies.BKnight] = bKnights;
        position.pieceBBs[BitboardIndicies.BBishop] = bBishops;
        position.pieceBBs[BitboardIndicies.BRook]   = bRooks;
        position.pieceBBs[BitboardIndicies.BQueen]  = bQueen;
        position.pieceBBs[BitboardIndicies.BKing]   = bKing;

        position.pieceBBs[BitboardIndicies.WPawn]   = wPawns;
        position.pieceBBs[BitboardIndicies.WKnight] = wKnights;
        position.pieceBBs[BitboardIndicies.WBishop] = wBishops;
        position.pieceBBs[BitboardIndicies.WRook]   = wRooks;
        position.pieceBBs[BitboardIndicies.WQueen]  = wQueen;
        position.pieceBBs[BitboardIndicies.WKing]   = wKing;

        position.blockers[BitboardIndicies.BBlockers]   = bAll;
        position.blockers[BitboardIndicies.WBlockers]   = wAll;
        position.blockers[BitboardIndicies.AllBlockers] = bAll | wAll;

        position.color          = Colors.White;
        position.castlings      = Castlings.None;
        position.enpassant      = Squares.Empty;
        position.fullMoveCount  = 0;
        position.halfMoveClock  = 1;

        return position;
    }
}