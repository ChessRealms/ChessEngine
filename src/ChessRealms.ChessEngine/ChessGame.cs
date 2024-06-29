using ChessRealms.ChessEngine.Core.Constants;
using ChessRealms.ChessEngine.Core.Extensions;
using ChessRealms.ChessEngine.Core.Math;
using ChessRealms.ChessEngine.Core.Movements;
using ChessRealms.ChessEngine.Core.Types;
using ChessRealms.ChessEngine.Parsing;

namespace ChessRealms.ChessEngine;

public unsafe struct ChessGame
{
    private Position position;

    public readonly PieceColor CurrentColor => (PieceColor)position.color;

    public readonly PieceColor EnemyColor => (PieceColor)Colors.Mirror(position.color);

    public bool IsFinished { get; private set; }

    /// <summary>
    /// Creates default chess board with init position setup.
    /// </summary>
    public ChessGame() : this(Position.CreateDefault())
    {
    }

    public ChessGame(Position position)
    {
        this.position = position;
    }

    public readonly void GetBoardToSpan(Span<ChessPiece> destination)
    {
        ulong allBlockers;
        ulong whiteBlockers;

        fixed (Position* position = &this.position)
        {
            allBlockers = position->blockers[BitboardIndicies.AllBlockers];
            whiteBlockers = position->blockers[BitboardIndicies.WBlockers];
        }

        for (int i = 0; i < 64; ++i)
        {
            if (BitboardOps.GetBitAt(allBlockers, i).IsTrue())
            {
                Piece piece;
                if (BitboardOps.GetBitAt(whiteBlockers, i).IsTrue())
                    piece = position.GetPieceAt(square: i, Colors.White);
                else
                    piece = position.GetPieceAt(i, Colors.Black);

                destination[i] = new ChessPiece(
                    (PieceColor)piece.Color, 
                    (PieceValue)piece.Value);
            }
            else
            {
                destination[i] = ChessPiece.Empty;
            }
        }
    }

    public MoveResult MakeMove(int src, int trg) 
    {
        if (!Squares.IsValid(src) || !Squares.IsValid(trg))
        {
            return MoveResult.None;
        }

        Position positionBackup;
        position.CopyTo(&positionBackup);

        int* moves = stackalloc int[218];
        int written;

        fixed (Position* position = &this.position)
            written = MoveGen.WriteMovesToPtrUnsafe(position, position->color, moves);

        int move = BinaryMoveOps.NoneMove;
        for (int i = 0; i < written; ++i)
        {
            int mSrc = BinaryMoveOps.DecodeSrc(moves[i]);
            int mTrg = BinaryMoveOps.DecodeTrg(moves[i]);

            if (src == mSrc && trg == mTrg)
            {
                MoveDriver.MakeMove(ref position, moves[i]);

                if (position.IsKingChecked())
                {
                    fixed (Position* position = &this.position)
                        positionBackup.CopyTo(position);

                    break;
                }

                move = moves[i];
            }
        }

        if (move == BinaryMoveOps.NoneMove)
        {
            return MoveResult.None;
        }

        var moveResult = MoveResult.Move;

        if (BinaryMoveOps.DecodeCapture(move).IsTrue())
        {
            moveResult |= MoveResult.Capture;
        }

        int enemyColor = Colors.Mirror(position.color);
        fixed (Position* position = &this.position)
            written = MoveGen.WriteMovesToPtrUnsafe(position, enemyColor, moves);

        if (!HasMoves(enemyColor))
        {
            IsFinished = true;

            if (position.IsKingChecked(enemyColor))
                moveResult |= MoveResult.Checkmate;
            else
                moveResult |= MoveResult.Stalemate;
        }
        else
        {
            if (position.IsKingChecked(enemyColor))
                moveResult |= MoveResult.Check;

            position.SwitchColor();
        }
        
        return moveResult;
    }

    public bool HasMoves()
    {
        return HasMoves(position.color);
    }

    public bool HasMoves(PieceColor color)
    {
        if (!color.IsBlackOrWhite())
            return false;

        return HasMoves((int)color);
    }

    private bool HasMoves(int color)
    {
        int* moves = stackalloc int[218];
        int written;

        fixed (Position* position = &this.position)
            written = MoveGen.WriteMovesToPtrUnsafe(position, color, moves);

        Position tmpPosition;
        
        for (int i = 0; i < written; ++i)
        {
            position.CopyTo(&tmpPosition);
            MoveDriver.MakeMove(ref tmpPosition, moves[i]);
            
            if (!tmpPosition.IsKingChecked(color))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try create chess game from FEN string. Creates default chess game if FEN is invalid.
    /// </summary>
    /// <param name="fen"> FEN string. </param>
    /// <param name="chessGame"> Created chess game. </param>
    /// <returns></returns>
    public static bool TryCreateFromFen(string fen, out ChessGame chessGame)
    {
        var parsed = FenStrings.TryParse(fen, out Position position);

        if (parsed)
        {
            chessGame = new ChessGame(position);
        }
        else
        {
            chessGame = new ChessGame();
        }

        return parsed;
    }
}
