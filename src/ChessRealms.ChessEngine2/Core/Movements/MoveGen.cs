using ChessRealms.ChessEngine2.Core.Attacks;
using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.Types;

namespace ChessRealms.ChessEngine2.Core.Movements;

internal static unsafe class MoveGen
{
    public static int WriteMovesToSpan_v1(ref Position position, int color, Span<int> moves, int offset = 0)
    {
        int cursor = offset;

        cursor += PawnMovement.WriteMovesToSpan(ref position, color, moves, cursor);
        
        cursor += LeapingMovement.WriteMovesToSpan(
            ref position, color, Pieces.Knight, 
            KnightAttacks.AttackMasks, moves, cursor);
        
        cursor += SlidingMovement.WriteMovesToSpan(
            ref position, color, Pieces.Bishop,
            BishopAttacks.GetSliderAttack, moves, cursor);

        cursor += SlidingMovement.WriteMovesToSpan(
            ref position, color, Pieces.Rook,
            RookAttacks.GetSliderAttack, moves, cursor);
        
        cursor += SlidingMovement.WriteMovesToSpan(
            ref position, color, Pieces.Queen,
            QueenAttacks.GetSliderAttack, moves, cursor);
        
        cursor += LeapingMovement.WriteMovesToSpan(
            ref position, color, Pieces.King,
            KingAttacks.AttackMasks, moves, cursor);

        cursor += CastlingMovement.WriteMovesToSpan(ref position, color, moves, cursor);

        return cursor - offset;
    }

    public static int WriteMovesToUnsafePtr_v1(Position* position, int color, int* moves, int offset = 0)
    {
        int cursor = offset;

        cursor += PawnMovement.WriteMovesToPtrUnsafe(position, color, moves, cursor);
        
        cursor += LeapingMovement.WriteMovesToPtrUnsafe(
            position, color, Pieces.Knight, 
            KnightAttacks.AttackMasks, moves, cursor);
        
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
            KingAttacks.AttackMasks, moves, cursor);

        cursor += CastlingMovement.WriteMovesToUnsafePtr(position, color, moves, cursor);

        return cursor - offset;
    }
}
