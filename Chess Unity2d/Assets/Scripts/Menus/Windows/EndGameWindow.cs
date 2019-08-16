using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameWindow : MonoBehaviour
{
    #region Variables

    public bool isEnabled;

    private readonly Rect EndGameRect = new Rect(
        (Screen.width - 200) / 2.0f,
        (Screen.height - 300) / 2.0f,
        200,
        260);

    private int score;
    private string result;

    private Action<EventParam> EndGameWindowAction;

    #endregion

    #region UnityEvents

    private void Awake()
    {
        EndGameWindowAction = EndGameWindowActionHandler;
    }

    private void OnEnable()
    {
        EventManager.StartListening("EndGameWindowEvent", EndGameWindowAction);
    }

    private void OnDisable()
    {
        EventManager.StopListening("EndGameWindowEvent", EndGameWindowAction);
    }

    private void OnGUI()
    {
        if (!isEnabled)
            return;

        EndGameWindowFunc();
    }

    #endregion

    #region Methods

    private void EndGameWindowActionHandler(EventParam eventParam)
    {
        switch (eventParam.TypeOfEvent)
        {
            case EventResponseType.PieceManagerEvent:
            {
                break;
            }
            case EventResponseType.GameManagerEvent:
            {
                switch (eventParam.Response)
                {
                    case EndGameResponse _:
                        result = GameStatData.Result;
                        score = GameStatData.Score;
                        break;
                    case MenuResponse response:
                        isEnabled = response.IsShowed;
                        break;
                }

                break;
            }
            case EventResponseType.BoardsEvents:
                break;
            case EventResponseType.StartDialogResult:
                break;
            case EventResponseType.None:
                break;
            case EventResponseType.AudioManagerEvent:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    // ReSharper disable StringLiteralTypo
    private void EndGameWindowFunc()
    {
        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");

        GUI.Label(EndGameRect,
            "Игра окончена\n" +
            result +
            "\n" +
            "Набранные очки: " +
            score +
            "\n" +
            "Общее время игры: " +
            TimeSpan.FromSeconds(GameStatData.Playtime)
                .ToString(@"hh\:mm\:ss\:fff") +
            "\n" +
            " Время ходов белых " + TimeSpan.FromSeconds(GameStatData.WhiteTime)
                .ToString(@"hh\:mm\:ss\:fff") + "\n" +
            " Время ходов черных " + TimeSpan.FromSeconds(GameStatData.BlackTime)
                .ToString(@"hh\:mm\:ss\:fff") + "\n",
            new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                fontSize = Screen.height * 2 / 65,
                normal = {textColor = Color.white}
            });

        if (GUI.Button(new Rect(Screen.width / 2.0f, 380,
                EndGameRect.width - 10, 35),
            "ГЛАВНОЕ МЕНЮ"))
        {
            isEnabled = !isEnabled;
            SceneManager.LoadScene("MainMenuScene");
        }

        if (GUI.Button(new Rect(Screen.width / 3f, 380,
                EndGameRect.width - 10, 35),
            "ЗАНОВО"))
        {
            isEnabled = !isEnabled;
            MenuEvents.RestartGame();
        }
    }

    #endregion
}