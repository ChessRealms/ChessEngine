namespace ChessRealms.ChessEngine.Core.Types;

/// <summary>
/// Value that represents position at chess-board. Basically it is <see cref="int"/> readonly value.
/// </summary>
/// <param name="square"> Chess board square position. </param>
public readonly struct SquareIndex(int square)
{
    public const int MAX_FILE = 7;
    public const int MAX_RANK = 7;

    /// <summary>
    /// Raw representation of <see cref="SquareIndex"/>. 
    /// It is just <see cref="int"/> value from 0 to 63 (64 square indicies in total).
    /// </summary>
    public readonly int Square = square;

    /// <summary>
    /// Zero-based index of file.
    /// </summary>
    public readonly int File => Square % 8;

    /// <summary>
    /// Zero-based index of rank.
    /// </summary>
    public readonly int Rank => Square / 8;

    /// <summary>
    /// <see cref="BitBoard"/> instance with single bit that corresponds to square at board.
    /// </summary>
    public readonly BitBoard Board => 1UL << Square;

    /// <summary>
    /// Empty <see cref="SquareIndex"/> value.
    /// </summary>
    public static readonly SquareIndex None = -1;

    public SquareIndex() : this(0)
    {
    }

    public SquareIndex(uint square) : this(unchecked((int)square)) 
    {
    }

    public SquareIndex(EnumSquare square) : this((int)square) 
    {
    }

    public override string ToString()
    {
        return string.Format("{0}{1}", (char)('a' + File), (char)('1' + Rank));
    }

    /// <summary>
    /// Parse square from file-rank string.
    /// </summary>
    /// <param name="fileRankString"> Square in file-rank format. Example: <c>"f4"</c>.</param>
    /// <returns> Parsed SquareIndex. </returns>
    public static SquareIndex Parse(ReadOnlySpan<char> fileRankString)
    {
        return FromFileRank(
            file: FromFile(fileRankString[0]),
            rank: FromRank(fileRankString[1]));
    }

    /// <summary>
    /// Try parse square from file-rank string.
    /// </summary>
    /// <param name="fileRankString">Square in file-rank format. Example: <c>"f4"</c>.</param>
    /// <param name="square"> Result of parsing. Writes <see cref="None"/> if parsing failed. </param>
    /// <returns> Returns <see langword="true"/> if parsing is succeed. Otherwise returns <see langword="false"/>. </returns>
    public static bool TryParse(ReadOnlySpan<char> fileRankString, out SquareIndex square)
    {
        if (fileRankString.Length < 2 || !ValidateFile(fileRankString[0]) || !ValidateRank(fileRankString[1]))
        {
            square = None;
            return false;
        }
        
        square = Parse(fileRankString);
        return true;
    }

    /// <summary>
    /// Create SquareIndex from file and rank.
    /// </summary>
    /// <param name="file"> Zero-based 'file' value that is in range from <c>0</c> to <see cref="MAX_FILE"/>. </param>
    /// <param name="rank"> Zero-based 'rank' value that is in range from <c>0</c> to <see cref="MAX_RANK"/>. </param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static SquareIndex FromFileRank(int file, int rank)
    {
        if (!ValidateFileRankIndex(file))
        {
            throw new ArgumentOutOfRangeException(nameof(file), file, $"Invalid 'file' value. File must be in range from 0 to 7.");
        }

        if (!ValidateFileRankIndex(rank))
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
        if (!ValidateFile(file))
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
        if (!ValidateRank(rank))
        {
            throw new ArgumentOutOfRangeException(nameof(rank), rank, "Invalid rank value. Rank must be in range from '1' to '8'.");
        }

        return rank - '1';
    }

    private static bool ValidateFileRankIndex(int fileOrRank)
    {
        return fileOrRank >= 0 && fileOrRank <= MAX_FILE;
    }

    private static bool ValidateFile(char file)
    {
        return file >= 'a' && file <= 'h';
    }

    private static bool ValidateRank(char rank)
    {
        return rank >= '1' && rank <= '8';
    }

    public static implicit operator SquareIndex(int square) => new(square);

    public static implicit operator int(SquareIndex squareIndex) => squareIndex.Square;

    public static implicit operator SquareIndex(EnumSquare square) => new(square);

    public static implicit operator EnumSquare(SquareIndex squareIndex) => unchecked((EnumSquare)squareIndex.Square);

    public static implicit operator uint(SquareIndex squareIndex) => unchecked((uint)squareIndex.Square);

    public static implicit operator SquareIndex(uint squareIndex) => new(squareIndex);
}