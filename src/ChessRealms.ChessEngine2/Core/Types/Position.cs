using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.Math;
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
        Debug.Assert(Squares.IsValid(square));
        Debug.Assert(Colors.IsValid(color));

        int bbIndex     = BBIndex(Pieces.Pawn, color);
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
        Debug.Assert(Squares.IsValid(square));
        Debug.Assert(Pieces.IsValid(piece));
        Debug.Assert(Colors.IsValid(color));
        
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
        Debug.Assert(Squares.IsValid(square));
        Debug.Assert(Pieces.IsValid(piece));
        Debug.Assert(Colors.IsValid(color));

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
        Debug.Assert(Pieces.IsValid(piece));
        Debug.Assert(Colors.IsValid(color));

        return color * 6 + piece;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int PieceFromBBIndex(int bbIndex, int color)
    {
        Debug.Assert(IsValidBBIndex(bbIndex));
        Debug.Assert(Colors.IsValid(color));

        return bbIndex - color * 6;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsValidBBIndex(int bbIndex)
    {
        return bbIndex >= 0 && bbIndex < 12;
    }
}
