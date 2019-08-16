using System;
using UnityEngine;

public class ScoreWindow : MonoBehaviour
{
    private void ScoreWindowActionHandler(EventParam eventParam)
    {
        switch (eventParam.TypeOfEvent)
        {
            case EventResponseType.GameManagerEvent:
            {
                if (eventParam.Response is MenuResponse result)
                    isEnabled = result.IsShowed || result.IsGameRestart;
                break;
            }
            case EventResponseType.PieceManagerEvent:
            {
                if (eventParam.Response is ScoreResponse response)
                    score = response.Score;
                break;
            }
            case EventResponseType.BoardsEvents:
                break;
            case EventResponseType.StartDialogResult:
                break;
            case EventResponseType.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    } // ReSharper disable StringLiteralTypo
    private void DrawWindow(int id)
    {
        GUI.Label(new Rect(-10, 25, 100, 25),
            "Победа\nБелых",
            new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Screen.height * 2 / 100,
                normal = {textColor = Color.white}
            });

        for (var i = 5; i < TextureWidth; i++)
        for (var j = 0; j <= TextureHeight; j++)
            Texture.SetPixel(i, j, i <= score + 32767 / 65 ? Color.white : Color.black);

        Texture.Apply();
        GUI.DrawTexture(new Rect(75, 20, 350, 25), Texture,
            ScaleMode.ScaleAndCrop, true,
            7.5f);


        GUI.Label(new Rect(415, 25, 100, 25),
            "Победа\nЧерных",
            new GUIStyle
            {
                richText = enabled,
                alignment = TextAnchor.MiddleCenter,
                fontSize = Screen.height * 2 / 100,
                normal = {textColor = Color.white}
            });


        GUI.Label(new Rect(202, 42, 100, 25), score.ToString(),
            new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Screen.height * 2 / 100,
                normal = {textColor = Color.white}
            });

        GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
    }

    #region Variables

    public bool isEnabled;

    private static Rect _scoreWindowRect = new Rect(
        (Screen.width - 200) / 3.3f,
        15,
        500,
        65);

    private Action<EventParam> Action;

    private int score;

    private const int TextureWidth = 1000;
    private const int TextureHeight = 25;
    private Texture2D Texture;

    #endregion

    #region UnityEvents

    private void Awake()
    {
        Action = ScoreWindowActionHandler;
    }

    private void OnEnable()
    {
        EventManager.StartListening(EventsNames.ScoreWindowEvent, Action);

        Texture = new Texture2D(TextureWidth, TextureHeight, TextureFormat.ARGB32, true);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventsNames.ScoreWindowEvent, Action);
    }

    private void OnGUI()
    {
        if (!isEnabled)
            return;

        _scoreWindowRect = GUI.Window(
            (int) WindowCodeId.ScoreWindow,
            _scoreWindowRect,
            DrawWindow,
            Extensions.ToString(WindowCodeId.ScoreWindow));
    }

    #endregion
}