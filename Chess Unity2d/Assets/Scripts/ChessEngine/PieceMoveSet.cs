using System.Collections.Generic;

namespace ChessEngine
{
    internal struct PieceMoveSet
    {
        internal readonly List<byte> Moves;

        internal PieceMoveSet(List<byte> moves)
        {
            Moves = moves;
        }
    }
}