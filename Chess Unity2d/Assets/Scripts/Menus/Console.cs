using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <inheritdoc />
/// <summary>
///     A console to display Unity's debug logs in-game.
/// </summary>
internal class Console : MonoBehaviour
{
    #region Methods

    private static void DrawCollapsedLog(Log log)
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label(log.GetTruncatedMessage());
        GUILayout.FlexibleSpace();
        GUILayout.Label(log.count.ToString(), GUI.skin.box);

        GUILayout.EndHorizontal();
    }

    private static void DrawExpandedLog(Log log)
    {
        for (var i = 0; i < log.count; i += 1) GUILayout.Label(log.GetTruncatedMessage());
    }

    private void DrawLog(Log log)
    {
        GUI.contentColor = LogTypeColors[log.type];

        if (isCollapsed)
            DrawCollapsedLog(log);
        else
            DrawExpandedLog(log);
    }

    private void DrawLogList()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        // Used to determine height of accumulated log labels.
        GUILayout.BeginVertical();

        var visibleLogs = logs.Where(IsLogVisible);

        foreach (var log in visibleLogs) DrawLog(log);

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

    private void DrawToolbar()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button(ClearLabel)) logs.Clear();

        foreach (LogType logType in Enum.GetValues(typeof(LogType)))
        {
            var currentState = logTypeFilters[logType];
            var label = logType.ToString();
            logTypeFilters[logType] = GUILayout.Toggle(currentState, label, GUILayout.ExpandWidth(false));
            GUILayout.Space(20);
        }

        isCollapsed = GUILayout.Toggle(isCollapsed, CollapseLabel, GUILayout.ExpandWidth(false));

        GUILayout.EndHorizontal();
    }

    private void DrawWindow(int windowId)
    {
        DrawLogList();
        DrawToolbar();

        // Allow the window to be dragged by its title bar.
        GUI.DragWindow(titleBarRect);
    }

    private Log? GetLastLog()
    {
        if (logs.Count == 0) return null;

        return logs.Last();
    }

    private void UpdateQueuedLogs()
    {
        while (queuedLogs.TryDequeue(out var log)) ProcessLogItem(log);
    }

    private void HandleLogThreaded(string message, string stackTrace, LogType type)
    {
        var log = new Log
        {
            count = 1,
            message = message,
            stackTrace = stackTrace,
            type = type
        };

        // Queue the log into a ConcurrentQueue to be processed later in the Unity main thread,
        // so that we don't get GUI-related errors for logs coming from other threads
        queuedLogs.Enqueue(log);
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

    private bool IsLogVisible(Log log)
    {
        return logTypeFilters[log.type];
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

    private void TrimExcessLogs()
    {
        if (!restrictLogCount) return;

        var amountToRemove = logs.Count - maxLogCount;

        if (amountToRemove <= 0) return;

        logs.RemoveRange(0, amountToRemove);
    }

    #endregion

    #region Varibles

    private const int Margin = 20;
    private const string WindowTitle = "Console";

    private static readonly GUIContent ClearLabel = new GUIContent("Clear", "Clear the contents of the console.");
    private static readonly GUIContent CollapseLabel = new GUIContent("Collapse", "Hide repeated messages.");

    private static readonly Dictionary<LogType, Color> LogTypeColors = new Dictionary<LogType, Color>
    {
        {LogType.Assert, Color.white},
        {LogType.Error, Color.red},
        {LogType.Exception, Color.red},
        {LogType.Log, Color.white},
        {LogType.Warning, Color.yellow}
    };

    private readonly List<Log> logs = new List<Log>();

    private readonly Dictionary<LogType, bool> logTypeFilters = new Dictionary<LogType, bool>
    {
        {LogType.Assert, true},
        {LogType.Error, true},
        {LogType.Exception, true},
        {LogType.Log, true},
        {LogType.Warning, true}
    };

    private readonly ConcurrentQueue<Log> queuedLogs = new ConcurrentQueue<Log>();
    private readonly Rect titleBarRect = new Rect(0, 0, 10000, 20);

    private bool isCollapsed;
    private bool isVisible;

    private Vector2 scrollPosition;
    private Rect windowRect = new Rect(Margin, Margin, Screen.width - Margin * 2, Screen.height - Margin * 2);

    #endregion

    #region Inspector Settings

    /// <summary>
    ///     The hotkey to show and hide the console window.
    /// </summary>
    public KeyCode toggleKey = KeyCode.BackQuote;

    /// <summary>
    ///     Whether to open as soon as the game starts.
    /// </summary>
    public bool openOnStart;

    /// <summary>
    ///     Whether to open the window by shaking the device (mobile-only).
    /// </summary>
    public bool shakeToOpen = true;

    /// <summary>
    ///     The (squared) acceleration above which the window should open.
    /// </summary>
    public float shakeAcceleration = 3f;

    /// <summary>
    ///     Whether to only keep a certain number of logs, useful if memory usage is a concern.
    /// </summary>
    public bool restrictLogCount;

    /// <summary>
    ///     Number of logs to keep before removing old ones.
    /// </summary>
    public int maxLogCount = 1000;

    #endregion

    #region MonoBehaviour Messages

    private void OnDisable()
    {
        Application.logMessageReceivedThreaded -= HandleLogThreaded;
    }

    private void OnEnable()
    {
        Application.logMessageReceivedThreaded += HandleLogThreaded;
    }

    private void OnGUI()
    {
        if (!isVisible) return;

        windowRect = GUILayout.Window(
            (int) WindowCodeId.Console,
            windowRect,
            DrawWindow,
            WindowTitle);
    }

    private void Start()
    {
        if (openOnStart) isVisible = true;
    }

    private void Update()
    {
        UpdateQueuedLogs();

        if (Input.GetKeyDown(toggleKey)) isVisible = !isVisible;

        if (shakeToOpen && Input.acceleration.sqrMagnitude > shakeAcceleration) isVisible = true;
    }

    #endregion
}