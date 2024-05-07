namespace ChessRealms.ChessEngine.Core.Types;

/// <summary>
/// Value that represents position at chess-board. Basically it is <see cref="int"/> readonly value.
/// </summary>
/// <param name="square"> Chess board square position. </param>
public readonly struct SquareIndex(int square)
{
    public const int MAX_FILE = 7;
    public const int MAX_RANK = 7;

    public readonly int Square = square;

    public SquareIndex() : this(0) { }

    public SquareIndex(EnumSquare square) : this((int)square) { }

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

    public static readonly SquareIndex Empty = -1;

    public static implicit operator SquareIndex(int square) => new(square);

    public static implicit operator int(SquareIndex squareIndex) => squareIndex.Square;

    public static implicit operator SquareIndex(EnumSquare square) => new(square);

    public static implicit operator EnumSquare(SquareIndex squareIndex) => (EnumSquare)squareIndex.Square;
}