using ChessRealms.ChessEngine2.Core.Attacks;
using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.Math;
using ChessRealms.ChessEngine2.Debugs;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ChessRealms.ChessEngine2.Core.Types;

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
        castlings = Castlings.None;
        enpassant = Squares.Empty;
        halfMoveClock = 1;
    }

    public Piece GetPieceAt(int square, int color)
    {
        DebugAsserts.ValidSquare(square);
        DebugAsserts.ValidColor(color);

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
        DebugAsserts.ValidSquare(square);
        DebugAsserts.ValidPiece(piece);
        DebugAsserts.ValidColor(color);

        int bbIndex = BBIndex(piece, color);
        Debug.Assert(IsValidBBIndex(bbIndex));

        fixed (ulong* pieceBBs = this.pieceBBs)
        {
            pieceBBs[bbIndex] = BitboardOps.SetBitAt(pieceBBs[bbIndex], square);
        }

        fixed (ulong* blockers = this.blockers)
        {
            blockers[color] = BitboardOps.SetBitAt(blockers[color], square);
            blockers[Colors.None] = BitboardOps.SetBitAt(blockers[Colors.None], square);
        }
    }

    public void PopPieceAt(int square, int piece, int color)
    {
        DebugAsserts.ValidSquare(square);
        DebugAsserts.ValidPiece(piece);
        DebugAsserts.ValidColor(color);

        int bbIndex = BBIndex(piece, color);
        Debug.Assert(IsValidBBIndex(bbIndex));

        fixed (ulong* pieceBBs = this.pieceBBs)
        {
            pieceBBs[bbIndex] = BitboardOps.PopBitAt(pieceBBs[bbIndex], square);
        }

        fixed (ulong* blockers = this.blockers)
        {
            blockers[color] = BitboardOps.PopBitAt(blockers[color], square);
            blockers[Colors.None] = BitboardOps.PopBitAt(blockers[Colors.None], square);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int BBIndex(int piece, int color)
    {
        DebugAsserts.ValidPiece(piece);
        DebugAsserts.ValidColor(color);

        return color * 6 + piece;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int PieceFromBBIndex(int bbIndex, int color)
    {
        Debug.Assert(IsValidBBIndex(bbIndex));
        DebugAsserts.ValidColor(color);

        return bbIndex - color * 6;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsValidBBIndex(int bbIndex)
    {
        return bbIndex >= 0 && bbIndex < 12;
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
    private bool IsAttackedByPawn(int square, int enemyColor)
    {
        return (PawnAttacks.GetAttackMask(Colors.Mirror(enemyColor), square)
            & pieceBBs[BBIndex(Pieces.Pawn, enemyColor)]) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsAttackedByKnight(int square, int enemyColor)
    {
        return (KnightAttacks.AttackMasks[square]
            & pieceBBs[BBIndex(Pieces.Knight, enemyColor)]) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsAttackedByBishop(int square, int enemyColor)
    {
        return (BishopAttacks.GetSliderAttack(square, blockers[Colors.None])
            & (pieceBBs[BBIndex(Pieces.Bishop, enemyColor)] 
            | pieceBBs[BBIndex(Pieces.Queen, enemyColor)])) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsAttackedByRook(int square, int enemyColor)
    {
        return (BishopAttacks.GetSliderAttack(square, blockers[Colors.None])
            & (pieceBBs[BBIndex(Pieces.Rook, enemyColor)] 
            | pieceBBs[BBIndex(Pieces.Queen, enemyColor)])) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsAttackedByKing(int square, int enemyColor)
    {
        return (KingAttacks.AttackMasks[square]
            & pieceBBs[BBIndex(Pieces.King, enemyColor)]) != 0;
    }
}