using ChessRealms.ChessEngine2.Core.Constants;
using ChessRealms.ChessEngine2.Core.Math;
using System.Text;

namespace ChessRealms.ChessEngine2.Debugs;

internal static partial class DebugHelper
{    
    public const string DEBUG = "DEBUG"; 
    
    public static string MoveToString(int move)
    {
        var sb = new StringBuilder();
        
        var srcP = BinaryMoveOps.DecodeSrcPiece(move);
        var srcC = BinaryMoveOps.DecodeSrcColor(move);

        var trg = BinaryMoveOps.DecodeTrg(move);
        var cap = BinaryMoveOps.DecodeCapture(move);

        char piece = srcP switch
        {
            Pieces.Pawn => 'p',
            Pieces.Knight => 'n',
            Pieces.Bishop => 'b',
            Pieces.Rook => 'r',
            Pieces.Queen => 'q',
            Pieces.King => 'k',
            _ => '*'
        };

        if (srcC == Colors.White) piece = char.ToUpper(piece);

        sb.Append(piece);
        
        if (cap == 1) 
            sb.Append('x');

        sb.Append(SquareOps.ToAbbreviature(trg));

        return sb.ToString();
    }
}
