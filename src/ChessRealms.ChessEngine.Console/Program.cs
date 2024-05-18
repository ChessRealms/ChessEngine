using ChessRealms.ChessEngine;
using ChessRealms.ChessEngine.Console;
using ChessRealms.ChessEngine.Core.Builders;
using ChessRealms.ChessEngine.Core.Constants;
using ChessRealms.ChessEngine.Core.Types;
using ChessRealms.ChessEngine.Parsing;

ChessBoard board = new();

FenStrings.TryParse(FenStrings.StartPosition, ref board);

ChessBoard cpy = new();

board.CopyTo(ref cpy);

Print.Board(board);
Print.Board(cpy);