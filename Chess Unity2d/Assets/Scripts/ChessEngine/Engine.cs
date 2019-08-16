using System.Collections.Generic;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable CommentTypo
// ReSharper disable NotAccessedField.Global
namespace ChessEngine
{
    public sealed class Engine
    {
        #region Search

        public void AiPonderMove()
        {
            Thinking = true;

            ChessBoard.BlackMate = false;
            ChessBoard.WhiteMate = false;

            PieceValidMoves.GenerateValidMoves(ChessBoard);

            NodesSearched = 0;

            var resultBoards = new ResultBoards
            {
                Positions = new List<Board>()
            };

            // First search if human move might have caused a mate 
            if (CheckForMate(WhoseMove, ref ChessBoard))
            {
                Thinking = false;
                return;
            }

            var bestMove = new MoveContent();

            //If there is no playbook move search for the best move
            if (FindPlayBookMove(ref bestMove, ChessBoard, OpeningBook) == false
                || ChessBoard.HalfMoveClock > 90 
                || ChessBoard.RepeatedMove >= 2)
                if (FindPlayBookMove(ref bestMove, ChessBoard, CurrentGameBook) == false 
                    || ChessBoard.HalfMoveClock > 90 
                    || ChessBoard.RepeatedMove >= 2)
                    bestMove = Search.IterativeSearch(
                        ChessBoard,
                        PlyDepthSearched,
                        ref NodesSearched,
                        ref NodesQuiessence,
                        ref pvLine,
                        ref PlyDepthReached,
                        ref RootMovesSearched,
                        CurrentGameBook);

            //Make the move 
            PreviousChessBoard = new Board(ChessBoard);

            RootMovesSearched = (byte) resultBoards.Positions.Count;

            Board.MovePiece(ChessBoard,
                bestMove.MovingPiecePrimary.SrcPosition,
                bestMove.MovingPiecePrimary.DstPosition,
                ChessPieceType.Queen);

            ChessBoard.LastMove.GeneratePGNString(ChessBoard);

            FileIO.SaveCurrentGameMove(ChessBoard, PreviousChessBoard, CurrentGameBook, bestMove);

            for (byte x = 0; x < 64; x++)
            {
                var sqr = ChessBoard.Squares[x];

                if (sqr.Piece == null)
                    continue;

                sqr.Piece.DefendedValue = 0;
                sqr.Piece.AttackedValue = 0;
            }

            PieceValidMoves.GenerateValidMoves(ChessBoard);
            Evaluation.EvaluateBoardScore(ChessBoard);

            PieceTakenAdd(ChessBoard.LastMove);

            MoveHistory.Push(ChessBoard.LastMove);

            //Second Call if computer move might have caused a mate
            if (CheckForMate(WhoseMove, ref ChessBoard))
            {
                Thinking = false;

                if (ChessBoard.WhiteMate || ChessBoard.BlackMate) 
                    LastMove.PgnMove += "#";

                return;
            }

            if (ChessBoard.WhiteCheck || ChessBoard.BlackCheck) 
                LastMove.PgnMove += "+";

            Thinking = false;
        }

        #endregion

        #region InternalMembers

        internal List<OpeningMove> CurrentGameBook;
        internal List<OpeningMove> UndoGameBook;

        #endregion

        #region PrivateMembers

        private Board ChessBoard;
        private Board PreviousChessBoard;
        private Board UndoChessBoard;

        private Stack<MoveContent> MoveHistory;
        private List<OpeningMove> OpeningBook;

        private string pvLine;

        #endregion

        #region PublicMembers

        public enum Difficulty
        {
            Easy,
            Medium,
            Hard,
            VeryHard
        }

        public enum TimeSettings
        {
            Moves40In5Minutes,
            Moves40In10Minutes,
            Moves40In20Minutes,
            Moves40In30Minutes,
            Moves40In40Minutes,
            Moves40In60Minutes,
            Moves40In90Minutes
        }

        public ChessPieceType PromoteToPieceType = ChessPieceType.Queen;

        public PiecesTaken PiecesTakenCount = new PiecesTaken();

        //State Variables
        public ChessPieceColor HumanPlayer;
        public bool Thinking;
        public bool TrainingMode;

        //Stats
        public int NodesSearched;
        public int NodesQuiessence;
        public byte PlyDepthSearched;
        public byte PlyDepthReached;
        public byte RootMovesSearched;

        public TimeSettings GameTimeSettings;

        // ReSharper disable once InconsistentNaming
        public string FEN => Board.Fen(false, ChessBoard);

        public MoveContent LastMove => ChessBoard.LastMove;

        public Difficulty GameDifficulty
        {
            get
            {
                if (PlyDepthSearched == 3) return Difficulty.Easy;
                if (PlyDepthSearched == 5) return Difficulty.Medium;
                if (PlyDepthSearched == 6) return Difficulty.Hard;
                return PlyDepthSearched == 7 ? Difficulty.VeryHard : Difficulty.Medium;
            }
            set
            {
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (value == Difficulty.Easy)
                {
                    PlyDepthSearched = 3;
                    GameTimeSettings = TimeSettings.Moves40In10Minutes;
                }
                else if (value == Difficulty.Medium)
                {
                    PlyDepthSearched = 5;
                    GameTimeSettings = TimeSettings.Moves40In20Minutes;
                }
                else if (value == Difficulty.Hard)
                {
                    PlyDepthSearched = 6;
                    GameTimeSettings = TimeSettings.Moves40In60Minutes;
                }
                else if (value == Difficulty.VeryHard)
                {
                    PlyDepthSearched = 7;
                    GameTimeSettings = TimeSettings.Moves40In90Minutes;
                }
            }
        }

        public ChessPieceColor WhoseMove
        {
            get => ChessBoard.WhoseMove;
            set => ChessBoard.WhoseMove = value;
        }

        public bool StaleMate
        {
            get => ChessBoard.StaleMate;
            set => ChessBoard.StaleMate = value;
        }

        public bool RepeatedMove => ChessBoard.RepeatedMove >= 3;

        public string PvLine => pvLine;

        public bool FiftyMove => ChessBoard.HalfMoveClock >= 100;

        public bool InsufficientMaterial => ChessBoard.InsufficientMaterial;

        #endregion

        #region Constructors

        public Engine()
        {
            NewGame();
            /*InitiateEngine();
            InitiateBoard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");*/
        }

        public Engine(string fen)
        {
            InitiateEngine();
            InitiateBoard(fen);
        }

        public void NewGame()
        {
            InitiateEngine();
            InitiateBoard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        }

        public void InitiateBoard(string fen)
        {
            ChessBoard = new Board(fen);

            if (string.IsNullOrEmpty(fen))
                return;
            PieceValidMoves.GenerateValidMoves(ChessBoard);
            Evaluation.EvaluateBoardScore(ChessBoard);
        }

        private void InitiateEngine()
        {
            GameDifficulty = Difficulty.Medium;

            MoveHistory = new Stack<MoveContent>();
            HumanPlayer = ChessPieceColor.White;
            OpeningBook = new List<OpeningMove>();
            CurrentGameBook = new List<OpeningMove>();
            PieceMoves.InitiateChessPieceMotion();
            LoadOpeningBook();
        }

        #endregion

        #region Methods

        public void SetChessPieceSelection(byte boardColumn, byte boardRow,
            bool selection)
        {
            var index = GetBoardIndex(boardColumn, boardRow);

            if (ChessBoard.Squares[index].Piece == null) return;
            if (ChessBoard.Squares[index].Piece.PieceColor != HumanPlayer) return;
            if (ChessBoard.Squares[index].Piece.PieceColor != WhoseMove) return;
            ChessBoard.Squares[index].Piece.Selected = selection;
        }

        public int ValidateOpeningBook()
        {
            return Book.ValidateOpeningBook(OpeningBook);
        }

        private static bool CheckForMate(ChessPieceColor whosTurn, ref Board chessBoard)
        {
            Search.SearchForMate(whosTurn, chessBoard, ref chessBoard.BlackMate,
                ref chessBoard.WhiteMate, ref chessBoard.StaleMate);

            return chessBoard.BlackMate || chessBoard.WhiteMate || chessBoard.StaleMate;
        }

        private static bool FindPlayBookMove(ref MoveContent bestMove, Board chessBoard,
            IEnumerable<OpeningMove> openingBook)
        {
            //Get the Hash for the current Board;
            var boardFen = Board.Fen(true, chessBoard);

            //Check the Opening Move Book
            foreach (var move in openingBook)
            {
                if (!move.StartingFEN.Contains(boardFen))
                    continue;
                var index = 0;

                bestMove = move.Moves[index];
                return true;
            }

            return false;
        }

        public void Undo()
        {
            if (UndoChessBoard == null)
                return;

            PieceTakenRemove(ChessBoard.LastMove);
            PieceTakenRemove(PreviousChessBoard.LastMove);

            ChessBoard = new Board(UndoChessBoard);
            CurrentGameBook = new List<OpeningMove>(UndoGameBook);

            PieceValidMoves.GenerateValidMoves(ChessBoard);
            Evaluation.EvaluateBoardScore(ChessBoard);
        }

        private static byte GetBoardIndex(byte boardColumn, byte boardRow)
        {
            return (byte) (boardColumn + boardRow * 8);
        }

        public byte[] GetEnPassantMoves()
        {
            if (ChessBoard == null) return null;

            var returnArray = new byte[2];

            returnArray[0] = (byte) (ChessBoard.EnPassantPosition % 8);
            returnArray[1] = (byte) (ChessBoard.EnPassantPosition / 8);

            return returnArray;
        }

        public byte GetEnPassantMovesSquares()
        {
            if (ChessBoard == null) return byte.MaxValue;

            return ChessBoard.WhoseMove == ChessBoard.EnPassantColor ? ChessBoard.EnPassantPosition : byte.MaxValue;

            //return ChessBoard?.EnPassantPosition ?? byte.MaxValue;
        }

        public bool GetBlackMate()
        {
            return ChessBoard != null && ChessBoard.BlackMate;
        }

        public bool GetWhiteMate()
        {
            return ChessBoard.WhiteMate;
        }

        public bool GetBlackCheck()
        {
            return ChessBoard.BlackCheck;
        }

        public bool GetWhiteCheck()
        {
            return ChessBoard.WhiteCheck;
        }

        public byte GetRepeatedMove()
        {
            return ChessBoard.RepeatedMove;
        }

        public byte GetHalfMoveClock()
        {
            return ChessBoard.HalfMoveClock;
        }

        public Stack<MoveContent> GetMoveHistory()
        {
            return MoveHistory;
        }

        public ChessPieceType GetPieceTypeAt(byte boardColumn, byte boardRow)
        {
            var index = GetBoardIndex(boardColumn,
                boardRow);
            return ChessBoard.Squares[index].Piece == null
                ? ChessPieceType.None
                : ChessBoard.Squares[index].Piece.PieceType;
        }

        public ChessPieceType GetPieceTypeAt(byte index)
        {
            return ChessBoard.Squares[index].Piece == null
                ? ChessPieceType.None
                : ChessBoard.Squares[index].Piece.PieceType;
        }

        public ChessPieceColor GetPieceColorAt(byte boardColumn, byte boardRow)
        {
            var index = GetBoardIndex(boardColumn, boardRow);
            return ChessBoard.Squares[index].Piece == null
                ? ChessPieceColor.White
                : ChessBoard.Squares[index].Piece.PieceColor;
        }

        public ChessPieceColor GetPieceColorAt(byte index)
        {
            return ChessBoard.Squares[index].Piece == null
                ? ChessPieceColor.White
                : ChessBoard.Squares[index].Piece.PieceColor;
        }

        public bool GetChessPieceSelected(byte boardColumn, byte boardRow)
        {
            var index = GetBoardIndex(boardColumn, boardRow);
            return ChessBoard.Squares[index].Piece != null
                   && ChessBoard.Squares[index].Piece.Selected;
        }

        public void GenerateValidMoves()
        {
            PieceValidMoves.GenerateValidMoves(ChessBoard);
        }

        public int EvaluateBoardScore()
        {
            Evaluation.EvaluateBoardScore(ChessBoard);
            return ChessBoard.Score;
        }

        public IEnumerable<byte> GetValidMovesSquare(byte boardColumn, byte boardRow)
        {
            var index = GetBoardIndex(boardColumn, boardRow);

            var list = new List<byte>();
            if (ChessBoard.Squares[index].Piece == null) 
                return list;

            foreach (var square in ChessBoard.Squares[index].Piece.ValidMoves)
                list.Add(square);

            return list;
        }

        public IEnumerable<byte[]> GetValidMoves(byte boardColumn, byte boardRow)
        {
            var index = GetBoardIndex(boardColumn, boardRow);

            if (ChessBoard.Squares[index].Piece == null) return null;

            var returnArray = new byte[ChessBoard.Squares[index].Piece.ValidMoves.Count][];
            var counter = 0;

            foreach (var square in ChessBoard.Squares[index].Piece.ValidMoves)
            {
                returnArray[counter] = new byte[2];
                returnArray[counter][0] = (byte) (square % 8);
                returnArray[counter][1] = (byte) (square / 8);
                counter++;
            }

            return returnArray;
        }

        public int GetScore()
        {
            return ChessBoard.Score;
        }

        public byte FindSourcePositon(ChessPieceType chessPieceType, ChessPieceColor chessPieceColor, byte dstPosition,
            bool capture, int forceCol, int forceRow)
        {
            Square square;
            if (dstPosition == ChessBoard.EnPassantPosition && chessPieceType == ChessPieceType.Pawn)
            {
                if (chessPieceColor == ChessPieceColor.White)
                {
                    square = ChessBoard.Squares[dstPosition + 7];
                    if (square.Piece != null)
                        if (square.Piece.PieceType == ChessPieceType.Pawn)
                            if (square.Piece.PieceColor == chessPieceColor)
                                if ((dstPosition + 7) % 8 == forceCol || forceCol == -1)
                                    return (byte) (dstPosition + 7);

                    square = ChessBoard.Squares[dstPosition + 9];
                    if (square.Piece != null)
                        if (square.Piece.PieceType == ChessPieceType.Pawn)
                            if (square.Piece.PieceColor == chessPieceColor)
                                if ((dstPosition + 9) % 8 == forceCol || forceCol == -1)
                                    return (byte) (dstPosition + 9);
                }
                else
                {
                    if (dstPosition - 7 >= 0)
                    {
                        square = ChessBoard.Squares[dstPosition - 7];
                        if (square.Piece != null)
                            if (square.Piece.PieceType == ChessPieceType.Pawn)
                                if (square.Piece.PieceColor == chessPieceColor)
                                    if ((dstPosition - 7) % 8 == forceCol || forceCol == -1)
                                        return (byte) (dstPosition - 7);
                    }

                    if (dstPosition - 9 >= 0)
                    {
                        square = ChessBoard.Squares[dstPosition - 9];
                        if (square.Piece != null)
                            if (square.Piece.PieceType == ChessPieceType.Pawn)
                                if (square.Piece.PieceColor == chessPieceColor)
                                    if ((dstPosition - 9) % 8 == forceCol || forceCol == -1)
                                        return (byte) (dstPosition - 9);
                    }
                }
            }

            for (byte x = 0; x < 64; x++)
            {
                square = ChessBoard.Squares[x];
                if (square.Piece == null) continue;
                if (square.Piece.PieceType != chessPieceType) continue;
                if (square.Piece.PieceColor != chessPieceColor) continue;
                foreach (var move in square.Piece.ValidMoves)
                {
                    if (move != dstPosition) continue;
                    if (!capture)
                        if ((byte) (x / 8) == forceRow || forceRow == -1)
                            if (x % 8 == forceCol || forceCol == -1)
                                return x;

                    //Capture
                    if (ChessBoard.Squares[dstPosition].Piece == null) continue;
                    if (ChessBoard.Squares[dstPosition].Piece.PieceColor == chessPieceColor) continue;
                    if (x % 8 != forceCol && forceCol != -1) continue;
                    if ((byte) (x / 8) == forceRow || forceRow == -1) return x;
                }
            }

            return 0;
        }

        // ReSharper disable once InconsistentNaming
        public bool IsValidMoveAN(string move)
        {
            byte sourceColumn = 0, sourceRow = 0, destinationColumn = 0, destinationRow = 0;
            MoveContent.ParseAN(move, ref sourceColumn, ref sourceRow, ref destinationColumn, ref destinationRow);
            return IsValidMove(sourceColumn, sourceRow, destinationColumn, destinationRow);
        }

        public bool IsValidMove(byte srcPosition, byte dstPosition)
        {
            if (ChessBoard?.Squares?[srcPosition].Piece == null) return false;

            foreach (var bs in ChessBoard.Squares[srcPosition].Piece.ValidMoves)
                if (bs == dstPosition)
                    return true;

            return dstPosition == ChessBoard.EnPassantPosition;
        }

        public bool IsValidMove(byte sourceColumn, byte sourceRow, byte destinationColumn, byte destinationRow)
        {
            if (ChessBoard?.Squares == null) return false;

            var index = GetBoardIndex(sourceColumn, sourceRow);

            if (ChessBoard.Squares[index].Piece == null) return false;

            foreach (var bs in ChessBoard.Squares[index].Piece.ValidMoves)
            {
                if (bs % 8 != destinationColumn) continue;
                if ((byte) (bs / 8) == destinationRow) return true;
            }

            /*index = GetBoardIndex(destinationColumn, destinationRow);

            if (index == ChessBoard.EnPassantPosition && ChessBoard.EnPassantPosition > 0)
            {
                return true;
            }*/

            return false;
        }

        public bool IsGameOver()
        {
            if (ChessBoard.StaleMate) return true;
            if (ChessBoard.WhiteMate || ChessBoard.BlackMate) return true;
            if (ChessBoard.HalfMoveClock >= 100) return true;
            return ChessBoard.RepeatedMove >= 3 || ChessBoard.InsufficientMaterial;
        }

        public bool IsTie()
        {
            if (ChessBoard.StaleMate) return true;

            if (ChessBoard.HalfMoveClock >= 100) return true;
            return ChessBoard.RepeatedMove >= 3 || ChessBoard.InsufficientMaterial;
        }

        public bool MovePiece(byte srcPosition, byte dstPosition)
        {
            var piece = ChessBoard.Squares[srcPosition].Piece;

            PreviousChessBoard = new Board(ChessBoard);
            UndoChessBoard = new Board(ChessBoard);
            UndoGameBook = new List<OpeningMove>(CurrentGameBook);

            Board.MovePiece(ChessBoard, srcPosition, dstPosition, PromoteToPieceType);

            ChessBoard.LastMove.GeneratePGNString(ChessBoard);

            PieceValidMoves.GenerateValidMoves(ChessBoard);
            Evaluation.EvaluateBoardScore(ChessBoard);

            // If there is a check in place, check if this is still true;
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (piece.PieceColor == ChessPieceColor.White)
            {
                if (ChessBoard.WhiteCheck)
                {
                    //Invalid Move
                    ChessBoard = new Board(PreviousChessBoard);
                    PieceValidMoves.GenerateValidMoves(ChessBoard);
                    return false;
                }
            }
            else if (piece.PieceColor == ChessPieceColor.Black)
            {
                if (ChessBoard.BlackCheck)
                {
                    //Invalid Move
                    ChessBoard = new Board(PreviousChessBoard);
                    PieceValidMoves.GenerateValidMoves(ChessBoard);
                    return false;
                }
            }

            MoveHistory.Push(ChessBoard.LastMove);
            FileIO.SaveCurrentGameMove(ChessBoard, PreviousChessBoard, CurrentGameBook, ChessBoard.LastMove);

            CheckForMate(WhoseMove, ref ChessBoard);
            PieceTakenAdd(ChessBoard.LastMove);

            if (ChessBoard.WhiteMate || ChessBoard.BlackMate)
                LastMove.PgnMove += "#";
            else if (ChessBoard.WhiteCheck || ChessBoard.BlackCheck) LastMove.PgnMove += "+";

            return true;
        }

        private void PieceTakenAdd(MoveContent lastMove)
        {
            if (lastMove.TakenPiece.PieceType == ChessPieceType.None) return;
            if (lastMove.TakenPiece.PieceColor == ChessPieceColor.White)
            {
                if (lastMove.TakenPiece.PieceType == ChessPieceType.Queen)
                    PiecesTakenCount.WhiteQueen++;
                else if (lastMove.TakenPiece.PieceType == ChessPieceType.Rook)
                    PiecesTakenCount.WhiteRook++;
                else if (lastMove.TakenPiece.PieceType == ChessPieceType.Bishop)
                    PiecesTakenCount.WhiteBishop++;
                else if (lastMove.TakenPiece.PieceType == ChessPieceType.Knight)
                    PiecesTakenCount.WhiteKnight++;
                else if (lastMove.TakenPiece.PieceType == ChessPieceType.Pawn) PiecesTakenCount.WhitePawn++;
            }

            if (ChessBoard.LastMove.TakenPiece.PieceColor != ChessPieceColor.Black) return;
            if (lastMove.TakenPiece.PieceType == ChessPieceType.Queen)
                PiecesTakenCount.BlackQueen++;
            else if (lastMove.TakenPiece.PieceType == ChessPieceType.Rook)
                PiecesTakenCount.BlackRook++;
            else if (lastMove.TakenPiece.PieceType == ChessPieceType.Bishop)
                PiecesTakenCount.BlackBishop++;
            else if (lastMove.TakenPiece.PieceType == ChessPieceType.Knight)
                PiecesTakenCount.BlackKnight++;
            else if (lastMove.TakenPiece.PieceType == ChessPieceType.Pawn) PiecesTakenCount.BlackPawn++;
        }

        private void PieceTakenRemove(MoveContent lastMove)
        {
            if (lastMove.TakenPiece.PieceType == ChessPieceType.None) return;
            if (lastMove.TakenPiece.PieceColor == ChessPieceColor.White)
            {
                if (lastMove.TakenPiece.PieceType == ChessPieceType.Queen)
                    PiecesTakenCount.WhiteQueen--;
                else if (lastMove.TakenPiece.PieceType == ChessPieceType.Rook)
                    PiecesTakenCount.WhiteRook--;
                else if (lastMove.TakenPiece.PieceType == ChessPieceType.Bishop)
                    PiecesTakenCount.WhiteBishop--;
                else if (lastMove.TakenPiece.PieceType == ChessPieceType.Knight)
                    PiecesTakenCount.WhiteKnight--;
                else if (lastMove.TakenPiece.PieceType == ChessPieceType.Pawn) PiecesTakenCount.WhitePawn--;
            }

            if (lastMove.TakenPiece.PieceColor != ChessPieceColor.Black) return;
            if (lastMove.TakenPiece.PieceType == ChessPieceType.Queen)
                PiecesTakenCount.BlackQueen--;
            else if (lastMove.TakenPiece.PieceType == ChessPieceType.Rook)
                PiecesTakenCount.BlackRook--;
            else if (lastMove.TakenPiece.PieceType == ChessPieceType.Bishop)
                PiecesTakenCount.BlackBishop--;
            else if (lastMove.TakenPiece.PieceType == ChessPieceType.Knight)
                PiecesTakenCount.BlackKnight--;
            else if (lastMove.TakenPiece.PieceType == ChessPieceType.Pawn) PiecesTakenCount.BlackPawn--;
        }

        // ReSharper disable once InconsistentNaming
        public bool MovePieceAN(string move)
        {
            byte sourceColumn = 0, sourceRow = 0, destinationColumn = 0, destinationRow = 0;
            MoveContent.ParseAN(move, ref sourceColumn, ref sourceRow, ref destinationColumn, ref destinationRow);
            return MovePiece(sourceColumn, sourceRow, destinationColumn, destinationRow);
        }

        public bool MovePiece(byte sourceColumn, byte sourceRow, byte destinationColumn, byte destinationRow)
        {
            var srcPosition = (byte) (sourceColumn + sourceRow * 8);
            var dstPosition = (byte) (destinationColumn + destinationRow * 8);

            return MovePiece(srcPosition, dstPosition);
        }

        internal void SetChessPiece(Piece piece, byte index)
        {
            ChessBoard.Squares[index].Piece = new Piece(piece);
        }

        #endregion

        #region FileIO

        public bool SaveGame(string filePath)
        {
            return FileIO.SaveGame(filePath, ChessBoard, WhoseMove, MoveHistory);
        }

        public bool LoadGame(string filePath)
        {
            return FileIO.LoadGame(filePath, ref ChessBoard, WhoseMove, ref MoveHistory, ref CurrentGameBook,
                ref UndoGameBook);
        }

        public bool LoadOpeningBook()
        {
            OpeningBook = Book.LoadOpeningBook();

            return true;
        }

        #endregion
    }
}