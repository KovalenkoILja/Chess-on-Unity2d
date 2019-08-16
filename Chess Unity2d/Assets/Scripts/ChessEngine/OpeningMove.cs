using System.Collections.Generic;

namespace ChessEngine
{
    // Used FEN notation to store the chess board history
    // Store the move history 

    // ReSharper disable InconsistentNaming
    // ReSharper disable FieldCanBeMadeReadOnly.Global
    public class OpeningMove
    {
        public string EndingFEN;

        public List<MoveContent> Moves;
        public string StartingFEN;

        public OpeningMove()
        {
            StartingFEN = string.Empty;
            EndingFEN = string.Empty;
            Moves = new List<MoveContent>();
        }
    }
}