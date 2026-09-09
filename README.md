# ChessEngine

Chess engine with bitboard representation with no external dependencies. Only C# and .NET 8.

### Build and regression tests

Install the .NET 8 SDK. Run from the repository root:

```sh
dotnet restore src/ChessRealms.ChessEngine.sln
dotnet build src/ChessRealms.ChessEngine.sln --configuration Release --no-restore
dotnet test src/ChessRealms.ChessEngine.sln --configuration Release --no-build --filter "TestCategory!=Deep"
```

For the ordinary developer loop, `cd src` and run `dotnet test`. This builds and
runs all ordinary tests, including fast perft; no settings file is required.
The solution lives in `src`, so commands from the repository root need its path.

- Ordinary tests cover the existing core/parsing checks, every square for all
  piece/color combinations, and basic public `ChessGame` behavior.
- `Perft` includes seven reference positions at depths 1–3 and assertions for
  special-move counts (castling, en passant and promotions). FEN and expected
  values come from [Chess Programming Wiki](https://www.chessprogramming.org/Perft_Results);
  the source link is also beside the data in `PerftTests.cs`.
- `Deep` contains only additional expensive perft depths: 4–6 for the initial
  position and position 3, and 4–5 for the other positions. These tests also
  carry NUnit `Explicit`, so a plain `dotnet test` does not run them. They are
  opt-in for cost, not failing tests being suppressed. Select them explicitly:

```sh
dotnet test src/ChessRealms.ChessEngine.sln --configuration Release --filter "TestCategory=Deep"
```

Run both commands to check both suites. All tests compile in either run. Tests,
the console perft runner and benchmarks use the same `PerftDriver` implementation.

GitHub Actions (`.github/workflows/ci.yml`) runs restore, Release build and the
same fast-test command on Windows and Linux with .NET 8, on pushes and pull requests.
Deep tests are not part of the default CI job. A local pass does not establish
that either GitHub Actions job has passed.

Deferred promotion, completion and history scenarios are in
[docs/known-issues.md](docs/known-issues.md).

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
AlgebraicMove move = AlgebraicMove.Parse(inputMove);
MoveResult moveResult = chessGame.MakeMove(in move);
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

// >> White, Rook
```

Empty squares are equal to `ChessPiece.Empty`.
Or just check it with `piece.IsEmpty()`.
