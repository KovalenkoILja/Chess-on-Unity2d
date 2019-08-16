using System;
using U3D.Threading;
using U3D.Threading.Tasks;
using UnityEngine;
using static SaveGameHelper;

// ReSharper disable StringLiteralTypo
public class GameManager : MonoBehaviour
{
    #region Variables

    public Board board;
    public PieceManager pieceManager;
    public GameObject startDialog;

    private bool IsPaused;
    private bool IsStartDialogue;
    private float Playtime;

    private Action<EventParam> DialogEnd;

    private Action<EventParam> BoardEvent;
    //private GameSetup GameSetup;

    #endregion

    #region UnityEvents

    private void Awake()
    {
        Dispatcher.Initialize();

        IsPaused = false;

        if (SaveGameData.Data == null ||
            SaveGameData.Data.gameSetup == GameSetup.None)
        {
            IsStartDialogue = true;
            startDialog.SetActive(IsStartDialogue);
        }
        else
        {
            IsStartDialogue = false;
            startDialog.SetActive(IsStartDialogue);
        }

        BoardEvent = BoardEventHandler;
        DialogEnd = DialogEndFunction;
    }

    private void Start()
    {
        if (startDialog.activeSelf)
            return;

        //GameSetup = GameSetup.Player_White_VS_AI_Black; 

        if (SaveGameData.Data != null)
            LoadSaveGame();
    }

    private void OnEnable()
    {
        EventManager.StartListening(EventsNames.DialogEndEvent, DialogEnd);
        EventManager.StartListening(EventsNames.BoardsEvent, BoardEvent);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventsNames.DialogEndEvent, DialogEnd);
        EventManager.StopListening(EventsNames.BoardsEvent, BoardEvent);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Pause Button pressed");

            if (IsStartDialogue) return;

            IsPaused = !IsPaused;

            MenuEvents.PauseGame(IsPaused);
        }
    }

    #endregion

    #region Methods

    private void BoardEventHandler(EventParam boardEvent)
    {
        if (boardEvent.TypeOfEvent != EventResponseType.BoardsEvents)
            return;

        switch (boardEvent.Response)
        {
            case BoardResponse result:
                switch (result.BoardResponseType)
                {
                    case BoardResponseType.RestartGame:
                        Debug.Log("RestartGame");
                        pieceManager.ResetPieces();
                        IsPaused = false;
                        break;
                    case BoardResponseType.SaveGame:
                        SaveGame(result);

                        break;
                    case BoardResponseType.None:
                        Debug.Log("BoardResponseType.None");
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                break;
            case EndGameResponse endGame:
                switch (endGame.EndGamesStates)
                {
                    case EndGamesStates.BlackMate:
                        Debug.Log("EndGamesStates.BlackMate");

                        GameStop(endGame);
                        break;
                    case EndGamesStates.WhiteMate:
                        Debug.Log("EndGamesStates.WhiteMate:");

                        GameStop(endGame);
                        break;
                    case EndGamesStates.StaleMate:
                        Debug.Log("EndGamesStates.StaleMate");

                        GameStop(endGame);
                        break;
                    case EndGamesStates.StaleMateByInsufficientMaterial:
                        Debug.Log("EndGamesStates.StaleMateByInsufficientMaterial");

                        GameStop(endGame);
                        break;
                    case EndGamesStates.StaleMateByRepeatedMove:
                        Debug.Log("EndGamesStates.StaleMateByRepeatedMove");

                        GameStop(endGame);
                        break;
                    case EndGamesStates.StaleMateByFiftyMove:
                        Debug.Log("EndGamesStates.StaleMateByFiftyMove");

                        GameStop(endGame);
                        break;
                    case EndGamesStates.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                break;
        }
    }

    private void SaveGame(BoardResponse result)
    {
        Debug.Log("SaveGame");
        if (string.IsNullOrEmpty(result.SaveFileName))
            return;

        Task.RunInMainThread(() =>
        {
            var gameData = new GameData();

            //var datetime = DateTime.Now.ToLocalTime().ToString(CultureInfo.CurrentCulture);

            Debug.Log("SaveGame clicked");
            //Debug.Log(datetime);

            gameData.gameSetup = pieceManager.gameSetup;
            gameData.timeFromStartGame = Playtime;
            gameData.fen = pieceManager.engine.FEN;
            gameData.whoIsMove = pieceManager.engine.WhoseMove;
            gameData.logs = SaveGameData.LogList;

            foreach (var cell in board.AllCells)
            {
                if (cell.currentPiece == null) continue;
                var pieceData = new PieceData
                {
                    Type = cell.currentPiece.GetType().ToString(),
                    Color = cell.currentPiece.mainColor,
                    X = cell.boardPosition.x,
                    Y = cell.boardPosition.y
                };
                gameData.cells.Add(pieceData);
            }

            Debug.Log("json");
            var json = JsonUtility.ToJson(gameData, true);
            Debug.Log(json);
            if (!IsSaveGameDirExist())
                CreateSaveGameDir();

            SaveFile(result.SaveFileName, json);
        });
    }

    private void GameStop(EndGameResponse result)
    {
        GameStatData.Result = result.Result;
        GameStatData.Score = result.Score;
        //MenuEvents.EndGame();
        //StaticEvents.SendWinner(result);

        StartCoroutine(Extensions.LoadingAsync("EndGameScene"));
    }

    private void DialogEndFunction(EventParam eventParam)
    {
        Debug.Log("Dialog End called!");
        Debug.Log(eventParam.TypeOfEvent);

        if (!(eventParam.Response is StartDialogResults results))
            return;

        Debug.Log(results.Setup);

        if (SaveGameData.Data == null)
            StartNewGame(results.Setup);
        else
            LoadSaveGame();

        IsStartDialogue = false;
    }

    private void StartNewGame(GameSetup resultsSetup)
    {
        board.Create();
        IsPaused = false;
        Playtime = 0;
        pieceManager.Setup(board, resultsSetup);

        MenuEvents.StartGame(true);

        GameStatData.GameSetup = resultsSetup;
    }

    private void LoadSaveGame()
    {
        board.Create();
        IsPaused = false;
        Playtime = SaveGameData.Data.timeFromStartGame;
        pieceManager.LoadGameSave(SaveGameData.Data, board, SaveGameData.Data.gameSetup);
        SaveGameData.Data = null;
        Playtime += Time.timeSinceLevelLoad;

        MenuEvents.StartGame(false);

        if (SaveGameData.Data != null)
            GameStatData.GameSetup = SaveGameData.Data.gameSetup;
    }

    #endregion
}