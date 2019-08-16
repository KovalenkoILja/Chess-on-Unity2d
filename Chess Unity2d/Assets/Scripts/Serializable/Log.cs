using System;
using UnityEngine;

/// <summary>
///     A basic container for log details.
/// </summary>
[Serializable]
public struct Log
{
    [SerializeField] public int count;
    [SerializeField] public string message;
    [SerializeField] public string stackTrace;
    [SerializeField] public LogType type;

    /// <summary>
    ///     The max string length supported by UnityEngine.GUILayout.Label without triggering this error:
    ///     "String too long for TextMeshGenerator. Cutting off characters."
    /// </summary>
    [NonSerialized] private const int MaxMessageLength = 16382;

    public bool Equals(Log log)
    {
        return message == log.message && stackTrace == log.stackTrace && type == log.type;
    }

    /// <summary>
    ///     Return a truncated message if it exceeds the max message length.
    /// </summary>
    public string GetTruncatedMessage()
    {
        if (string.IsNullOrEmpty(message)) return message;

        return message.Length <= MaxMessageLength ? message : message.Substring(0, MaxMessageLength);
    }
}