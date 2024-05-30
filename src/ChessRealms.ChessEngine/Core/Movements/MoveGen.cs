using ChessRealms.ChessEngine.Core.Attacks;
using ChessRealms.ChessEngine.Core.Constants;
using ChessRealms.ChessEngine.Core.Types;

namespace ChessRealms.ChessEngine.Core.Movements;

internal static unsafe class MoveGen
{
    public static int WriteMovesToPtrUnsafe(Position* position, int color, int* moves, int offset = 0)
    {
        int cursor = offset;

        cursor += PawnMovement.WriteMovesToPtrUnsafe(position, color, moves, cursor);
        
        cursor += LeapingMovement.WriteMovesToPtrUnsafe(
            position, color, Pieces.Knight, 
            KnightAttacks.AttackMasksPtr, moves, cursor);
        
        cursor += SlidingMovement.WriteMovesToPtrUnsafe(
            position, color, Pieces.Bishop,
            &BishopAttacks.GetSliderAttack, moves, cursor);

        cursor += SlidingMovement.WriteMovesToPtrUnsafe(
            position, color, Pieces.Rook,
            &RookAttacks.GetSliderAttack, moves, cursor);
        
        cursor += SlidingMovement.WriteMovesToPtrUnsafe(
            position, color, Pieces.Queen,
            &QueenAttacks.GetSliderAttack, moves, cursor);
        
        cursor += LeapingMovement.WriteMovesToPtrUnsafe(
            position, color, Pieces.King, 
            KingAttacks.AttackMasksPtr, moves, cursor);

        cursor += CastlingMovement.WriteMovesToUnsafePtr(position, color, moves, cursor);

        return cursor - offset;
    }
}
