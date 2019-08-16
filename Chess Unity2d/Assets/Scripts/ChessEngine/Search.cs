using System.Collections.Generic;

namespace ChessEngine
{
    // ReSharper disable InconsistentNaming
    // ReSharper disable NotAccessedField.Local
    // ReSharper disable CommentTypo
    internal static class Search
    {
        private static int progress;

        private static int piecesRemaining;

        //The Killer Move is a quiet move which caused a beta-cutoff in a sibling Cut-node
        //or any other earlier branch in the tree with the same ply distance to the root
        private static readonly Position[,] KillerMove = new Position[3, 20];
        private static int kIndex;

        private static int Sort(Position s2, Position s1)
        {
            return s1.Score.CompareTo(s2.Score);
        }

        private static int Sort(Board s2, Board s1)
        {
            return s1.Score.CompareTo(s2.Score);
        }

        private static int SideToMoveScore(int score, ChessPieceColor color)
        {
            if (color == ChessPieceColor.Black)
                return -score;

            return score;
        }

        // ReSharper disable IdentifierTypo
        internal static MoveContent IterativeSearch(Board examineBoard,
            byte depth,
            ref int nodesSearched,
            ref int nodesQuiessence,
            ref string pvLine,
            ref byte plyDepthReached,
            ref byte rootMovesSearched,
            List<OpeningMove> currentGameBook)
        {
            var pvChild = new List<Position>();
            var alpha = -400000000;
            const int beta = 400000000;

            var bestMove = new MoveContent();

            //We are going to store our result boards here           
            var succ = GetSortValidMoves(examineBoard);

            rootMovesSearched = (byte) succ.Positions.Count;

            if (rootMovesSearched == 1)
                return succ.Positions[0].LastMove;

            foreach (var pos in succ.Positions)
            {
                var value = -AlphaBeta(pos,
                    1,
                    -beta,
                    -alpha,
                    ref nodesSearched,
                    ref nodesQuiessence,
                    ref pvChild,
                    true);

                //Can I make an instant mate?
                if (value >= 32767)
                    return pos.LastMove;
            }

            var currentBoard = 0;

            alpha = -400000000;

            succ.Positions.Sort(Sort);

            depth--;

            plyDepthReached = ModifyDepth(depth, succ.Positions.Count);

            foreach (var pos in succ.Positions)
            {
                currentBoard++;

                progress = (int) (currentBoard / (decimal) succ.Positions.Count * 100);

                pvChild = new List<Position>();

                var value = -AlphaBeta(pos,
                    depth,
                    -beta,
                    -alpha,
                    ref nodesSearched,
                    ref nodesQuiessence,
                    ref pvChild,
                    false);

                if (value >= 32767)
                    return pos.LastMove;

                if (examineBoard.RepeatedMove == 2)
                {
                    var fen = Board.Fen(true, pos);

                    foreach (var move in currentGameBook)
                        if (move.EndingFEN == fen)
                        {
                            value = 0;
                            break;
                        }
                }

                pos.Score = value;

                //If value is greater then alpha this is the best board
                // ReSharper disable once InvertIf
                if (value > alpha || alpha == -400000000)
                {
                    pvLine = pos.LastMove.ToString();

                    foreach (var pvPos in pvChild)
                        pvLine += " " + pvPos.ToString();

                    alpha = value;
                    bestMove = pos.LastMove;
                }
            }

            plyDepthReached++;
            progress = 100;

            return bestMove;
        }

        private static ResultBoards GetSortValidMoves(Board examineBoard)
        {
            var sortValidMoves = new ResultBoards {Positions = new List<Board>(30)};

            piecesRemaining = 0;

            for (byte x = 0; x < 64; x++)
            {
                var sqr = examineBoard.Squares[x];

                //Make sure there is a piece on the square
                if (sqr.Piece == null)
                    continue;

                piecesRemaining++;

                //Make sure the color is the same color as the one we are moving.
                if (sqr.Piece.PieceColor != examineBoard.WhoseMove)
                    continue;

                //For each valid move for this piece
                foreach (var dst in sqr.Piece.ValidMoves)
                {
                    //We make copies of the board and move so that we can move it without effecting the parent board
                    var board = examineBoard.FastCopy();

                    //Make move so we can examine it
                    Board.MovePiece(board, x, dst, ChessPieceType.Queen);

                    //We Generate Valid Moves for Board
                    PieceValidMoves.GenerateValidMoves(board);

                    //Invalid Move
                    if (board.WhiteCheck && examineBoard.WhoseMove == ChessPieceColor.White) continue;

                    //Invalid Move
                    if (board.BlackCheck && examineBoard.WhoseMove == ChessPieceColor.Black) continue;

                    //We calculate the board score
                    Evaluation.EvaluateBoardScore(board);

                    //Invert Score to support Negamax
                    board.Score = SideToMoveScore(board.Score, board.WhoseMove);

                    sortValidMoves.Positions.Add(board);
                }
            }

            sortValidMoves.Positions.Sort(Sort);
            return sortValidMoves;
        }

        // Not used anymore
        // ReSharper disable once UnusedMember.Local
        private static int MinMax(Board examineBoard, byte depth)
        {
            if (depth == 0)
            {
                //Evaluate Score
                Evaluation.EvaluateBoardScore(examineBoard);
                //Invert Score to support Negamax
                return SideToMoveScore(examineBoard.Score, examineBoard.WhoseMove);
            }

            var positions = EvaluateMoves(examineBoard, depth);

            if (examineBoard.WhiteCheck || examineBoard.BlackCheck || positions.Count == 0)
                if (SearchForMate(examineBoard.WhoseMove, examineBoard, ref examineBoard.BlackMate,
                    ref examineBoard.WhiteMate, ref examineBoard.StaleMate))
                {
                    if (examineBoard.BlackMate)
                    {
                        if (examineBoard.WhoseMove == ChessPieceColor.Black)
                            return -32767 - depth;

                        return 32767 + depth;
                    }

                    // ReSharper disable once InvertIf
                    if (examineBoard.WhiteMate)
                    {
                        if (examineBoard.WhoseMove == ChessPieceColor.Black)
                            return 32767 + depth;

                        return -32767 - depth;
                    }

                    //If Not Mate then StaleMate
                    return 0;
                }

            var bestScore = -32767;

            foreach (var move in positions)
            {
                //Make a copy
                var board = examineBoard.FastCopy();

                //Move Piece
                Board.MovePiece(board, move.SrcPosition, move.DstPosition, ChessPieceType.Queen);

                //We Generate Valid Moves for Board
                PieceValidMoves.GenerateValidMoves(board);

                if (board.BlackCheck)
                    if (examineBoard.WhoseMove == ChessPieceColor.Black)
                        continue;

                if (board.WhiteCheck)
                    if (examineBoard.WhoseMove == ChessPieceColor.White)
                        continue;

                var value = -MinMax(board, (byte) (depth - 1));

                if (value > bestScore) bestScore = value;
            }

            return bestScore;
        }

        // Not used anymore
        // ReSharper disable once UnusedMember.Local
        private static int AlphaBeta(Board examineBoard, byte depth, int alpha, int beta, ref int nodesSearched)
        {
            nodesSearched++;

            if (examineBoard.HalfMoveClock >= 100 || examineBoard.RepeatedMove >= 3)
                return 0;

            if (depth == 0)
            {
                //Evaluate Score
                Evaluation.EvaluateBoardScore(examineBoard);
                //Invert Score to support Negamax
                return SideToMoveScore(examineBoard.Score, examineBoard.WhoseMove);
            }

            var positions = EvaluateMoves(examineBoard, depth);

            if (examineBoard.WhiteCheck || examineBoard.BlackCheck || positions.Count == 0)
                if (SearchForMate(examineBoard.WhoseMove, examineBoard, ref examineBoard.BlackMate,
                    ref examineBoard.WhiteMate, ref examineBoard.StaleMate))
                {
                    if (examineBoard.BlackMate)
                    {
                        if (examineBoard.WhoseMove == ChessPieceColor.Black)
                            return -32767 - depth;

                        return 32767 + depth;
                    }

                    // ReSharper disable once InvertIf
                    if (examineBoard.WhiteMate)
                    {
                        if (examineBoard.WhoseMove == ChessPieceColor.Black)
                            return 32767 + depth;

                        return -32767 - depth;
                    }

                    //If Not Mate then StaleMate
                    return 0;
                }

            positions.Sort(Sort);

            foreach (var move in positions)
            {
                //Make a copy
                var board = examineBoard.FastCopy();

                //Move Piece
                Board.MovePiece(board, move.SrcPosition, move.DstPosition, ChessPieceType.Queen);

                //We Generate Valid Moves for Board
                PieceValidMoves.GenerateValidMoves(board);

                if (board.BlackCheck)
                    if (examineBoard.WhoseMove == ChessPieceColor.Black)
                        continue;

                if (board.WhiteCheck)
                    if (examineBoard.WhoseMove == ChessPieceColor.White)
                        continue;

                var value = -AlphaBeta(board,
                    (byte) (depth - 1),
                    -beta,
                    -alpha,
                    ref nodesSearched);

                if (value >= beta) return beta;
                if (value > alpha) alpha = value;
            }

            return alpha;
        }

        // Final version
        // ReSharper disable once IdentifierTypo
        private static int AlphaBeta(Board examineBoard,
            byte depth,
            int alpha,
            int beta,
            ref int nodesSearched,
            ref int nodesQuiessence,
            ref List<Position> pvLine,
            bool extended)
        {
            nodesSearched++;

            if (examineBoard.HalfMoveClock >= 100 || examineBoard.RepeatedMove >= 3)
                return 0;

            //End Main Search with Quiescence
            if (depth == 0)
            {
                if (!extended && examineBoard.BlackCheck || examineBoard.WhiteCheck)
                {
                    depth++;
                    extended = true;
                }
                else
                {
                    //Perform a Quiessence Search
                    return Quiescence(examineBoard,
                        alpha,
                        beta,
                        ref nodesQuiessence);
                }
            }

            var positions = EvaluateMoves(examineBoard, depth);

            if (examineBoard.WhiteCheck || examineBoard.BlackCheck || positions.Count == 0)
                if (SearchForMate(examineBoard.WhoseMove, examineBoard, ref examineBoard.BlackMate,
                    ref examineBoard.WhiteMate, ref examineBoard.StaleMate))
                {
                    if (examineBoard.BlackMate)
                    {
                        if (examineBoard.WhoseMove == ChessPieceColor.Black)
                            return -32767 - depth;

                        return 32767 + depth;
                    }

                    // ReSharper disable once InvertIf
                    if (examineBoard.WhiteMate)
                    {
                        if (examineBoard.WhoseMove == ChessPieceColor.Black)
                            return 32767 + depth;

                        return -32767 - depth;
                    }

                    //If Not Mate then StaleMate
                    return 0;
                }

            positions.Sort(Sort);

            foreach (var move in positions)
            {
                var pvChild = new List<Position>();

                //Make a copy
                var board = examineBoard.FastCopy();

                //Move Piece
                Board.MovePiece(board, move.SrcPosition, move.DstPosition, ChessPieceType.Queen);

                //We Generate Valid Moves for Board
                PieceValidMoves.GenerateValidMoves(board);

                if (board.BlackCheck)
                    if (examineBoard.WhoseMove == ChessPieceColor.Black)
                        continue;

                if (board.WhiteCheck)
                    if (examineBoard.WhoseMove == ChessPieceColor.White)
                        continue;

                var value = -AlphaBeta(board,
                    (byte) (depth - 1),
                    -beta,
                    -alpha,
                    ref nodesSearched,
                    ref nodesQuiessence,
                    ref pvChild,
                    extended);

                if (value >= beta)
                {
                    KillerMove[kIndex, depth].SrcPosition = move.SrcPosition;
                    KillerMove[kIndex, depth].DstPosition = move.DstPosition;

                    kIndex = (kIndex + 1) % 2;

                    return beta;
                }

                // ReSharper disable once InvertIf
                if (value > alpha)
                {
                    //This nodes is PV-node
                    //That nodes have a score that ends up being inside the window
                    //Between the lower bound Alpha and the upper bound Beta.

                    var pvPos = new Position
                    {
                        SrcPosition = board.LastMove.MovingPiecePrimary.SrcPosition,
                        DstPosition = board.LastMove.MovingPiecePrimary.DstPosition,
                        Move = board.LastMove.ToString()
                    };

                    //All Siblings of a PV-node are expected Cut-nodes
                    pvChild.Insert(0, pvPos);

                    pvLine = pvChild;

                    alpha = value;
                }
            }

            return alpha;
        }

        //Quiescence Search
        private static int Quiescence(Board examineBoard, int alpha, int beta, ref int nodesSearched)
        {
            nodesSearched++;

            //Evaluate Score
            Evaluation.EvaluateBoardScore(examineBoard);

            //Invert Score to support Negamax
            examineBoard.Score = SideToMoveScore(examineBoard.Score, examineBoard.WhoseMove);

            if (examineBoard.Score >= beta)
                return beta;

            if (examineBoard.Score > alpha)
                alpha = examineBoard.Score;

            List<Position> positions;

            if (examineBoard.WhiteCheck || examineBoard.BlackCheck)
                positions = EvaluateMoves(examineBoard, 0);
            else
                positions = EvaluateMovesQ(examineBoard);

            if (positions.Count == 0) return examineBoard.Score;

            positions.Sort(Sort);

            foreach (var move in positions)
            {
                if (StaticExchangeEvaluation(examineBoard.Squares[move.DstPosition]) >= 0)
                    continue;

                //Make a copy
                var board = examineBoard.FastCopy();

                //Move Piece
                Board.MovePiece(board, move.SrcPosition, move.DstPosition, ChessPieceType.Queen);

                //We Generate Valid Moves for Board
                PieceValidMoves.GenerateValidMoves(board);

                if (board.BlackCheck)
                    if (examineBoard.WhoseMove == ChessPieceColor.Black)
                        continue;

                if (board.WhiteCheck)
                    if (examineBoard.WhoseMove == ChessPieceColor.White)
                        continue;

                var value = -Quiescence(board,
                    -beta,
                    -alpha,
                    ref nodesSearched);

                if (value >= beta)
                {
                    KillerMove[2, 0].SrcPosition = move.SrcPosition;
                    KillerMove[2, 0].DstPosition = move.DstPosition;

                    return beta;
                }

                if (value > alpha)
                    alpha = value;
            }

            return alpha;
        }

        // Loops through all of the chess pieces on the board
        // and records the source position
        // and destination position of the move along with its pseudo score
        private static List<Position> EvaluateMoves(Board examineBoard, byte depth)
        {
            //We are going to store our result boards here           
            var positions = new List<Position>();

            //bool foundPV = false;

            for (byte x = 0; x < 64; x++)
            {
                var piece = examineBoard.Squares[x].Piece;

                //Make sure there is a piece on the square
                if (piece == null)
                    continue;

                //Make sure the color is the same color as the one we are moving.
                if (piece.PieceColor != examineBoard.WhoseMove)
                    continue;

                //For each valid move for this piece
                foreach (var dst in piece.ValidMoves)
                {
                    var move = new Position
                    {
                        SrcPosition = x,
                        DstPosition = dst
                    };

                    // killer moves located higher in the list
                    if (move.SrcPosition == KillerMove[0, depth].SrcPosition &&
                        move.DstPosition == KillerMove[0, depth].DstPosition)
                    {
                        //move.TopSort = true;
                        move.Score += 5000;
                        positions.Add(move);
                        continue;
                    }

                    if (move.SrcPosition == KillerMove[1, depth].SrcPosition &&
                        move.DstPosition == KillerMove[1, depth].DstPosition)
                    {
                        //move.TopSort = true;
                        move.Score += 5000;
                        positions.Add(move);
                        continue;
                    }

                    var pieceAttacked = examineBoard.Squares[move.DstPosition].Piece;

                    //If the move is a capture add it's value to the score
                    if (pieceAttacked != null)
                    {
                        move.Score += pieceAttacked.PieceValue;

                        if (piece.PieceValue < pieceAttacked.PieceValue)
                            move.Score += pieceAttacked.PieceValue - piece.PieceValue;
                    }

                    if (!piece.Moved) move.Score += 10;

                    move.Score += piece.PieceActionValue;

                    //Add Score for Castling
                    if (!examineBoard.WhiteCastled && examineBoard.WhoseMove == ChessPieceColor.White)
                    {
                        // ReSharper disable once ConvertIfStatementToSwitchStatement
                        if (piece.PieceType == ChessPieceType.King)
                        {
                            if (move.DstPosition != 62 && move.DstPosition != 58)
                                move.Score -= 40;
                            else
                                move.Score += 40;
                        }

                        if (piece.PieceType == ChessPieceType.Rook) move.Score -= 40;
                    }

                    if (!examineBoard.BlackCastled && examineBoard.WhoseMove == ChessPieceColor.Black)
                    {
                        // ReSharper disable once ConvertIfStatementToSwitchStatement
                        if (piece.PieceType == ChessPieceType.King)
                        {
                            if (move.DstPosition != 6 && move.DstPosition != 2)
                                move.Score -= 40;
                            else
                                move.Score += 40;
                        }

                        if (piece.PieceType == ChessPieceType.Rook) move.Score -= 40;
                    }

                    positions.Add(move);
                }
            }

            return positions;
        }

        // Loops through all of the chess pieces on the board
        // and records the source position
        // and destination position of the move along with its pseudo score
        // optimized for Quiescence search
        private static List<Position> EvaluateMovesQ(Board examineBoard)
        {
            //We are going to store our result boards here           
            var positions = new List<Position>();

            for (byte x = 0; x < 64; x++)
            {
                var piece = examineBoard.Squares[x].Piece;

                //Make sure there is a piece on the square
                if (piece == null)
                    continue;

                //Make sure the color is the same color as the one we are moving.
                if (piece.PieceColor != examineBoard.WhoseMove)
                    continue;

                //For each valid move for this piece
                foreach (var dst in piece.ValidMoves)
                {
                    if (examineBoard.Squares[dst].Piece == null) continue;

                    var move = new Position
                    {
                        SrcPosition = x,
                        DstPosition = dst
                    };

                    if (move.SrcPosition == KillerMove[2, 0].SrcPosition &&
                        move.DstPosition == KillerMove[2, 0].DstPosition)
                    {
                        //move.TopSort = true;
                        move.Score += 5000;
                        positions.Add(move);
                        continue;
                    }

                    var pieceAttacked = examineBoard.Squares[move.DstPosition].Piece;

                    move.Score += pieceAttacked.PieceValue;

                    if (piece.PieceValue < pieceAttacked.PieceValue)
                        move.Score += pieceAttacked.PieceValue - piece.PieceValue;

                    move.Score += piece.PieceActionValue;

                    positions.Add(move);
                }
            }

            return positions;
        }

        // Returns true if a check mate or stalemate is found
        // Values of type of mate and side mated are stored in the three reference variables
        internal static bool SearchForMate(ChessPieceColor movingSide,
            Board examineBoard,
            ref bool blackMate,
            ref bool whiteMate,
            ref bool staleMate)
        {
            var foundNonCheckBlack = false;
            var foundNonCheckWhite = false;

            for (byte x = 0; x < 64; x++)
            {
                var sqr = examineBoard.Squares[x];

                //Make sure there is a piece on the square
                if (sqr.Piece == null)
                    continue;

                //Make sure the color is the same color as the one we are moving.
                if (sqr.Piece.PieceColor != movingSide)
                    continue;

                //For each valid move for this piece
                foreach (var dst in sqr.Piece.ValidMoves)
                {
                    //We make copies of the board and move so that we can move it without effecting the parent board
                    var board = examineBoard.FastCopy();

                    //Make move so we can examine it
                    Board.MovePiece(board, x, dst, ChessPieceType.Queen);

                    //We Generate Valid Moves for Board
                    PieceValidMoves.GenerateValidMoves(board);

                    if (board.BlackCheck == false)
                        foundNonCheckBlack = true;
                    else if (movingSide == ChessPieceColor.Black) continue;

                    if (board.WhiteCheck == false)
                    {
                        foundNonCheckWhite = true;
                    }
                    else if (movingSide == ChessPieceColor.White)
                    {
                    }
                }
            }

            if (foundNonCheckBlack == false)
            {
                if (examineBoard.BlackCheck)
                {
                    blackMate = true;
                    return true;
                }

                if (!examineBoard.WhiteMate && movingSide != ChessPieceColor.White)
                {
                    staleMate = true;
                    return true;
                }
            }

            // ReSharper disable once InvertIf
            if (foundNonCheckWhite == false)
            {
                if (examineBoard.WhiteCheck)
                {
                    whiteMate = true;
                    return true;
                }

                // ReSharper disable once InvertIf
                if (!examineBoard.BlackMate && movingSide != ChessPieceColor.Black)
                {
                    staleMate = true;
                    return true;
                }
            }

            return false;
        }

        // Modify depth search deeper during the end game
        private static byte ModifyDepth(byte depth, int possibleMoves)
        {
            // ReSharper disable once InvertIf
            if (possibleMoves <= 20 || piecesRemaining < 14)
            {
                if (possibleMoves <= 10 || piecesRemaining < 6)
                    depth += 1;

                depth += 1;
            }

            return depth;
        }

        private static int StaticExchangeEvaluation(Square examineSquare)
        {
            if (examineSquare.Piece == null) return 0;
            if (examineSquare.Piece.AttackedValue == 0) return 0;

            return examineSquare.Piece.PieceActionValue - examineSquare.Piece.AttackedValue +
                   examineSquare.Piece.DefendedValue;
        }

        // Struct for stores a List of Positions
        private struct Position
        {
            internal byte SrcPosition;
            internal byte DstPosition;

            internal int Score;

            //internal bool TopSort;
            internal string Move;

            public new string ToString()
            {
                return Move;
            }
        }
    }
}