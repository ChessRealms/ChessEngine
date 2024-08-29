using ChessRealms.ChessEngine.Core.Constants;
using ChessRealms.ChessEngine.Core.Types;
using System.Text.RegularExpressions;

namespace ChessRealms.ChessEngine.Parsing;

public static partial class FenStrings
{
    public const string StartPosition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    
    public static bool TryParse(string fen, out Position position)
    {
        position = new();

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
        int squareIndex = Squares.a8;

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
                int color = char.IsUpper(piecePlacementSpan[i]) ? Colors.White : Colors.Black;
                int piece = char.ToLower(piecePlacementSpan[i]) switch
                {
                    'p' => Pieces.Pawn,
                    'n' => Pieces.Knight,
                    'b' => Pieces.Bishop,
                    'r' => Pieces.Rook,
                    'q' => Pieces.Queen,
                    'k' => Pieces.King,
                    _ => Pieces.None
                };

                if (Pieces.IsValid(piece))
                {
                    position.SetPieceAt(squareIndex, piece, color);
                }

                ++squareIndex;
            }
        }
        #endregion

        #region Side To Move
        position.color = sideToMoveSpan.Equals("w", StringComparison.OrdinalIgnoreCase) 
            ? Colors.White
            : Colors.Black;
        #endregion

        #region Castling
        if (castlingSpan.Contains("K", StringComparison.Ordinal))
        {
            position.castlings |= Castlings.WK;
        }

        if (castlingSpan.Contains("Q", StringComparison.Ordinal))
        {
            position.castlings |= Castlings.WQ;
        }

        if (castlingSpan.Contains("k", StringComparison.Ordinal))
        {
            position.castlings |= Castlings.BK;
        }

        if (castlingSpan.Contains("q", StringComparison.Ordinal))
        {
            position.castlings |= Castlings.BQ;
        }
        #endregion

        #region EnPassant & HalfMoveClock & FullMoveNumber
        if (AlgebraicNotation.TryParseSquare(enPassantSpan, out int enPassantSquare))
        {
            position.enpassant = enPassantSquare;
        }
        
        if (int.TryParse(halfMoveClockSpan, out var halfMoveClock))
        {
            position.halfMoveClock = halfMoveClock;
        }
        
        if (int.TryParse(fullMoveNumberSpan, out var fullMoveNumber))
        {
            position.fullMoveCount = fullMoveNumber;
        }
        #endregion

        return true;
    }

    [GeneratedRegex(
        "^(?<PiecePlacement>((?<RankItem>[pnbrqkPNBRQK1-8]{1,8})\\/?){8})\\s+" +
        "(?<SideToMove>b|w)\\s+" +
        "(?<Castling>-|K?Q?k?q?)\\s+" +
        "(?<EnPassant>-|[a-h][36])\\s+" +
        "(?<HalfMoveClock>\\d+)\\s+" +
        "(?<FullMoveNumber>\\d+)\\s*$", RegexOptions.Compiled | RegexOptions.ExplicitCapture)]
    private static partial Regex FenRegex();
}
