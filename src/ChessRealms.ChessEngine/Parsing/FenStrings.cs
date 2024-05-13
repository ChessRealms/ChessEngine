using ChessRealms.ChessEngine.Core.Types;
using System.Text.RegularExpressions;

namespace ChessRealms.ChessEngine.Parsing;

public static partial class FenStrings
{
#if DEBUG
    public const string StartPosition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    public const string TrickyPosition = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/Pp2P3/2N2Q1p/1PPBBPPP/R3K2R b KQkq a4 0 1";
#endif

    public static bool TryParse(string fen, out ChessBoard chessBoard)
    {
        Match match = FenRegex().Match(fen);

        if (!match.Success)
        {
            chessBoard = new();
            return false;
        }

        var groups = match.Groups;

        ReadOnlySpan<char> piecePlacementSpan   = groups["PiecePlacement"].ValueSpan;
        ReadOnlySpan<char> sideToMoveSpan       = groups["SideToMove"].ValueSpan;
        ReadOnlySpan<char> castlingSpan         = groups["Castling"].ValueSpan;
        ReadOnlySpan<char> enPassantSpan        = groups["EnPassant"].ValueSpan;
        ReadOnlySpan<char> halfMoveClockSpan    = groups["HalfMoveClock"].ValueSpan;
        ReadOnlySpan<char> fullMoveNumberSpan   = groups["FullMoveNumber"].ValueSpan;

        chessBoard = new ChessBoard();

        #region Read pieces
        // Fen string represented as piece in next positions a8-h8/a7-h8/.../a1-h1.
        // So we setup a8 as start index.
        SquareIndex squareIndex = EnumSquare.a8;

        for (int i = 0; i < piecePlacementSpan.Length; ++i)
        {
            if (!char.IsLetterOrDigit(piecePlacementSpan[i]))
            {
                // Move to next rank. '-16' insted of '-8' related to ordering
                // from 'a' to 'h' and next increments of 'squareIndex'.
                squareIndex -= 16;
                continue;
            }

            if (char.IsDigit(piecePlacementSpan[i]))
            {
                int spaces = (int) char.GetNumericValue(piecePlacementSpan[i]);
                squareIndex += spaces;
            }
            else
            {
                PieceColor color = char.IsUpper(piecePlacementSpan[i]) ? PieceColor.White : PieceColor.Black;
                PieceType piece = char.ToLower(piecePlacementSpan[i]) switch
                {
                    'p' => PieceType.Pawn,
                    'n' => PieceType.Knight,
                    'b' => PieceType.Bishop,
                    'r' => PieceType.Rook,
                    'q' => PieceType.Queen,
                    _ => PieceType.King
                };

                chessBoard.SetPieceAt(squareIndex, color, piece);
                ++squareIndex;
            }
        }
        #endregion

        #region Side To Move
        chessBoard.CurrentColor = sideToMoveSpan.Equals("w", StringComparison.OrdinalIgnoreCase) 
            ? PieceColor.White
            : PieceColor.Black;
        #endregion

        #region Castling
        if (castlingSpan.Contains("K", StringComparison.Ordinal))
        {
            chessBoard.Castling |= Castling.WK;
        }

        if (castlingSpan.Contains("Q", StringComparison.Ordinal))
        {
            chessBoard.Castling |= Castling.WQ;
        }

        if (castlingSpan.Contains("k", StringComparison.Ordinal))
        {
            chessBoard.Castling |= Castling.BK;
        }

        if (castlingSpan.Contains("q", StringComparison.Ordinal))
        {
            chessBoard.Castling |= Castling.BQ;
        }
        #endregion

        #region EnPassant & HalfMoveClock & FullMoveNumber
        if (SquareIndex.TryParse(enPassantSpan, out var enPassantSquare))
        {
            chessBoard.Enpassant = enPassantSquare;
        }
        
        if (int.TryParse(halfMoveClockSpan, out var halfMoveClock))
        {
            chessBoard.HalfMoveClock = halfMoveClock;
        }
        
        if (int.TryParse(fullMoveNumberSpan, out var fullMoveNumber))
        {
            chessBoard.FullMoveNumber = fullMoveNumber;
        }
        #endregion

        return true;
    }

    [GeneratedRegex(
        "^(?<PiecePlacement>((?<RankItem>[pnbrqkPNBRQK1-8]{1,8})\\/?){8})\\s+" +
        "(?<SideToMove>b|w)\\s+" +
        "(?<Castling>-|K?Q?k?q)\\s+" +
        "(?<EnPassant>-|[a-h][3-6])\\s+" +
        "(?<HalfMoveClock>\\d+)\\s+" +
        "(?<FullMoveNumber>\\d+)\\s*$", RegexOptions.Compiled | RegexOptions.ExplicitCapture)]
    private static partial Regex FenRegex();
}
