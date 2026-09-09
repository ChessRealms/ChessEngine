# ChessEngine

Chess engine with bitboard representation. The core library has no external package
dependencies. All six projects target .NET 10.

### Build and regression tests

Install the stable [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
version **10.0.401**, as pinned in `global.json`. `rollForward: disable` requires
this exact SDK; `allowPrerelease: false` excludes previews. CI reads the same file.
Run from the repository root:

```sh
dotnet --version
dotnet restore src/ChessRealms.ChessEngine.sln --locked-mode
dotnet build src/ChessRealms.ChessEngine.sln --configuration Release --no-restore
dotnet test src/ChessRealms.ChessEngine.sln --configuration Release --no-build --filter "TestCategory!=Deep"
```

For the ordinary developer loop, `cd src` and run `dotnet test`. This builds and
runs all ordinary tests, including fast perft; no settings file is required.
The solution lives in `src`, so commands from the repository root need its path.
Tests continue to use NUnit 3 through VSTest, explicitly selected in `global.json`.
The existing filters and NUnit `Explicit` behavior are unchanged.

Committed `packages.lock.json` files pin direct and transitive package versions
and content hashes. CI uses `--locked-mode` to reject dependency drift. When
intentionally updating packages, run
`dotnet restore src/ChessRealms.ChessEngine.sln --force-evaluate`, review the
lock-file changes, and repeat the checks above. When updating the SDK, update
`global.json` and this README together, then regenerate/review the lock files with
that SDK. Restore requires access to NuGet.org or a cache containing the locked packages.

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
dotnet test src/ChessRealms.ChessEngine.sln --configuration Release --no-build --filter "TestCategory=Deep"
```

Run both test commands after the Release build, which compiles all tests. Tests,
the console perft runner and benchmarks use the same `PerftDriver` implementation.

GitHub Actions (`.github/workflows/ci.yml`) runs restore, Release build and the
same fast-test command on Windows and Linux with SDK 10.0.401, on pushes and pull requests.
Deep tests are not part of the default CI job. A local pass does not establish
that either GitHub Actions job has passed.

Optional coverage check using the existing VSTest collector:

```sh
dotnet test src/ChessRealms.ChessEngine.sln -c Release --no-build --filter "TestCategory!=Deep" --collect:"XPlat Code Coverage"
```

### Tool smoke checks

After the Release build, run from the repository root:

```sh
dotnet run --project src/ChessRealms.ChessEngine.Perft -c Release --no-build
dotnet run --project src/ChessRealms.MagicBruteforce -c Release --no-build --no-launch-profile -- 42 1
dotnet run --project src/ChessRealms.ChessEngine.Benchmark -c Release --no-build -- --job Dry --inProcess --filter "*StartPos_Depth_6*" --wakeLock None
dotnet run --project src/ChessRealms.ChessEngine.Console -c Release --no-build
```

Perft runs the initial position at depth 6: expect **119,060,324 nodes**.
MagicBruteforce tries one candidate per square/piece type; failure to find magic
numbers with this deliberately tiny budget is expected. In the interactive Console,
enter `e2e4`, then `e7e5`, check the board and side to move, then stop with Ctrl+C.
The benchmark command runs one Dry measurement to check startup and execution;
its result is not a performance comparison. Without arguments, the benchmark keeps
its original full `MediumRun` configuration; explicit arguments use BenchmarkDotNet's
CLI configuration instead.

See [the .NET 10 migration record](docs/dotnet-10-migration.md) for package
compatibility sources, baseline results and validation limits.

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


### Historical perft benchmarks (.NET 8)

These measurements predate the .NET 10 migration and have not been remeasured.
The SDK below describes the historical benchmark environment, not the current build requirement.

Recorded perft benchmarks for _Initial Position_ `rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1` with _depth_ `6`.

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
