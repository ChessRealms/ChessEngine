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
| StartPos_Depth_6 | 3.309 s | 0.0484 s | 0.0429 s |

