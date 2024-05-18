using ChessRealms.ChessEngine.Core.Types;
using System.Numerics;

namespace ChessRealms.ChessEngine.Core.Extensions;

internal static class BitBoardExtensions
{
    public static ulong GetBitAt(this ulong board, SquareIndex squareIndex)
    {
        return board & (1ul << squareIndex.Value);
    }

    public static bool TryGetBitAt(this ulong board, SquareIndex squareIndex, out ulong bitBoard)
    {
        bitBoard = board & (1ul << squareIndex.Value);
        return bitBoard != 0;
    }

    public static ulong PopBitAt(this ulong board, SquareIndex squareIndex)
    {
        return board ^ (1ul << squareIndex.Value);
    }

    public static bool TryPopBitAt(this ulong board, SquareIndex squareIndex, out ulong bitBoard)
    {
        bitBoard = board ^ (1ul << squareIndex.Value);
        return bitBoard != 0;
    }

    public static ulong PopFirstSquare(this ulong board, out SquareIndex squareIndex)
    {
        if (board == 0)
        {
            squareIndex = SquareIndex.None;
            return board;
        }

        squareIndex = BitOperations.TrailingZeroCount(board);
        return board.PopBitAt(squareIndex);
    }

    public static bool TryPopFirstSquare(this ulong board, out SquareIndex squareIndex, out BitBoard bitBoard)
    {
        if (board == 0)
        {
            squareIndex = SquareIndex.None;
            bitBoard = 0ul;
            return false;
        }

        squareIndex = BitOperations.TrailingZeroCount(board);
        bitBoard = board.PopBitAt(squareIndex);
        return true;
    }

    public static ulong SetBitAt(this ulong board, SquareIndex index)
    {
        return board | index.Board;
    }
}
