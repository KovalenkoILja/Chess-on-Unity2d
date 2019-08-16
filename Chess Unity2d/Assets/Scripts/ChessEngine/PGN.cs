using System;
using System.Collections.Generic;

namespace ChessEngine
{
    // Viewing and Saving Move History in PGN format

    // ReSharper disable InconsistentNaming
    // ReSharper disable UnusedMember.Global
    // ReSharper disable CommentTypo
    // ReSharper disable ClassNeverInstantiated.Global
    public static class PGN
    {
        public enum Result
        {
            White,
            Black,
            Tie,
            Ongoing
        }

        public static string GeneratePGN(Stack<MoveContent> moveHistory,
            string whitePlayer,
            string blackPlayer,
            Result result)
        {
            var count = 0;

            var pgn = "";

            var pgnHeader = "";

            pgnHeader += "[Date \"" + DateTime.Now.Year + "." + DateTime.Now.Month + "." + DateTime.Now.Day + "\"]\r\n";
            pgnHeader += "[White \"" + whitePlayer + "\"]\r\n";
            pgnHeader += "[Black \"" + blackPlayer + "\"]\r\n";

            switch (result)
            {
                case Result.Ongoing:
                    pgnHeader += "[Result \"" + "*" + "\"]\r\n";
                    break;
                case Result.White:
                    pgnHeader += "[Result \"" + "1-0" + "\"]\r\n";
                    break;
                case Result.Black:
                    pgnHeader += "[Result \"" + "0-1" + "\"]\r\n";
                    break;
                case Result.Tie:
                    pgnHeader += "[Result \"" + "1/2-1/2" + "\"]\r\n";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }

            foreach (var move in moveHistory)
            {
                var tmp = "";

                if (move.MovingPiecePrimary.PieceColor == ChessPieceColor.White)
                    tmp += moveHistory.Count / 2 - count + 1 + ". ";

                tmp += move.ToString();
                tmp += " ";

                tmp += pgn;
                pgn = tmp;

                if (move.MovingPiecePrimary.PieceColor == ChessPieceColor.Black) count++;
            }

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (result == Result.White)
                pgn += " 1-0";
            else if (result == Result.Black)
                pgn += " 0-1";
            else if (result == Result.Tie) pgn += " 1/2-1/2";

            return pgnHeader + pgn;
        }
    }
}