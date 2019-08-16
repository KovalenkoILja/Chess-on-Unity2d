using System;
using UnityEngine;

[Serializable]
public class LogResponse : EventResponse
{
    [SerializeField] private bool isNewGame;

    [SerializeField] private LogType logType;

    [SerializeField] private string message;

    public bool IsNewGame
    {
        get => isNewGame;
        set => isNewGame = value;
    }

    public string Message
    {
        get => message;
        set => message = value;
    }

    public LogType LogType
    {
        get => logType;
        set => logType = value;
    }

    public override string Serialize()
    {
        return JsonUtility.ToJson(this);
    }

    public override EventResponse Deserialize(string obj)
    {
        return JsonUtility.FromJson<LogResponse>(obj);
    }
}