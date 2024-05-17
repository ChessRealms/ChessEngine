using ChessRealms.ChessEngine;
using ChessRealms.ChessEngine.Console;
using ChessRealms.ChessEngine.Core.Builders;
using ChessRealms.ChessEngine.Core.Constants;
using ChessRealms.ChessEngine.Core.Types;
using ChessRealms.ChessEngine.Parsing;



BinaryMoveBuilder builder = new();

BinaryMove _move = builder.Build();

//builder.WithSourcePiece(ChessConstants.PIECE_ROOK, ChessConstants.COLOR_WHITE);
builder.WithSourcePiece(ChessConstants.PIECE_ROOK, ChessConstants.COLOR_WHITE);

BinaryMove move = builder.Build();

Console.WriteLine(move);