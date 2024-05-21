using System.Runtime.CompilerServices;

namespace ChessRealms.ChessEngine2.Core.Constants;

public static class Colors
{
    public const int Black = 0;
    public const int White = 1;
    public const int None = 2;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Mirror(int color)
    {
        return color ^ White;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsValid(int color)
    {
        return color == Black || color == White;
    }
}

public static class Pieces
{
    public const int Pawn = 0;
    public const int Knight = 1;
    public const int Bishop = 2;
    public const int Rook = 3;
    public const int Queen = 4;
    public const int King = 5;
    public const int None = 6;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsValid(int piece)
    {
        return piece >= Pawn && piece < None;
    }
}

public static class Promotions
{
    public const int None = 0;
    public const int Knight = 1;
    public const int Bishop = 2;
    public const int Rook = 3;
    public const int Queen = 4;
}

public static class Castlings
{
    public const int None = 0;
    public const int WK = 1;
    public const int WQ = 2;
    public const int BK = 4;
    public const int BQ = 8;

    public const int White = WK | WQ;
    public const int Black = BK | BQ;

    public const int All = White | Black;
}

public static class Directions
{
    public const int North = 8;
    public const int South = -8;
    public const int West = 1;
    public const int East = -1;

    public const int NorthEast = North + East;
    public const int NorthWest = North + West;

    public const int SouthEast = South + East;
    public const int SouthWest = South + West;
}

public static class Squares
{
    public const int Empty = -1;

    public const int a1 = 0;
    public const int b1 = 1;
    public const int c1 = 2;
    public const int d1 = 3;
    public const int e1 = 4;
    public const int f1 = 5;
    public const int g1 = 6;
    public const int h1 = 7;

    public const int a2 = 0 + 8;
    public const int b2 = 1 + 8;
    public const int c2 = 2 + 8;
    public const int d2 = 3 + 8;
    public const int e2 = 4 + 8;
    public const int f2 = 5 + 8;
    public const int g2 = 6 + 8;
    public const int h2 = 7 + 8;

    public const int a3 = 0 + 8 * 2;
    public const int b3 = 1 + 8 * 2;
    public const int c3 = 2 + 8 * 2;
    public const int d3 = 3 + 8 * 2;
    public const int e3 = 4 + 8 * 2;
    public const int f3 = 5 + 8 * 2;
    public const int g3 = 6 + 8 * 2;
    public const int h3 = 7 + 8 * 2;

    public const int a4 = 0 + 8 * 3;
    public const int b4 = 1 + 8 * 3;
    public const int c4 = 2 + 8 * 3;
    public const int d4 = 3 + 8 * 3;
    public const int e4 = 4 + 8 * 3;
    public const int f4 = 5 + 8 * 3;
    public const int g4 = 6 + 8 * 3;
    public const int h4 = 7 + 8 * 3;

    public const int a5 = 0 + 8 * 4;
    public const int b5 = 1 + 8 * 4;
    public const int c5 = 2 + 8 * 4;
    public const int d5 = 3 + 8 * 4;
    public const int e5 = 4 + 8 * 4;
    public const int f5 = 5 + 8 * 4;
    public const int g5 = 6 + 8 * 4;
    public const int h5 = 7 + 8 * 4;

    public const int a6 = 0 + 8 * 5;
    public const int b6 = 1 + 8 * 5;
    public const int c6 = 2 + 8 * 5;
    public const int d6 = 3 + 8 * 5;
    public const int e6 = 4 + 8 * 5;
    public const int f6 = 5 + 8 * 5;
    public const int g6 = 6 + 8 * 5;
    public const int h6 = 7 + 8 * 5;

    public const int a7 = 0 + 8 * 6;
    public const int b7 = 1 + 8 * 6;
    public const int c7 = 2 + 8 * 6;
    public const int d7 = 3 + 8 * 6;
    public const int e7 = 4 + 8 * 6;
    public const int f7 = 5 + 8 * 6;
    public const int g7 = 6 + 8 * 6;
    public const int h7 = 7 + 8 * 6;

    public const int a8 = 0 + 8 * 7;
    public const int b8 = 1 + 8 * 7;
    public const int c8 = 2 + 8 * 7;
    public const int d8 = 3 + 8 * 7;
    public const int e8 = 4 + 8 * 7;
    public const int f8 = 5 + 8 * 7;
    public const int g8 = 6 + 8 * 7;
    public const int h8 = 7 + 8 * 7;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsValid(int square)
    {
        return square >= a1 && square <= h8;
    }
}