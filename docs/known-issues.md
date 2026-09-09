# Deferred game API issues

These scenarios describe missing or incorrect behavior for a later game-state/API stage.
They are not assertions that the current behavior is correct. Perft exercises the core
move generator and does not validate game history or the public promotion API.

## Promotion selection and repeated application

From `7k/P7/8/8/8/8/8/7K w - - 0 1`, request `a7a8q` (and separately
`a7a8n`, `a7a8r`, `a7a8b`). Each should apply exactly one move and leave only
the selected promoted piece on a8. `AlgebraicMove` currently stores only source
and target; parsing ignores the suffix. `ChessGame.MakeMove` matches every
generated promotion with those coordinates and continues applying them without
stopping after the first match. Resolve promotion representation and selection
together; do not bless an arbitrary default or the resulting overlapping pieces.

## Terminal positions and moves after completion

Create from checkmate FEN `7k/6Q1/6K1/8/8/8/8/8 b - - 0 1`, or stalemate FEN
`7k/5Q2/6K1/8/8/8/8/8 b - - 0 1`. `IsFinished` is not initialized from the
position. A future completion model should classify both on creation.

From the initial position play `f2f3 e7e5 g2g4 d8h4` (Fool's Mate), then request
`a7a6`. The mating move sets `IsFinished`, but leaves the current side as Black;
`MakeMove` has no finished-state guard, allowing another Black move. Define the
terminal side-to-move contract and reject subsequent moves without mutation.

## History, repetition and move clocks

From the initial position repeat `g1f3 g8f6 f3g1 f6g8` twice. This produces the
third occurrence of the initial position, but there is no position history or
repetition/claim API. A future implementation must also compare side to move,
castling rights and relevant en-passant rights, not just piece placement.

From `4k3/8/8/8/8/8/8/R3K3 w - - 99 50`, play `a1a2`, then `e8e7`.
The halfmove clock should advance to 100 then 101, and the fullmove number to 51
after Black's move. `MoveDriver` currently updates neither clock. Pawn moves and
captures must reset the halfmove clock. `Position.CreateDefault` also initializes
the clocks to halfmove 1 / fullmove 0 rather than 0 / 1. Implement clock maintenance
with the future history and draw-claim policy; perft deliberately ignores clocks.
