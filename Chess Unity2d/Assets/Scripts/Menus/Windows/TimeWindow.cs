using System;
using UnityEngine;

public class TimeWindow : MonoBehaviour
{
    #region Variables

    private static Rect _timeWindowRect = new Rect(20, 20, 120, 50);
    public bool isEnabled;

    private float Playtime;
    private float WhiteTime;
    private float BlackTime;

    private Action<EventParam> TimeWindowAction;

    #endregion

    #region UnityEvents

    private void Awake()
    {
        TimeWindowAction = TimeWindowActionHandler;
    }

    private void OnEnable()
    {
        EventManager.StartListening("TimeWindowEvent", TimeWindowAction);
    }

    private void OnDisable()
    {
        EventManager.StopListening("TimeWindowEvent", TimeWindowAction);
    }

    private void OnGUI()
    {
        if (!isEnabled)
            return;

        _timeWindowRect = GUI.Window(
            (int) WindowCodeId.TimeWindow,
            _timeWindowRect,
            TimeWindowFunc,
            Extensions.ToString(WindowCodeId.TimeWindow));
    }

    private void Update()
    {
        //Time.timeScale = isEnabled ? 0 : 1;
        if (!isEnabled)
            return;

        Playtime += Time.unscaledDeltaTime;
        GameStatData.Playtime = Playtime;
    }

    #endregion

    #region Methods

    private void TimeWindowActionHandler(EventParam eventParam)
    {
        if (eventParam.TypeOfEvent != EventResponseType.GameManagerEvent)
            return;

        if (!(eventParam.Response is MenuResponse result))
            return;

        if (result.IsGameEnd) GameStatData.Playtime = Playtime;

        isEnabled = result.IsShowed || result.IsGameRestart;

        if (result.IsGameRestart)
            Playtime = 0;
    }

    private void TimeWindowFunc(int windowId)
    {
        GUI.Label(new Rect(10, 20, 100, 25),
            TimeSpan.FromSeconds(Playtime).ToString(@"hh\:mm\:ss\:fff"),
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