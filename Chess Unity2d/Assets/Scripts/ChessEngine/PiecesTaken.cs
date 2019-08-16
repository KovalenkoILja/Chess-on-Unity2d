namespace ChessEngine
{
    // ReSharper disable NotAccessedField.Global
    public class PiecesTaken
    {
        public int BlackBishop;
        public int BlackKnight;
        public int BlackPawn;
        public int BlackQueen;
        public int BlackRook;
        public int WhiteBishop;
        public int WhiteKnight;
        public int WhitePawn;
        public int WhiteQueen;
        public int WhiteRook;

        public PiecesTaken()
        {
            WhiteQueen = 0;
            WhiteRook = 0;
            WhiteBishop = 0;
            WhiteKnight = 0;
            WhitePawn = 0;
            BlackQueen = 0;
            BlackRook = 0;
            BlackBishop = 0;
            BlackKnight = 0;
            BlackPawn = 0;
        }
    }
}