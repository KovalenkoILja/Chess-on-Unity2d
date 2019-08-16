using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LogWindow : MonoBehaviour
{
    #region Methods

    private void LogActionHandler(EventParam obj)
    {
        switch (obj.TypeOfEvent)
        {
            case EventResponseType.BoardsEvents:
                break;
            case EventResponseType.StartDialogResult:
                break;
            case EventResponseType.GameManagerEvent:

                switch (obj.Response)
                {
                    case MenuResponse result:
                        isEnabled = result.IsShowed;
                        break;
                    case LogResponse response:
                        if (response.IsNewGame)
                        {
                            logs.Clear();
                            SaveGameData.LogList.Clear();
                        }

                        if (!_isMute)
                            notificationSource.PlayOneShot(notificationSound);

                        queuedLogs.Enqueue(new Log
                        {
                            count = 1,
                            type = response.LogType,
                            message = response.Message
                        });
                        break;
                }

                break;
            case EventResponseType.PieceManagerEvent:
                break;
            case EventResponseType.None:
                break;
            case EventResponseType.AudioManagerEvent:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    private void DrawWindow(int windowId)
    {
        DrawLogList();
        DrawToolbar();
        // Allow the window to be dragged by its title bar.
        GUI.DragWindow(titleBarRect);
    }

    private void DrawToolbar()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button(ClearLabel)) logs.Clear();

        var currentState = _isMute;
        var label = _isMute ? "Звук OFF" : "Звук ON";
        _isMute = GUILayout.Toggle(currentState, label, GUILayout.ExpandWidth(false));

        //isCollapsed = GUILayout.Toggle(isCollapsed, CollapseLabel, GUILayout.ExpandWidth(false));

        GUILayout.EndHorizontal();
    }

    private void DrawLogList()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        // Used to determine height of accumulated log labels.
        GUILayout.BeginVertical();

        var visibleLogs = logs.Where(IsLogVisible);

        foreach (var log in visibleLogs)
            DrawLog(log);

        GUILayout.EndVertical();
        var innerScrollRect = GUILayoutUtility.GetLastRect();
        GUILayout.EndScrollView();
        var outerScrollRect = GUILayoutUtility.GetLastRect();

        // If we're scrolled to bottom now, guarantee that it continues to be in next cycle.
        if (Event.current.type == EventType.Repaint && IsScrolledToBottom(innerScrollRect, outerScrollRect))
            ScrollToBottom();

        // Ensure GUI colour is reset before drawing other components.
        GUI.contentColor = Color.white;
    }

    private bool IsLogVisible(Log log)
    {
        return logTypeFilters[log.type];
    }

    private static void DrawLog(Log log)
    {
        GUI.contentColor = LogTypeColors[log.type];

        DrawExpandedLog(log);
    }

    private static void DrawExpandedLog(Log log)
    {
        for (var i = 0; i < log.count; i += 1)
            GUILayout.Label(log.GetTruncatedMessage());
    }

    private bool IsScrolledToBottom(Rect innerScrollRect, Rect outerScrollRect)
    {
        var innerScrollHeight = innerScrollRect.height;

        // Take into account extra padding added to the scroll container.
        var outerScrollHeight = outerScrollRect.height - GUI.skin.box.padding.vertical;

        // If contents of scroll view haven't exceeded outer container, treat it as scrolled to bottom.
        if (outerScrollHeight > innerScrollHeight) return true;

        // Scrolled to bottom (with error margin for float math)
        return Mathf.Approximately(innerScrollHeight, scrollPosition.y + outerScrollHeight);
    }

    private void ScrollToBottom()
    {
        scrollPosition = new Vector2(0, int.MaxValue);
    }

    private void UpdateQueuedLogs()
    {
        while (queuedLogs.TryDequeue(out var log))
        {
            ProcessLogItem(log);
            SaveGameData.LogList.Add(log);
        }
    }

    private void ProcessLogItem(Log log)
    {
        var lastLog = GetLastLog();
        var isDuplicateOfLastLog = lastLog.HasValue && log.Equals(lastLog.Value);

        if (isDuplicateOfLastLog)
        {
            // Replace previous log with incremented count instead of adding a new one.
            log.count = lastLog.Value.count + 1;
            logs[logs.Count - 1] = log;
        }
        else
        {
            logs.Add(log);
            TrimExcessLogs();
        }
    }

    private Log? GetLastLog()
    {
        if (logs.Count == 0) return null;

        return logs.Last();
    }

    private void TrimExcessLogs()
    {
        const int amountToRemove = 0;

        logs.RemoveRange(0, amountToRemove);
    }

    #endregion

    #region Variables

    public bool isEnabled;
    public AudioClip notificationSound;
    private AudioSource notificationSource;

    private static bool _isMute;
    private const int Margin = 20;
    private const string WindowTitle = "Журнал";
    private Vector2 scrollPosition;

    private Rect windowRect = new Rect(
        900 - Margin, Margin,
        Screen.width - 850 - Margin * 2,
        Screen.height - Margin * 2);

    private static readonly GUIContent ClearLabel = new GUIContent("Очистить", "Очистить содержимое журнала.");
    private readonly Rect titleBarRect = new Rect(0, 0, 10000, 20);

    private readonly ConcurrentQueue<Log> queuedLogs = new ConcurrentQueue<Log>();
    private readonly List<Log> logs = new List<Log>();

    private readonly Dictionary<LogType, bool> logTypeFilters = new Dictionary<LogType, bool>
    {
        {LogType.Assert, true},
        {LogType.Error, true},
        {LogType.Exception, true},
        {LogType.Log, true},
        {LogType.Warning, true}
    };

    private static readonly Dictionary<LogType, Color> LogTypeColors = new Dictionary<LogType, Color>
    {
        {LogType.Assert, Color.white},
        {LogType.Error, Color.red},
        {LogType.Exception, Color.red},
        {LogType.Log, Color.white},
        {LogType.Warning, Color.yellow}
    };

    private Action<EventParam> LogAction;

    #endregion

    #region UnityEvents

    private void Start()
    {
        notificationSource = GetComponent<AudioSource>();
        notificationSource.clip = notificationSound;
        notificationSource.playOnAwake = false;
    }

    private void Awake()
    {
        LogAction = LogActionHandler;
        if (SaveGameData.LogList == null)
            SaveGameData.LogList = new List<Log>();
        else if (SaveGameData.LogList.Count > 0)
            foreach (var log in SaveGameData.LogList)
                queuedLogs.Enqueue(log);
    }

    private void OnEnable()
    {
        EventManager.StartListening(EventsNames.LogEvent, LogAction);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventsNames.LogEvent, LogAction);
    }

    private void OnGUI()
    {
        if (!isEnabled)
            return;

        windowRect = GUILayout.Window(
            (int) WindowCodeId.LogWindow,
            windowRect,
            DrawWindow,
            WindowTitle);
    }

    private void Update()
    {
        UpdateQueuedLogs();
    }

    #endregion
}