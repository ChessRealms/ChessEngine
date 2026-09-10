# Game API status

The deferred game-rules stage is implemented on `feature/complete-game-rules`.
See [the API contracts and support boundaries](game-rules-api.md).

| Previous defect | Resolution and regression coverage |
| --- | --- |
| Promotion suffix lost; several variants applied to one board | Promotion participates in move equality; exact matching applies one variant atomically. Both colors, four pieces, quiet/capture, missing suffix and undo are covered. |
| Default castling rights/clocks incorrect | Starting FEN is exactly the standard position with KQkq, halfmove 0, fullmove 1. All castlings and rights loss on king/rook movement or rook capture are covered. |
| Loaded mate/stalemate not classified; moves accepted after completion | Initialization evaluates endings; every successful move switches side; terminal moves are rejected without mutation. Both colors and undo from mate/draw are covered. |
| Permissive FEN and numeric overflow | Strict structural and local semantic validation precedes generation. Invalid ranks, kings, material, castling and en passant are rejected. BigInteger counters preserve and advance values beyond Int32/Int64 exactly; malformed numbers are rejected. |
| Clocks/history/repetition absent | Counters, successful-move snapshots, independent Clone and exact UndoMove are implemented. Repetition includes only legally available en passant. Claims at 3/50 and automatic endings at 5/75 are distinct, including intended-move claims and mate priority. |
| Unchecked move-buffer writes and unproven capacity 218 | Every move write uses Span bounds checks. The full generator uses the conservative bound 434 for accepted positions; short-buffer sentinel tests cover safety. Position bitboards remain inline values and magic attacks are retained. |

Regression suites: `CompleteGameRulesTests`, `StrictFenTests`, the existing core
tests, and unchanged fast/deep `PerftTests`. Tests also execute deterministic legal
playouts, apply every offered move to independent copies, check bitboard invariants,
and undo to equal full positions and repetition history.

## Remaining rule boundary

Dead-position recognition covers K–K, K+B–K, K+N–K and bishops-only material when
all bishops occupy one square color. It does not prove arbitrary dead positions
or blocked fortresses. Unrecognized dead positions can remain active until
another supported ending condition; lack of a forced mate is never itself used
to award a draw.

FEN validation establishes local invariants, not full historical reachability.
A FEN does not preserve pre-import repetitions or claimed outcomes. Clone preserves
both within a running process.

Tournament procedures, draw agreement, resignation, clocks, SAN/PGN, Chess960,
AI/UCI, networking and UI remain outside this stage. These are support boundaries,
not disabled tests or unimplemented claims of this API.
