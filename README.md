# ChessEngine

Chess engine with bitboard representation with no external dependencies. Only C# and .NET 8.

### To Do:
- [X] Basic bitboard and square management operations.
- [X] Movegen for all moves.
- [X] Parsing FEN to `Position`.
- [X] Simple Perft (no hashtables or parallel calculations).
- [ ] Algebraic notation parsing.
- [ ] PGN (optional).
- [ ] Hashtables for Perft.
- [ ] UCI.
- [ ] Play game functional.


### Perft benchmarks
Current perft benchmarks for _Initial Position_ `rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1` with _depth_ `6`.

- No hashtables.
- No parralel calculations. Everying is in 1 thread.
- `MakeMove`/`UndoMove` with copy `Position` (bitboards) approach.
- _Movegen_ gives pseudolegal moves then check for _IsKingChecked()_ determines if move is legal

See also about Perft: https://www.chessprogramming.org/Perft_Results

Benchmarks collected using `BenchmarkDotNet`.

#### Runned on
`Intel Core i5-8300H CPU 2.30GHz (Coffee Lake), 1 CPU, 8 logical and 4 physical cores`

`.NET SDK 8.0.205`

#### Results

| Method           | Mean    | Error    | StdDev   |
|----------------- |--------:|---------:|---------:|
| StartPos_Depth_6 | 3.564 s | 0.0484 s | 0.0429 s |

_This is average result._
_Sometimes benchmarks could be a bit faster or a bit slower._
_(`~3.443 s` or `~3.613 s`)_

### Example of usage

#### Create chess game

Create start position board.

`ChessGame chessGame = new();`

Or 

```
string fen = "";
_ = ChessGame.TryCreateFromFen(fen, out ChessGame chessGame);
```

#### Make move

```
string moveInput = "a2a4";
var (src, trg) = AlgebraicNotation.ParseMove(inputMove);
MoveResult moveResult = chessGame.MakeMove(src, trg);
Console.WriteLine(moveResult);
// >> Move
// There also could be 'Check', 'Capture', 'Checkmate', 'Stalemate'.
// MoveResult is enums with flags.
```

#### Get Board

```
Span<ChessPiece> pieces = stackalloc ChessPiece[64];
chessGame.GetBoardToSpan(pieces);

Console.WriteLine("{0}, {1}", 
	pieces[0].PieceColor, 
	pieces[0].PieceValue);

// >> Black, Rook
```

Empty squares are equal to `ChessPiece.Empty`.
Or just check it with `piece.IsEmpty()`.
