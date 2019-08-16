namespace ChessEngine
{
    //This array will hold the number of moves available for every position on the chess board.
    //This is a performance related addition and it allows to replace all foreach loops with regular for loops

    internal struct MoveArrays
    {
        internal static PieceMoveSet[] BishopMoves1;
        internal static byte[] BishopTotalMoves1;

        internal static PieceMoveSet[] BishopMoves2;
        internal static byte[] BishopTotalMoves2;

        internal static PieceMoveSet[] BishopMoves3;
        internal static byte[] BishopTotalMoves3;

        internal static PieceMoveSet[] BishopMoves4;
        internal static byte[] BishopTotalMoves4;

        internal static PieceMoveSet[] BlackPawnMoves;
        internal static byte[] BlackPawnTotalMoves;

        internal static PieceMoveSet[] WhitePawnMoves;
        internal static byte[] WhitePawnTotalMoves;

        internal static PieceMoveSet[] KnightMoves;
        internal static byte[] KnightTotalMoves;

        internal static PieceMoveSet[] QueenMoves1;
        internal static byte[] QueenTotalMoves1;
        internal static PieceMoveSet[] QueenMoves2;
        internal static byte[] QueenTotalMoves2;
        internal static PieceMoveSet[] QueenMoves3;
        internal static byte[] QueenTotalMoves3;
        internal static PieceMoveSet[] QueenMoves4;
        internal static byte[] QueenTotalMoves4;
        internal static PieceMoveSet[] QueenMoves5;
        internal static byte[] QueenTotalMoves5;
        internal static PieceMoveSet[] QueenMoves6;
        internal static byte[] QueenTotalMoves6;
        internal static PieceMoveSet[] QueenMoves7;
        internal static byte[] QueenTotalMoves7;
        internal static PieceMoveSet[] QueenMoves8;
        internal static byte[] QueenTotalMoves8;

        internal static PieceMoveSet[] RookMoves1;
        internal static byte[] RookTotalMoves1;
        internal static PieceMoveSet[] RookMoves2;
        internal static byte[] RookTotalMoves2;
        internal static PieceMoveSet[] RookMoves3;
        internal static byte[] RookTotalMoves3;
        internal static PieceMoveSet[] RookMoves4;
        internal static byte[] RookTotalMoves4;

        internal static PieceMoveSet[] KingMoves;
        internal static byte[] KingTotalMoves;
    }
}