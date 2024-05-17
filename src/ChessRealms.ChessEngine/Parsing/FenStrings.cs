using ChessRealms.ChessEngine.Core.Types;
using ChessRealms.ChessEngine.Core.Types.Enums;
using System.Text.RegularExpressions;

using static ChessRealms.ChessEngine.Core.Constants.ChessConstants;

namespace ChessRealms.ChessEngine.Parsing;

public static partial class FenStrings
{
    public const string StartPosition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    public const string TrickyPosition = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/Pp2P3/2N2Q1p/1PPBBPPP/R3K2R b KQkq a4 0 1";

    public static bool TryParse(string fen, ref ChessBoard chessBoard)
    {
        Match match = FenRegex().Match(fen);

        if (!match.Success)
        {
            return false;
        }

        var groups = match.Groups;

        ReadOnlySpan<char> piecePlacementSpan   = groups["PiecePlacement"].ValueSpan;
        ReadOnlySpan<char> sideToMoveSpan       = groups["SideToMove"].ValueSpan;
        ReadOnlySpan<char> castlingSpan         = groups["Castling"].ValueSpan;
        ReadOnlySpan<char> enPassantSpan        = groups["EnPassant"].ValueSpan;
        ReadOnlySpan<char> halfMoveClockSpan    = groups["HalfMoveClock"].ValueSpan;
        ReadOnlySpan<char> fullMoveNumberSpan   = groups["FullMoveNumber"].ValueSpan;

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
                int color = char.IsUpper(piecePlacementSpan[i]) ? COLOR_WHITE : COLOR_BLACK;
                int piece = char.ToLower(piecePlacementSpan[i]) switch
                {
                    'p' => PIECE_PAWN,
                    'n' => PIECE_KNIGHT,
                    'b' => PIECE_BISHOP,
                    'r' => PIECE_ROOK,
                    'q' => PIECE_QUEEN,
                    _ => PIECE_KING
                };

                chessBoard.SetPieceAt(squareIndex, new Piece(piece, color));
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
            chessBoard.CastlingState |= Castling.WK;
        }

        if (castlingSpan.Contains("Q", StringComparison.Ordinal))
        {
            chessBoard.CastlingState |= Castling.WQ;
        }

        if (castlingSpan.Contains("k", StringComparison.Ordinal))
        {
            chessBoard.CastlingState |= Castling.BK;
        }

        if (castlingSpan.Contains("q", StringComparison.Ordinal))
        {
            chessBoard.CastlingState |= Castling.BQ;
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
        "(?<Castling>-|K?Q?k?q?)\\s+" +
        "(?<EnPassant>-|[a-h][3-6])\\s+" +
        "(?<HalfMoveClock>\\d+)\\s+" +
        "(?<FullMoveNumber>\\d+)\\s*$", RegexOptions.Compiled | RegexOptions.ExplicitCapture)]
    private static partial Regex FenRegex();
}
