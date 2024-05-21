﻿using ChessRealms.ChessEngine2.Core.Constants;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ChessRealms.ChessEngine2.Core.Math;

public static class SquareOps
{
    public const int MinFileRank = 0;
    public const int MaxFileRank = 7;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Rank(int square)
    {
        Debug.Assert(Squares.IsValid(square));
        return square / 8;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int File(int square)
    {
        Debug.Assert(Squares.IsValid(square));
        return square % 8;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int FromFileRank(int file, int rank)
    {
        Debug.Assert(ValidateFileRank(file), "Invalid File Value", $"File value was {file}.");
        Debug.Assert(ValidateFileRank(rank), "Invalid Rank Value", $"Rank value was {rank}.");
        return rank * 8 + file;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ValidateFileRank(int fileOrRank)
    {
        return fileOrRank >= MinFileRank && fileOrRank <= MaxFileRank;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ToBitboard(int square)
    {
        Debug.Assert(Squares.IsValid(square));
        return 1ul << square;
    }

    public static string ToAbbriviature(int square)
    {
        Debug.Assert(Squares.IsValid(square));
        int file = File(square);
        int rank = Rank(square);
        return string.Format("{0}{1}", (char)('a' + file), (char)('1' + rank));
    }
}
