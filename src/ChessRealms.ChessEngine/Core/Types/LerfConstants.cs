namespace ChessRealms.ChessEngine.Core.Types;

/// <summary>
/// Little-Endian File-Rank Mapping Constants. 
/// See <see href="https://www.chessprogramming.org/Square_Mapping_Considerations"/> for details.
/// </summary>
internal static class LerfConstants
{
    public const int TOTAL_SQUARES = 64;

    public const ulong A_FILE = 0x0101010101010101;

    public const ulong NOT_A_FILE = 0xfefefefefefefefe; // A_FILE ^ ulong.MaxValue;

    public const ulong NOT_AB_FILE = 0xfcfcfcfcfcfcfcfc; // (A_FILE | (A_FILE * 2)) ^ ulong.MaxValue;

    public const ulong H_FILE = 0x8080808080808080;

    public const ulong NOT_H_FILE = 0x7f7f7f7f7f7f7f7f; // H_FILE ^ ulong.MaxValue;

    public const ulong NOT_HG_FILE = 0x3f3f3f3f3f3f3f3f; // (H_FILE | (H_FILE / 2)) ^ ulong.MaxValue;

    public const ulong RANK_1 = 0x00000000000000FF;

    public const ulong RANK_8 = 0xFF00000000000000;

    public const ulong RANK_4 = RANK_1 << 24;

    public const ulong RANK_5 = RANK_8 >> 24;

    public const ulong A1_H8_DIAGONAL = 0x8040201008040201;

    public const ulong H1_A8_ANTIDIAGONAL = 0x0102040810204080;

    public const ulong LIGHT_SQUARES = 0x55AA55AA55AA55AA;

    public const ulong DARK_SQUARES = 0xAA55AA55AA55AA55;

    public const ulong ALL_SQUARES = LIGHT_SQUARES | DARK_SQUARES;
}