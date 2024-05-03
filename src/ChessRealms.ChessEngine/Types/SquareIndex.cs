namespace ChessRealms.ChessEngine.Types;

/// <summary>
/// Value that represents position at chess-board. Basically it is <see cref="int"/> readonly value.
/// </summary>
/// <param name="square"> Chess board square position. </param>
public readonly struct SquareIndex(int square)
{
    public const int MAX_FILE = 7;
    public const int MAX_RANK = 7;

    public readonly int Square = square;

    public SquareIndex() : this(0) {}

    public SquareIndex(EnumSquare square) : this((int)square) {}

    /// <summary>
    /// Zero-based index of file.
    /// </summary>
    public readonly int File => Square % 8;

    /// <summary>
    /// Zero-based index of rank.
    /// </summary>
    public readonly int Rank => Square / 8;

    public readonly BitBoard BitBoard => 1UL << Square;

    public static SquareIndex Parse(ReadOnlySpan<char> fileRankString)
    {
        return FromFileRank(
            file: FromFile(fileRankString[0]),
            rank: FromRank(fileRankString[1]));
    }

    /// <summary>
    /// Create SquareIndex from file and rank.
    /// </summary>
    /// <param name="file"> File value that is in range from <c>0</c> to <see cref="MAX_FILE"/>. </param>
    /// <param name="rank"> Rank value that is in range from <c>0</c> to <see cref="MAX_RANK"/>. </param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static SquareIndex FromFileRank(int file, int rank)
    {
        if (file < 0 || file > MAX_FILE)
        {
            throw new ArgumentOutOfRangeException(nameof(file), file, $"Invalid 'file' value. File must be in range from 0 to 7.");
        }

        if (rank < 0 || rank > MAX_RANK)
        {
            throw new ArgumentOutOfRangeException(nameof(rank), rank, $"Invalid 'rank' value. Rank must be in range from 0 to 7");
        }

        return new SquareIndex(rank * 8 + file);
    }

    /// <summary>
    /// Get <see cref="int"/> file value from <see cref="char"/> file value.
    /// </summary>
    /// <param name="file"> File value. </param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static int FromFile(char file)
    {
        if (file < 'a' || file > 'h')
        {
            throw new ArgumentOutOfRangeException(nameof(file), file, "Invalid file value. File must be in range from 'a' to 'h'.");
        }

        return file - 'a';
    }

    /// <summary>
    /// Get <see cref="int"/> rank value from <see cref="char"/> rank value. 
    /// </summary>
    /// <param name="rank"> Rank value. </param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static int FromRank(char rank)
    {
        if (rank < '1' || rank > '8')
        {
            throw new ArgumentOutOfRangeException(nameof(rank), rank, "Invalid rank value. Rank must be in range from '1' to '8'.");
        }

        return rank - '1';
    }

    public static implicit operator SquareIndex(int square) => new(square);

    public static implicit operator int(SquareIndex squareIndex) => squareIndex.Square;

    public static implicit operator SquareIndex(EnumSquare square) => new(square);

    public static implicit operator EnumSquare(SquareIndex squareIndex) => (EnumSquare)squareIndex.Square;
}

public enum EnumSquare : int
{
  a1, b1, c1, d1, e1, f1, g1, h1,
  a2, b2, c2, d2, e2, f2, g2, h2,
  a3, b3, c3, d3, e3, f3, g3, h3,
  a4, b4, c4, d4, e4, f4, g4, h4,
  a5, b5, c5, d5, e5, f5, g5, h5,
  a6, b6, c6, d6, e6, f6, g6, h6,
  a7, b7, c7, d7, e7, f7, g7, h7,
  a8, b8, c8, d8, e8, f8, g8, h8
};

/// <summary>
/// Little-Endian File-Rank Mapping Constants. 
/// See <see href="https://www.chessprogramming.org/Square_Mapping_Considerations"/> for details.
/// </summary>
public static class LerfConstants
{
    public const ulong A_FILE               = 0x0101010101010101;

    public const ulong NOT_A_FILE           = A_FILE ^ ulong.MaxValue;

    public const ulong H_FILE               = 0x8080808080808080;

    public const ulong NOT_H_FILE           = H_FILE ^ ulong.MaxValue;

    public const ulong RANK_1               = 0x00000000000000FF;

    public const ulong RANK_8               = 0xFF00000000000000;

    public const ulong A1_H8_DIAGONAL       = 0x8040201008040201;

    public const ulong H1_A8_ANTIDIAGONAL   = 0x0102040810204080;

    public const ulong LIGHT_SQUARES        = 0x55AA55AA55AA55AA;

    public const ulong DARK_SQUARES         = 0xAA55AA55AA55AA55;
}