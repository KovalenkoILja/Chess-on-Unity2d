using System;
using UnityEngine;

public class MoveColorWindow : MonoBehaviour
{
    #region Variables

    public bool isEnabled;

    private static Rect _chessMoveColorWindowRect = new Rect(20, 70, 120, 65);

    private bool isBlackTurn;

    private float WhiteTime;
    private float BlackTime;

    private const int TextureWidth = 2;
    private const int TextureHeight = 2;
    private Texture2D Texture;


    private Action<EventParam> MoveColorWindowAction;

    #endregion

    #region UnityEvents

    private void Awake()
    {
        MoveColorWindowAction = MoveColorWindowActionHandler;
    }

    private void OnEnable()
    {
        EventManager.StartListening(EventsNames.MoveColorWindowEvent, MoveColorWindowAction);

        Texture = new Texture2D(TextureWidth, TextureHeight, TextureFormat.ARGB32, true);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventsNames.MoveColorWindowEvent, MoveColorWindowAction);
    }

    private void OnGUI()
    {
        if (!isEnabled)
            return;

        _chessMoveColorWindowRect = GUI.Window(
            (int) WindowCodeId.ChessMoveWindow,
            _chessMoveColorWindowRect,
            ColorMoveWindow,
            Extensions.ToString(WindowCodeId.ChessMoveWindow));
    }

    private void Update()
    {
        if (!isEnabled)
            return;

        if (isBlackTurn)
        {
            BlackTime += Time.unscaledDeltaTime;
            GameStatData.BlackTime = BlackTime;
        }
        else
        {
            WhiteTime += Time.unscaledDeltaTime;
            GameStatData.WhiteTime = WhiteTime;
        }
    }

    #endregion

    #region Methods

    private void MoveColorWindowActionHandler(EventParam eventParam)
    {
        switch (eventParam.TypeOfEvent)
        {
            case EventResponseType.GameManagerEvent:
            {
                if (eventParam.Response is MenuResponse result) isEnabled = result.IsShowed || result.IsGameRestart;
                break;
            }
            case EventResponseType.PieceManagerEvent:
            {
                if (eventParam.Response is PieceResponse result) isBlackTurn = result.IsBlackMove;
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
    }

    // ReSharper disable StringLiteralTypo
    private void ColorMoveWindow(int windowId)
    {
        var color = isBlackTurn ? Color.black : Color.white;

        Texture.SetPixel(0, 0, color);
        Texture.SetPixel(1, 0, Color.grey);
        Texture.SetPixel(0, 1, color);
        Texture.SetPixel(1, 1, Color.grey);
        Texture.Apply();

        GUI.DrawTexture(new Rect(10, 20, 100, 25), Texture);

        GUI.Label(new Rect(15, 20, 100, 25), color == Color.black ? "Черные" : "Белые",
            new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Screen.height * 2 / 75,
                normal = {textColor = color == Color.black ? Color.white : Color.black}
            });

        GUI.Label(new Rect(11, 41, 100, 25),
            TimeSpan.FromSeconds(isBlackTurn ? BlackTime : WhiteTime).ToString(@"hh\:mm\:ss\:fff"),
            new GUIStyle
            {
                richText = enabled,
                alignment = TextAnchor.MiddleCenter,
                fontSize = Screen.height * 2 / 85,
                normal = {textColor = new Color32(235, 235, 235, 255)}
            });

        GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
    }

    #endregion
}