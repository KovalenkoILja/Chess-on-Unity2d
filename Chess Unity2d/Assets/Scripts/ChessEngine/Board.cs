using System.Collections.Generic;

namespace ChessEngine
{
    // Board Representation

    // ReSharper disable IdentifierTypo
    // ReSharper disable CommentTypo
    // ReSharper disable MemberCanBePrivate.Global
    // ReSharper disable FieldCanBeMadeReadOnly.Global
    public sealed class Board
    {
        #region Variables

        public bool[] BlackAttackBoard;

        public bool BlackCanCastle;

        public bool BlackCastled;

        //Game Over Flags
        public bool BlackCheck;
        public byte BlackKingPosition;
        public bool BlackMate;

        public bool EndGamePhase;

        //Who initated En Passant
        public ChessPieceColor EnPassantColor;

        //Positions liable to En Passant
        public byte EnPassantPosition;

        //Halfmove clock specifies a decimal number of half moves with respect to the 50 move draw rule
        //Reset to zero after a capture or a pawn move and incremented otherwise
        public byte HalfMoveClock;

        public bool InsufficientMaterial;

        public MoveContent LastMove;

        public int MoveCount;

        //If repeat move count reaches 3 we know that a tie has occurred
        public byte RepeatedMove;

        public int Score;

        public Square[] Squares;
        public bool StaleMate;
        public bool[] WhiteAttackBoard;
        public bool WhiteCanCastle;
        public bool WhiteCastled;
        public bool WhiteCheck;

        public byte WhiteKingPosition;
        public bool WhiteMate;

        public ChessPieceColor WhoseMove;

        //The Zobrist Hash representing chess board as 64 bit variable

        #endregion

        #region Constructors

        //Default Constructor

        public Board(string fen) : this()
        {
            byte index = 0;
            byte spc = 0;

            WhiteCastled = true;
            BlackCastled = true;

            byte spacers = 0;

            WhoseMove = ChessPieceColor.White;

            if (fen.Contains("a3"))
            {
                EnPassantColor = ChessPieceColor.White;
                EnPassantPosition = 40;
            }
            else if (fen.Contains("b3"))
            {
                EnPassantColor = ChessPieceColor.White;
                EnPassantPosition = 41;
            }
            else if (fen.Contains("c3"))
            {
                EnPassantColor = ChessPieceColor.White;
                EnPassantPosition = 42;
            }
            else if (fen.Contains("d3"))
            {
                EnPassantColor = ChessPieceColor.White;
                EnPassantPosition = 43;
            }
            else if (fen.Contains("e3"))
            {
                EnPassantColor = ChessPieceColor.White;
                EnPassantPosition = 44;
            }
            else if (fen.Contains("f3"))
            {
                EnPassantColor = ChessPieceColor.White;
                EnPassantPosition = 45;
            }
            else if (fen.Contains("g3"))
            {
                EnPassantColor = ChessPieceColor.White;
                EnPassantPosition = 46;
            }
            else if (fen.Contains("h3"))
            {
                EnPassantColor = ChessPieceColor.White;
                EnPassantPosition = 47;
            }


            if (fen.Contains("a6"))
            {
                EnPassantColor = ChessPieceColor.Black;
                EnPassantPosition = 16;
            }
            else if (fen.Contains("b6"))
            {
                EnPassantColor = ChessPieceColor.Black;
                EnPassantPosition = 17;
            }
            else if (fen.Contains("c6"))
            {
                EnPassantColor = ChessPieceColor.Black;
                EnPassantPosition = 18;
            }
            else if (fen.Contains("d6"))
            {
                EnPassantColor = ChessPieceColor.Black;
                EnPassantPosition = 19;
            }
            else if (fen.Contains("e6"))
            {
                EnPassantColor = ChessPieceColor.Black;
                EnPassantPosition = 20;
            }
            else if (fen.Contains("f6"))
            {
                EnPassantColor = ChessPieceColor.Black;
                EnPassantPosition = 21;
            }
            else if (fen.Contains("g6"))
            {
                EnPassantColor = ChessPieceColor.Black;
                EnPassantPosition = 22;
            }
            else if (fen.Contains("h6"))
            {
                EnPassantColor = ChessPieceColor.Black;
                EnPassantPosition = 23;
            }

            if (fen.Contains(" w ")) WhoseMove = ChessPieceColor.White;
            if (fen.Contains(" b ")) WhoseMove = ChessPieceColor.Black;

            foreach (var c in fen)
                if (index < 64 && spc == 0)
                {
                    if (c == '1' && index < 63)
                    {
                        index++;
                    }
                    else if (c == '2' && index < 62)
                    {
                        index += 2;
                    }
                    else if (c == '3' && index < 61)
                    {
                        index += 3;
                    }
                    else if (c == '4' && index < 60)
                    {
                        index += 4;
                    }
                    else if (c == '5' && index < 59)
                    {
                        index += 5;
                    }
                    else if (c == '6' && index < 58)
                    {
                        index += 6;
                    }
                    else if (c == '7' && index < 57)
                    {
                        index += 7;
                    }
                    else if (c == '8' && index < 56)
                    {
                        index += 8;
                    }
                    else if (c == 'P')
                    {
                        Squares[index].Piece = new Piece(ChessPieceType.Pawn, ChessPieceColor.White)
                        {
                            Moved = true
                        };
                        index++;
                    }
                    else if (c == 'N')
                    {
                        Squares[index].Piece = new Piece(ChessPieceType.Knight, ChessPieceColor.White)
                        {
                            Moved = true
                        };
                        index++;
                    }
                    else if (c == 'B')
                    {
                        Squares[index].Piece = new Piece(ChessPieceType.Bishop, ChessPieceColor.White)
                        {
                            Moved = true
                        };
                        index++;
                    }
                    else if (c == 'R')
                    {
                        Squares[index].Piece = new Piece(ChessPieceType.Rook, ChessPieceColor.White)
                        {
                            Moved = true
                        };
                        index++;
                    }
                    else if (c == 'Q')
                    {
                        Squares[index].Piece = new Piece(ChessPieceType.Queen, ChessPieceColor.White)
                        {
                            Moved = true
                        };
                        index++;
                    }
                    else if (c == 'K')
                    {
                        Squares[index].Piece = new Piece(ChessPieceType.King, ChessPieceColor.White)
                        {
                            Moved = true
                        };
                        index++;
                    }
                    else if (c == 'p')
                    {
                        Squares[index].Piece = new Piece(ChessPieceType.Pawn, ChessPieceColor.Black)
                        {
                            Moved = true
                        };
                        index++;
                    }
                    else if (c == 'n')
                    {
                        Squares[index].Piece = new Piece(ChessPieceType.Knight, ChessPieceColor.Black)
                        {
                            Moved = true
                        };
                        index++;
                    }
                    else if (c == 'b')
                    {
                        Squares[index].Piece = new Piece(ChessPieceType.Bishop, ChessPieceColor.Black)
                        {
                            Moved = true
                        };
                        index++;
                    }
                    else if (c == 'r')
                    {
                        Squares[index].Piece = new Piece(ChessPieceType.Rook, ChessPieceColor.Black)
                        {
                            Moved = true
                        };
                        index++;
                    }
                    else if (c == 'q')
                    {
                        Squares[index].Piece = new Piece(ChessPieceType.Queen, ChessPieceColor.Black)
                        {
                            Moved = true
                        };
                        index++;
                    }
                    else if (c == 'k')
                    {
                        Squares[index].Piece = new Piece(ChessPieceType.King, ChessPieceColor.Black)
                        {
                            Moved = true
                        };
                        index++;
                    }
                    else if (c == '/')
                    {
                    }
                    else if (c == ' ')
                    {
                        spc++;
                    }
                }
                else
                {
                    switch (c)
                    {
                        case 'K':
                        {
                            if (Squares[60].Piece != null)
                                if (Squares[60].Piece.PieceType == ChessPieceType.King)
                                    Squares[60].Piece.Moved = false;

                            if (Squares[63].Piece != null)
                                if (Squares[63].Piece.PieceType == ChessPieceType.Rook)
                                    Squares[63].Piece.Moved = false;

                            WhiteCastled = false;
                            break;
                        }
                        case 'Q':
                        {
                            if (Squares[60].Piece != null)
                                if (Squares[60].Piece.PieceType == ChessPieceType.King)
                                    Squares[60].Piece.Moved = false;

                            if (Squares[56].Piece != null)
                                if (Squares[56].Piece.PieceType == ChessPieceType.Rook)
                                    Squares[56].Piece.Moved = false;

                            WhiteCastled = false;
                            break;
                        }
                        case 'k':
                        {
                            if (Squares[4].Piece != null)
                                if (Squares[4].Piece.PieceType == ChessPieceType.King)
                                    Squares[4].Piece.Moved = false;

                            if (Squares[7].Piece != null)
                                if (Squares[7].Piece.PieceType == ChessPieceType.Rook)
                                    Squares[7].Piece.Moved = false;

                            BlackCastled = false;
                            break;
                        }
                        case 'q':
                        {
                            if (Squares[4].Piece != null)
                                if (Squares[4].Piece.PieceType == ChessPieceType.King)
                                    Squares[4].Piece.Moved = false;

                            if (Squares[0].Piece != null)
                                if (Squares[0].Piece.PieceType == ChessPieceType.Rook)
                                    Squares[0].Piece.Moved = false;

                            BlackCastled = false;
                            break;
                        }
                        case ' ':
                            spacers++;
                            break;
                        case '1' when spacers == 4:
                            HalfMoveClock = (byte) (HalfMoveClock * 10 + 1);
                            break;
                        case '2' when spacers == 4:
                            HalfMoveClock = (byte) (HalfMoveClock * 10 + 2);
                            break;
                        case '3' when spacers == 4:
                            HalfMoveClock = (byte) (HalfMoveClock * 10 + 3);
                            break;
                        case '4' when spacers == 4:
                            HalfMoveClock = (byte) (HalfMoveClock * 10 + 4);
                            break;
                        case '5' when spacers == 4:
                            HalfMoveClock = (byte) (HalfMoveClock * 10 + 5);
                            break;
                        case '6' when spacers == 4:
                            HalfMoveClock = (byte) (HalfMoveClock * 10 + 6);
                            break;
                        case '7' when spacers == 4:
                            HalfMoveClock = (byte) (HalfMoveClock * 10 + 7);
                            break;
                        case '8' when spacers == 4:
                            HalfMoveClock = (byte) (HalfMoveClock * 10 + 8);
                            break;
                        case '9' when spacers == 4:
                            HalfMoveClock = (byte) (HalfMoveClock * 10 + 9);
                            break;
                        case '0' when spacers == 4:
                            MoveCount = (byte) (MoveCount * 10 + 0);
                            break;
                        case '1' when spacers == 5:
                            MoveCount = (byte) (MoveCount * 10 + 1);
                            break;
                        case '2' when spacers == 5:
                            MoveCount = (byte) (MoveCount * 10 + 2);
                            break;
                        case '3' when spacers == 5:
                            MoveCount = (byte) (MoveCount * 10 + 3);
                            break;
                        case '4' when spacers == 5:
                            MoveCount = (byte) (MoveCount * 10 + 4);
                            break;
                        case '5' when spacers == 5:
                            MoveCount = (byte) (MoveCount * 10 + 5);
                            break;
                        case '6' when spacers == 5:
                            MoveCount = (byte) (MoveCount * 10 + 6);
                            break;
                        case '7' when spacers == 5:
                            MoveCount = (byte) (MoveCount * 10 + 7);
                            break;
                        case '8' when spacers == 5:
                            MoveCount = (byte) (MoveCount * 10 + 8);
                            break;
                        case '9' when spacers == 5:
                            MoveCount = (byte) (MoveCount * 10 + 9);
                            break;
                        case '0' when spacers == 5:
                            MoveCount = (byte) (MoveCount * 10 + 0);
                            break;
                    }
                }
        }

        public Board()
        {
            Squares = new Square[64];

            for (byte i = 0; i < 64; i++) Squares[i] = new Square();

            LastMove = new MoveContent();

            BlackCanCastle = true;
            WhiteCanCastle = true;

            WhiteAttackBoard = new bool[64];
            BlackAttackBoard = new bool[64];
        }

        private Board(IList<Square> squares)
        {
            Squares = new Square[64];

            for (byte x = 0; x < 64; x++)
                if (squares[x].Piece != null)
                    Squares[x].Piece = new Piece(squares[x].Piece);


            WhiteAttackBoard = new bool[64];
            BlackAttackBoard = new bool[64];
        }

        //Constructor
        internal Board(int score) : this()
        {
            Score = score;

            WhiteAttackBoard = new bool[64];
            BlackAttackBoard = new bool[64];
        }

        //Copy Constructor
        internal Board(Board board)
        {
            Squares = new Square[64];

            for (byte x = 0; x < 64; x++)
                if (board.Squares[x].Piece != null)
                    Squares[x] = new Square(board.Squares[x].Piece);

            WhiteAttackBoard = new bool[64];
            BlackAttackBoard = new bool[64];

            for (byte x = 0; x < 64; x++)
            {
                WhiteAttackBoard[x] = board.WhiteAttackBoard[x];
                BlackAttackBoard[x] = board.BlackAttackBoard[x];
            }

            EndGamePhase = board.EndGamePhase;

            HalfMoveClock = board.HalfMoveClock;
            RepeatedMove = board.RepeatedMove;

            WhiteCastled = board.WhiteCastled;
            BlackCastled = board.BlackCastled;

            WhiteCanCastle = board.WhiteCanCastle;
            BlackCanCastle = board.BlackCanCastle;

            WhiteKingPosition = board.WhiteKingPosition;
            BlackKingPosition = board.BlackKingPosition;

            BlackCheck = board.BlackCheck;
            WhiteCheck = board.WhiteCheck;
            StaleMate = board.StaleMate;
            WhiteMate = board.WhiteMate;
            BlackMate = board.BlackMate;
            WhoseMove = board.WhoseMove;
            EnPassantPosition = board.EnPassantPosition;
            EnPassantColor = board.EnPassantColor;

            Score = board.Score;

            LastMove = new MoveContent(board.LastMove);

            MoveCount = board.MoveCount;
        }

        #endregion

        #region PrivateMethods

        // ReSharper disable InvertIf
        private static bool PromotePawns(Board board, Piece piece, byte dstPosition, ChessPieceType promoteToPiece)
        {
            if (piece.PieceType == ChessPieceType.Pawn)
            {
                if (dstPosition < 8)
                {
                    board.Squares[dstPosition].Piece.PieceType = promoteToPiece;
                    board.Squares[dstPosition].Piece.PieceValue = Piece.CalculatePieceValue(promoteToPiece);
                    board.Squares[dstPosition].Piece.PieceActionValue =
                        Piece.CalculatePieceActionValue(promoteToPiece);
                    return true;
                }

                if (dstPosition > 55)
                {
                    board.Squares[dstPosition].Piece.PieceType = promoteToPiece;
                    board.Squares[dstPosition].Piece.PieceValue = Piece.CalculatePieceValue(promoteToPiece);
                    board.Squares[dstPosition].Piece.PieceActionValue =
                        Piece.CalculatePieceActionValue(promoteToPiece);
                    return true;
                }
            }

            return false;
        }

        private static void RecordEnPassant(ChessPieceColor pcColor,
            ChessPieceType pcType,
            Board board,
            byte srcPosition,
            byte dstPosition)
        {
            //Record En Passant if Pawn Moving
            if (pcType == ChessPieceType.Pawn)
            {
                //Reset HalfMoveClockCount if pawn moved
                board.HalfMoveClock = 0;

                var difference = srcPosition - dstPosition;

                if (difference == 16 || difference == -16)
                {
                    board.EnPassantPosition = (byte) (dstPosition + difference / 2);
                    board.EnPassantColor = pcColor;
                }
            }
        }

        private static bool SetEnpassantMove(Board board, byte srcPosition, byte dstPosition,
            ChessPieceColor pcColor)
        {
            if (board.EnPassantPosition != dstPosition) return false;

            if (pcColor == board.EnPassantColor) return false;

            if (board.Squares[srcPosition].Piece.PieceType != ChessPieceType.Pawn) return false;

            var pieceLocationOffset = 8;

            if (board.EnPassantColor == ChessPieceColor.White) pieceLocationOffset = -8;

            dstPosition = (byte) (dstPosition + pieceLocationOffset);

            var sqr = board.Squares[dstPosition];

            board.LastMove.TakenPiece = new PieceTaken(sqr.Piece.PieceColor, sqr.Piece.PieceType, sqr.Piece.Moved,
                dstPosition);

            board.Squares[dstPosition].Piece = null;

            //Reset HalfMoveClockCount if capture
            board.HalfMoveClock = 0;

            return true;
        }

        private static void KingCastle(Board board, Piece piece, byte srcPosition, byte dstPosition)
        {
            if (piece.PieceType != ChessPieceType.King) return;

            // If this is a casteling move.
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (piece.PieceColor == ChessPieceColor.White && srcPosition == 60)
            {
                //Castle Right
                if (dstPosition == 62)
                {
                    //Ok we are casteling we need to move the Rook
                    if (board.Squares[63].Piece != null)
                    {
                        board.Squares[61].Piece = board.Squares[63].Piece;
                        board.Squares[63].Piece = null;
                        board.WhiteCastled = true;
                        board.LastMove.MovingPieceSecondary = new PieceMoving(board.Squares[61].Piece.PieceColor,
                            board.Squares[61].Piece.PieceType, board.Squares[61].Piece.Moved, 63, 61);
                        board.Squares[61].Piece.Moved = true;
                    }
                }
                //Castle Left
                else if (dstPosition == 58)
                {
                    //Ok we are casteling we need to move the Rook
                    if (board.Squares[56].Piece != null)
                    {
                        board.Squares[59].Piece = board.Squares[56].Piece;
                        board.Squares[56].Piece = null;
                        board.WhiteCastled = true;
                        board.LastMove.MovingPieceSecondary = new PieceMoving(board.Squares[59].Piece.PieceColor,
                            board.Squares[59].Piece.PieceType, board.Squares[59].Piece.Moved, 56, 59);
                        board.Squares[59].Piece.Moved = true;
                    }
                }
            }
            else if (piece.PieceColor == ChessPieceColor.Black && srcPosition == 4)
            {
                if (dstPosition == 6)
                {
                    //Ok we are casteling we need to move the Rook
                    if (board.Squares[7].Piece != null)
                    {
                        board.Squares[5].Piece = board.Squares[7].Piece;
                        board.Squares[7].Piece = null;
                        board.BlackCastled = true;
                        board.LastMove.MovingPieceSecondary = new PieceMoving(board.Squares[5].Piece.PieceColor,
                            board.Squares[5].Piece.PieceType, board.Squares[5].Piece.Moved, 7, 5);
                        board.Squares[5].Piece.Moved = true;
                    }
                }
                //Castle Left
                else if (dstPosition == 2)
                {
                    //Ok we are casteling we need to move the Rook
                    if (board.Squares[0].Piece != null)
                    {
                        board.Squares[3].Piece = board.Squares[0].Piece;
                        board.Squares[0].Piece = null;
                        board.BlackCastled = true;
                        board.LastMove.MovingPieceSecondary = new PieceMoving(board.Squares[3].Piece.PieceColor,
                            board.Squares[3].Piece.PieceType, board.Squares[3].Piece.Moved, 0, 3);
                        board.Squares[3].Piece.Moved = true;
                    }
                }
            }
        }

        #endregion

        #region InternalMethods

        //Fast Copy will copy only the values that must persist from one board to another during move generation
        internal Board FastCopy()
        {
            var clonedBoard = new Board(Squares)
            {
                EndGamePhase = EndGamePhase,
                WhoseMove = WhoseMove,
                MoveCount = MoveCount,
                HalfMoveClock = HalfMoveClock,
                BlackCastled = BlackCastled,
                WhiteCastled = WhiteCastled,
                WhiteCanCastle = WhiteCanCastle,
                BlackCanCastle = BlackCanCastle
            };

            WhiteAttackBoard = new bool[64];
            BlackAttackBoard = new bool[64];

            return clonedBoard;
        }

        internal static MoveContent MovePiece(Board board, byte srcPosition, byte dstPosition,
            ChessPieceType promoteToPiece)
        {
            var piece = board.Squares[srcPosition].Piece;

            //Record my last move
            board.LastMove = new MoveContent();

            if (piece.PieceColor == ChessPieceColor.Black) board.MoveCount++;
            //Add One to HalfMoveClockCount to check for 50 move limit.
            board.HalfMoveClock++;

            //En Passant
            if (board.EnPassantPosition > 0)
                board.LastMove.EnPassantOccured =
                    SetEnpassantMove(board, srcPosition, dstPosition, piece.PieceColor);

            if (!board.LastMove.EnPassantOccured)
            {
                var sqr = board.Squares[dstPosition];

                if (sqr.Piece != null)
                {
                    board.LastMove.TakenPiece = new PieceTaken(sqr.Piece.PieceColor, sqr.Piece.PieceType,
                        sqr.Piece.Moved, dstPosition);
                    board.HalfMoveClock = 0;
                }
                else
                {
                    board.LastMove.TakenPiece = new PieceTaken(ChessPieceColor.White, ChessPieceType.None, false,
                        dstPosition);
                }
            }

            board.LastMove.MovingPiecePrimary = new PieceMoving(piece.PieceColor, piece.PieceType, piece.Moved,
                srcPosition, dstPosition);

            //Delete the piece in its source position
            board.Squares[srcPosition].Piece = null;

            //Add the piece to its new position
            piece.Moved = true;
            piece.Selected = false;
            board.Squares[dstPosition].Piece = piece;

            //Reset EnPassantPosition
            board.EnPassantPosition = 0;

            //Record En Passant if Pawn Moving
            if (piece.PieceType == ChessPieceType.Pawn)
            {
                board.HalfMoveClock = 0;
                RecordEnPassant(piece.PieceColor, piece.PieceType, board, srcPosition, dstPosition);
            }

            board.WhoseMove = board.WhoseMove == ChessPieceColor.White
                ? ChessPieceColor.Black
                : ChessPieceColor.White;

            KingCastle(board, piece, srcPosition, dstPosition);

            //Promote Pawns 
            board.LastMove.PawnPromotedTo = PromotePawns(board,
                piece,
                dstPosition,
                promoteToPiece)
                ? promoteToPiece
                : ChessPieceType.None;

            if (board.HalfMoveClock >= 100) board.StaleMate = true;

            return board.LastMove;
        }

        private static string GetColumnFromByte(byte column)
        {
            switch (column)
            {
                case 0:
                    return "a";
                case 1:
                    return "b";
                case 2:
                    return "c";
                case 3:
                    return "d";
                case 4:
                    return "e";
                case 5:
                    return "f";
                case 6:
                    return "g";
                case 7:
                    return "h";
                default:
                    return "a";
            }
        }

        public new string ToString()
        {
            return Fen(false, this);
        }

        internal static string Fen(bool boardOnly, Board board)
        {
            var output = string.Empty;
            byte blankSquares = 0;

            for (byte x = 0; x < 64; x++)
            {
                var index = x;

                if (board.Squares[index].Piece != null)
                {
                    if (blankSquares > 0)
                    {
                        output += blankSquares.ToString();
                        blankSquares = 0;
                    }

                    if (board.Squares[index].Piece.PieceColor == ChessPieceColor.Black)
                        output += Piece.GetPieceTypeShort(board.Squares[index].Piece.PieceType).ToLower();
                    else
                        output += Piece.GetPieceTypeShort(board.Squares[index].Piece.PieceType);
                }
                else
                {
                    blankSquares++;
                }

                if (x % 8 == 7)
                {
                    if (blankSquares > 0)
                    {
                        output += blankSquares.ToString();
                        output += "/";
                        blankSquares = 0;
                    }
                    else
                    {
                        if (x > 0 && x != 63) output += "/";
                    }
                }
            }

            if (output.EndsWith("/")) output = output.TrimEnd('/');

            if (board.WhoseMove == ChessPieceColor.White)
                output += " w ";
            else
                output += " b ";

            var castle = "-";

            if (board.WhiteCastled == false)
                if (board.Squares[60].Piece != null)
                    if (board.Squares[60].Piece.Moved == false)
                    {
                        if (board.Squares[63].Piece != null)
                            if (board.Squares[63].Piece.Moved == false)
                                castle += "K";
                        if (board.Squares[56].Piece != null)
                            if (board.Squares[56].Piece.Moved == false)
                                castle += "Q";
                    }

            if (board.BlackCastled == false)
                if (board.Squares[4].Piece != null)
                    if (board.Squares[4].Piece.Moved == false)
                    {
                        if (board.Squares[7].Piece != null)
                            if (board.Squares[7].Piece.Moved == false)
                                castle += "k";
                        if (board.Squares[0].Piece != null)
                            if (board.Squares[0].Piece.Moved == false)
                                castle += "q";
                    }

            if (castle != "-") castle = castle.TrimStart('-');
            output += castle;

            if (board.EnPassantPosition != 0)
                output += " " + GetColumnFromByte((byte) (board.EnPassantPosition % 8)) + "" +
                          (byte) (8 - (byte) (board.EnPassantPosition / 8)) + " ";
            else
                output += " - ";

            if (!boardOnly)
            {
                output += board.HalfMoveClock + " ";
                output += board.MoveCount;
            }

            return output.Trim();
        }

        #endregion
    }
}