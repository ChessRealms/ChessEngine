using ChessRealms.ChessEngine.Core.Types;
using System.Text.RegularExpressions;

namespace ChessRealms.ChessEngine.Parsing;

public static partial class FenStrings
{
    public static bool TryParse(string fen, out ChessBoard chessBoard)
    {
        Match match = FenRegex().Match(fen);

        if (!match.Success)
        {
            chessBoard = new();
            return false;
        }

        var groups = match.Groups;

        ReadOnlySpan<char> piecePlacement   = groups["PiecePlacement"].ValueSpan;
        ReadOnlySpan<char> sideToMove       = groups["SideToMove"].ValueSpan;
        ReadOnlySpan<char> castling         = groups["Castling"].ValueSpan;
        ReadOnlySpan<char> enPassant        = groups["EnPassant"].ValueSpan;
        ReadOnlySpan<char> halfMoveClock    = groups["HalfMoveClock"].ValueSpan;
        ReadOnlySpan<char> fullMoveNumber   = groups["FullMoveNumber"].ValueSpan;

        chessBoard = new ChessBoard();

        #region Read pieces
        SquareIndex squareIndex = EnumSquare.h8;

        for (int i = 0; i < piecePlacement.Length && squareIndex >= 0; ++i)
        {
            if (!char.IsLetterOrDigit(piecePlacement[i]))
            {
                continue;
            }

            if (char.IsDigit(piecePlacement[i]))
            {
                int spaces = (int) char.GetNumericValue(piecePlacement[i]);
                squareIndex -= spaces;
            }
            else
            {
                PieceColor color = char.IsUpper(piecePlacement[i]) ? PieceColor.White : PieceColor.Black;
                PieceType piece = char.ToLower(piecePlacement[i]) switch
                {
                    'p' => PieceType.Pawn,
                    'n' => PieceType.Knight,
                    'b' => PieceType.Bishop,
                    'r' => PieceType.Rook,
                    'q' => PieceType.Queen,
                    _ => PieceType.King
                };

                chessBoard.SetPieceAt(squareIndex, color, piece);
                --squareIndex;
            }
        }
        #endregion

        #region Side To Move
        chessBoard.CurrentColor = sideToMove.Equals("w", StringComparison.OrdinalIgnoreCase) 
            ? PieceColor.White
            : PieceColor.Black;
        #endregion

        #region Castling
        if (castling.Contains("K", StringComparison.Ordinal))
        {
            chessBoard.Castling |= Castling.WK;
        }

        if (castling.Contains("Q", StringComparison.Ordinal))
        {
            chessBoard.Castling |= Castling.WQ;
        }

        if (castling.Contains("k", StringComparison.Ordinal))
        {
            chessBoard.Castling |= Castling.BK;
        }

        if (castling.Contains("q", StringComparison.Ordinal))
        {
            chessBoard.Castling |= Castling.BQ;
        }
        #endregion

        #region EnPassant & HalfMoveClock & FullMoveNumber
        if (enPassant.Length == 2)
        {
            chessBoard.Enpassant = SquareIndex.Parse(enPassant);
        }

        chessBoard.HalfMoveClock = int.Parse(halfMoveClock);
        chessBoard.FullMoveNumber = int.Parse(fullMoveNumber);
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
