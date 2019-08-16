using System;
using System.Collections.Generic;
using System.Linq;
using ChessEngine;
using U3D.Threading.Tasks;
using UnityEngine;

// ReSharper disable StringLiteralTypo
public class PieceManager : MonoBehaviour
{
    #region Variables

    private struct EngineMove
    {
        public MoveContent MoveContent;
        public string Message;
    }

    private readonly Dictionary<string, Type> PieceLibrary = new Dictionary<string, Type>
    {
        {"P", typeof(Pawn)},
        {"R", typeof(Rook)},
        {"KN", typeof(Knight)},
        {"B", typeof(Bishop)},
        {"K", typeof(King)},
        {"Q", typeof(Queen)}
    };

    private readonly string[] PieceOrder = new string[16]
    {
        "P", "P", "P", "P", "P", "P", "P", "P",
        "R", "KN", "B", "Q", "K", "B", "KN", "R"
    };

    private Action<EventParam> SettingChangeResponse;

    public List<BasePiece> blackPieces;
    public Board board;

    [HideInInspector] public GameSetup gameSetup;

    public GameObject piecePrefab;

    public List<BasePiece> promotedPieces = new List<BasePiece>();
    public List<BasePiece> whitePieces;

    private readonly ConcurrentQueue<EngineMove> queuedMoves = new ConcurrentQueue<EngineMove>();

    [HideInInspector] public Engine engine;

    #endregion

    #region UnityEvents

    private void Awake()
    {
        SettingChangeResponse = ChangeSettingsResponseFunction;
    }

    private void OnEnable()
    {
        EventManager.StartListening(EventsNames.SettingsEvent, SettingChangeResponse);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventsNames.SettingsEvent, SettingChangeResponse);
    }

    private void Update()
    {
        if (queuedMoves == null) return;
        while (queuedMoves.TryDequeue(out var move)) ProceedMove(move.MoveContent, move.Message);
    }

    private void ChangeSettingsResponseFunction(EventParam eventParam)
    {
        if (!(eventParam.Response is SettingsResponse results))
            return;

        engine.GameDifficulty = results.Difficulty;
    }

    #endregion

    #region SetupMethods

    public void Setup(Board brd, GameSetup setup)
    {
        engine = new Engine();

        if (SettingsData.Settings != null)
            engine.GameDifficulty = SettingsData.Settings.Difficulty;

        engine.PromoteToPieceType = ChessPieceType.Queen;

        gameSetup = setup;
        board = brd;

        whitePieces = CreatePieces(Color.white, Colors.WhitePieces, brd);
        blackPieces = CreatePieces(Color.black, Colors.BlackPieces, brd);

        PlacePieces(1, 0, whitePieces, brd);
        PlacePieces(6, 7, blackPieces, brd);

        SwitchSides();
    }

    public void LoadGameSave(GameData gameData, Board brd, GameSetup setup)
    {
        if (gameData == null)
        {
            Setup(brd, setup);
            return;
        }

        gameSetup = setup;
        board = brd;

        engine = new Engine(gameData.fen)
        {
            WhoseMove = gameData.whoIsMove,
            Thinking = false
        };
        if (SettingsData.Settings != null)
            engine.GameDifficulty = SettingsData.Settings.Difficulty;

        engine.PromoteToPieceType = ChessPieceType.Queen;

        foreach (var cell in gameData.cells)
        {
            var newPiece = CreatePiece(Type.GetType(cell.Type));

            if (cell.Color == Color.white)
            {
                newPiece.Setup(cell.Color, Colors.WhitePieces, this);

                newPiece.Place(brd.AllCells[cell.X, cell.Y]);
                whitePieces.Add(newPiece);
            }
            else if (cell.Color == Color.black)
            {
                newPiece.Setup(cell.Color, Colors.BlackPieces, this);

                newPiece.Place(brd.AllCells[cell.X, cell.Y]);
                blackPieces.Add(newPiece);
            }
        }

        SwitchSides();
    }

    private List<BasePiece> CreatePieces(Color teamColor, Color32 spriteColor, Board brd)
    {
        var newPieces = new List<BasePiece>();

        foreach (var piece in PieceOrder)
        {
            var newPieceObj = Instantiate(piecePrefab, transform, true);

            newPieceObj.transform.localScale = new Vector3(1, 1, 1);
            newPieceObj.transform.localRotation = Quaternion.identity;

            var key = piece;
            var pieceType = PieceLibrary[key];

            var newPiece = (BasePiece) newPieceObj.AddComponent(pieceType);
            newPieces.Add(newPiece);

            newPiece.Setup(teamColor, spriteColor, this);
        }

        return newPieces;
    }

    private BasePiece CreatePiece(Type pieceType)
    {
        var newPieceObject = Instantiate(piecePrefab, transform, true);

        newPieceObject.transform.localScale = new Vector3(1, 1, 1);
        newPieceObject.transform.localRotation = Quaternion.identity;

        var newPiece = (BasePiece) newPieceObject.AddComponent(pieceType);

        return newPiece;
    }

    private static void PlacePieces(int pawnRow, int royaltyRow, IReadOnlyList<BasePiece> pieces, Board board)
    {
        for (var i = 0; i < 8; i++)
        {
            pieces[i].Place(board.AllCells[i, pawnRow]);
            pieces[i + 8].Place(board.AllCells[i, royaltyRow]);
        }
    }

    private static void SetInteractive(IEnumerable<BasePiece> allPieces, bool value)
    {
        foreach (var piece in allPieces) piece.enabled = value;
    }

    public void ResetPieces()
    {
        engine = new Engine();
        if (SettingsData.Settings != null)
            engine.GameDifficulty = SettingsData.Settings.Difficulty;

        engine.PromoteToPieceType = ChessPieceType.Queen;

        foreach (var piece in promotedPieces)
        {
            piece.Kill();
            Destroy(piece.gameObject);
        }

        promotedPieces.Clear();

        foreach (var piece in whitePieces) piece.Kill();

        foreach (var piece in blackPieces) piece.Kill();

        whitePieces = CreatePieces(Color.white, Colors.WhitePieces, board);
        blackPieces = CreatePieces(Color.black, Colors.BlackPieces, board);

        PlacePieces(1, 0, whitePieces, board);
        PlacePieces(6, 7, blackPieces, board);
        SwitchSides();
    }

    private void PromotePiece(BasePiece pawn, Cell cell, Color teamColor, Color spriteColor)
    {
        SoundEvents.PlayPromotionSound();
        pawn.Kill();

        var promotedPiece = CreatePiece(typeof(Queen));
        promotedPiece.Setup(teamColor, spriteColor, this);

        promotedPiece.Place(cell);

        promotedPieces.Add(promotedPiece);
    }

    #endregion

    #region Methods

    public void PieceMoveEvent(BasePiece piece, Cell currentCell, Cell targetCell)
    {
        var toX = (byte) targetCell.boardPosition.x;
        var toY = (byte) (7 - targetCell.boardPosition.y);
        var fromX = (byte) currentCell.boardPosition.x;
        var fromY = (byte) (7 - currentCell.boardPosition.y);
        if (engine.MovePiece(
            fromX, fromY, toX, toY))
        {
            var move = new EngineMove
            {
                MoveContent = engine.GetMoveHistory().ToArray()[0],
                Message = "Ход Игрока\n"
            };

            queuedMoves.Enqueue(move);
        }
        else
        {
            SoundEvents.PlayRejectSound();
            StaticEvents.LogMsgEvent("Недопустимый ход!", LogType.Warning);
            piece.Place(board.AllCells[currentCell.boardPosition.x,
                currentCell.boardPosition.y]);
        }
    }

    private static void CheckMove(Engine engine)
    {
        string msg;
        if (engine.GetWhiteCheck())
        {
            msg = "Шах белым";
            Debug.Log(msg);
            StaticEvents.LogMsgEvent(msg, LogType.Warning);
            SoundEvents.PlayCheckSound();
        }

        if (engine.GetBlackCheck())
        {
            msg = "Шах черным";
            Debug.Log(msg);
            StaticEvents.LogMsgEvent(msg, LogType.Warning);
            SoundEvents.PlayCheckSound();
        }

        if (!engine.IsGameOver())
            return;

        var result = "";
        var resultPgn = PGN.Result.Ongoing;
        var state = EndGamesStates.None;

        if (engine.StaleMate)
        {
            resultPgn = PGN.Result.Tie;
            if (engine.InsufficientMaterial)
            {
                state = EndGamesStates.StaleMateByInsufficientMaterial;
                result = "Ничья, недостаточно материала для мата";
            }
            else if (engine.RepeatedMove)
            {
                state = EndGamesStates.StaleMateByRepeatedMove;
                result = "Ничья после троекратного повторения";
            }
            else if (engine.FiftyMove)
            {
                state = EndGamesStates.StaleMateByFiftyMove;
                result = "Ничья по правилу 50 ходов";
            }
            else
            {
                state = EndGamesStates.StaleMate;
                result = "Пат";
            }

            Debug.Log(result);
            StaticEvents.LogMsgEvent(result, LogType.Warning);
            SoundEvents.PlayCheckSound();
        }

        if (engine.GetWhiteMate())
        {
            resultPgn = PGN.Result.Black;
            result = "Шах и мат белым";
            Debug.Log(result);
            StaticEvents.LogMsgEvent(result, LogType.Warning);
            SoundEvents.PlayCheckSound();
            state = EndGamesStates.WhiteMate;
        }

        if (engine.GetBlackMate())
        {
            resultPgn = PGN.Result.White;
            result = "Шах и мат черным";
            Debug.Log(result);
            StaticEvents.LogMsgEvent(result, LogType.Warning);
            SoundEvents.PlayCheckSound();
            state = EndGamesStates.BlackMate;
        }

        GameStatData.PGN = GeneratePNG(engine, resultPgn);

        SoundEvents.PlayVictorySound();
        msg = "Конец игры";

        StaticEvents.LogMsgEvent(msg, LogType.Exception);
        SoundEvents.PlayCheckSound();
        EndGameEvents.EndGameEvent(state, result, engine.GetScore());
    }

    // ReSharper disable once InconsistentNaming
    private static string GeneratePNG(Engine engine, PGN.Result result)
    {
        var white = "";
        var black = "";

        switch (GameStatData.GameSetup)
        {
            case GameSetup.PlayerWhiteVsPlayerBlack:
                white = "Игрок";
                black = "Игрок";
                break;
            case GameSetup.PlayerWhiteVsAiBlack:
                white = "Игрок";
                black = "Компьютер";
                break;
            case GameSetup.PlayerBlackVsAiWhite:
                white = "Компьютер";
                black = "Игрок";
                break;
            case GameSetup.AiWhiteVsAiBlack:
                white = "Компьютер";
                black = "Компьютер";
                break;
            case GameSetup.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return PGN.GeneratePGN(engine.GetMoveHistory(), white, black, result);
    }

    // ReSharper disable IdentifierTypo
    public IEnumerable<Cell> GetValidMoves(Cell cell)
    {
        var list = new List<Cell>();

        var currX = (byte) cell.boardPosition.x;
        var currY = (byte) (7 - cell.boardPosition.y);

        var pieceTypeAt = engine.GetPieceTypeAt(currX, currY);
        var pieceColorAt = engine.GetPieceColorAt(currX, currY);
        Debug.Log(pieceTypeAt);
        Debug.Log(pieceColorAt);
        var movesSquare = engine.GetValidMovesSquare(currX, currY);

        IEnumerable<byte> square = movesSquare as byte[] ?? movesSquare.ToArray();
        if (!square.Any())
            return list;
        foreach (var p in square)
        {
            Debug.Log(p);
            var vector2Int = Extensions.FromPosition(p);
            list.Add(board.AllCells[vector2Int.x, vector2Int.y]);
        }

        return list;
    }

    private void MakeEngineMove()
    {
        Task<MoveContent>.Run(() =>
        {
            lock (engine)
            {
                engine.AiPonderMove();
                return engine.GetMoveHistory().ToArray()[0];
            }
        }).ContinueInMainThreadWith(t =>
        {
            Debug.Log("AI Move: " + t.Result.GetPureCoordinateNotation());

            if (t.Result == null) return;
            var move = new EngineMove
            {
                MoveContent = t.Result,
                Message = "Ход (" + engine.GameDifficulty + "AI)\n"
            };

            queuedMoves.Enqueue(move);
        });
    }

    private void ProceedMove(MoveContent lastMove, string message)
    {
        if (lastMove.TakenPiece.PieceType.ToString() != "None")
        {
            var takenSrcCell = Extensions.FromPosition(lastMove.TakenPiece.Position);
            var takenPiece = board.AllCells[takenSrcCell.x, takenSrcCell.y].currentPiece;

            if (takenPiece != null)
            {
                message += Extensions.PieceToStr(takenPiece) + " взятие ";
                takenPiece.Kill();
            }
        }

        if (lastMove.MovingPiecePrimary.PieceType.ToString() != "None")
        {
            var primarySrcCell = Extensions.FromPosition(lastMove.MovingPiecePrimary.SrcPosition);
            var primaryDstCell = Extensions.FromPosition(lastMove.MovingPiecePrimary.DstPosition);

            var piecePrimary = board.AllCells[primarySrcCell.x, primarySrcCell.y].currentPiece;
            if (piecePrimary != null)
            {
                if (lastMove.PawnPromotedTo == ChessPieceType.Queen)
                {
                    message += " Pawn повышение Queen ";
                    PromotePiece(piecePrimary as Pawn, board.AllCells[primaryDstCell.x, primaryDstCell.y],
                        piecePrimary.mainColor,
                        piecePrimary.mainColor == Color.black ? Colors.BlackPieces : Colors.WhitePieces);
                }
                else
                {
                    piecePrimary.MovePieceToCell(board.AllCells[primaryDstCell.x, primaryDstCell.y]);
                    SoundEvents.PlayMoveSound();
                    message += Extensions.PieceToStr(piecePrimary);
                }
            }
            else
            {
                Debug.Log("Piece not found and not created!");
            }
        }

        if (lastMove.MovingPieceSecondary.PieceType.ToString() != "None")
        {
            var secondarySrcCell = Extensions.FromPosition(lastMove.MovingPieceSecondary.SrcPosition);
            var secondaryDstCell = Extensions.FromPosition(lastMove.MovingPieceSecondary.DstPosition);
            var pieceSecondary = board.AllCells[secondarySrcCell.x, secondarySrcCell.y].currentPiece;

            if (pieceSecondary != null)
            {
                message += " рокировка ";
                pieceSecondary.Place(board.AllCells[secondaryDstCell.x, secondaryDstCell.y]);
                SoundEvents.PlayMoveSound();
                message += Extensions.PieceToStr(pieceSecondary);
            }
            else
            {
                var msg = "pieceSecondary not found!";

                msg += " Src X " + secondarySrcCell.x + " Y " + secondarySrcCell.y;
                msg += " Dst X " + secondaryDstCell.x + " Y " + secondaryDstCell.y;
                Debug.LogError(msg);
                //StaticEvents.LogMsgEvent(msg, LogType.Exception);
            }
        }

        if (lastMove.EnPassantOccured) message += " взятие на проходе ";

        message += "\n" + lastMove.ToString();
        StaticEvents.LogMsgEvent(message, LogType.Log);
        CheckMove(engine);
        SwitchSides();
    }

    #endregion

    #region SwitchSides

    public void SwitchSides()
    {
        switch (gameSetup)
        {
            case GameSetup.PlayerWhiteVsPlayerBlack:
                PlayerWhiteVsPlayerBlack();
                break;
            case GameSetup.PlayerBlackVsAiWhite:
                PlayerBlackVsAiWhite();
                break;
            case GameSetup.PlayerWhiteVsAiBlack:
                PlayerWhiteVsAiBlack();
                break;
            case GameSetup.AiWhiteVsAiBlack:
                AiWhiteVsAiBlack();
                break;
            case GameSetup.None:
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(gameSetup), gameSetup, "GameSetup Error!");
        }
    }

    private void PlayerWhiteVsPlayerBlack()
    {
        if (engine.IsGameOver())
            return;

        switch (engine.WhoseMove)
        {
            case ChessPieceColor.White:
                SetInteractive(whitePieces, true);
                SetInteractive(blackPieces, false);

                foreach (var piece in promotedPieces)
                {
                    var isBlack = piece.mainColor != Color.white;
                    piece.enabled = !isBlack;
                }

                StaticEvents.ReportOfTurnChange(false, engine.GetScore());
                break;
            case ChessPieceColor.Black:
                SetInteractive(whitePieces, false);
                SetInteractive(blackPieces, true);

                foreach (var piece in promotedPieces)
                {
                    var isBlack = piece.mainColor != Color.white;
                    piece.enabled = isBlack;
                }

                StaticEvents.ReportOfTurnChange(true, engine.GetScore());
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void PlayerBlackVsAiWhite()
    {
        if (engine.IsGameOver())
            return;

        if (engine.Thinking)
            return;

        switch (engine.WhoseMove)
        {
            case ChessPieceColor.White:
                SetInteractive(whitePieces, false);
                SetInteractive(blackPieces, false);

                StaticEvents.ReportOfTurnChange(false, engine.GetScore());

                engine.HumanPlayer = ChessPieceColor.Black;

                if (engine.WhoseMove != engine.HumanPlayer)
                    MakeEngineMove();

                break;
            case ChessPieceColor.Black:
                SetInteractive(whitePieces, false);
                SetInteractive(blackPieces, true);
                foreach (var piece in promotedPieces)
                    if (piece.mainColor == Colors.BlackPieces)
                        piece.enabled = true;
                StaticEvents.ReportOfTurnChange(true, engine.GetScore());
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void PlayerWhiteVsAiBlack()
    {
        Debug.Log("PlayerWhiteVsAiBlack");

        if (engine.IsGameOver())
            return;

        if (engine.Thinking)
            return;

        switch (engine.WhoseMove)
        {
            case ChessPieceColor.White:

                Debug.Log("WhoseMove White");

                SetInteractive(whitePieces, true);
                SetInteractive(blackPieces, false);
                foreach (var piece in promotedPieces)
                    if (piece.mainColor == Colors.WhitePieces)
                        piece.enabled = true;
                StaticEvents.ReportOfTurnChange(false, engine.GetScore());

                break;
            case ChessPieceColor.Black:

                Debug.Log("WhoseMove Black");

                SetInteractive(whitePieces, false);
                SetInteractive(blackPieces, false);

                StaticEvents.ReportOfTurnChange(true, engine.GetScore());

                if (engine.WhoseMove != engine.HumanPlayer)
                    MakeEngineMove();

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void AiWhiteVsAiBlack()
    {
        Debug.Log("AiWhiteVsAiBlack");
        SetInteractive(whitePieces, false);
        SetInteractive(blackPieces, false);

        if (engine.IsGameOver())
            return;
        if (engine.Thinking)
            return;

        switch (engine.WhoseMove)
        {
            case ChessPieceColor.White:
                Debug.Log("WhoseMove White");
                StaticEvents.ReportOfTurnChange(false, engine.GetScore());

                engine.HumanPlayer = ChessPieceColor.Black;

                if (engine.WhoseMove != engine.HumanPlayer)
                    MakeEngineMove();

                break;
            case ChessPieceColor.Black:

                Debug.Log("WhoseMove Black");
                StaticEvents.ReportOfTurnChange(true, engine.GetScore());

                engine.HumanPlayer = ChessPieceColor.White;

                if (engine.WhoseMove != engine.HumanPlayer)
                    MakeEngineMove();

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #endregion
}