using System;
using UnityEngine;

[Serializable]
public class MenuResponse : EventResponse
{
    [SerializeField] private bool isGameEnd;

    [SerializeField] private bool isGameRestart;

    [SerializeField] private bool isShow;

    [SerializeField] private float playtime;

    public bool IsShowed
    {
        get => isShow;
        set => isShow = value;
    }

    public bool IsGameEnd
    {
        get => isGameEnd;
        set => isGameEnd = value;
    }

    public bool IsGameRestart
    {
        get => isGameRestart;
        set => isGameRestart = value;
    }

    public float Playtime
    {
        get => playtime;
        set => playtime = value;
    }

    public override string Serialize()
    {
        return JsonUtility.ToJson(this);
    }

    public override EventResponse Deserialize(string obj)
    {
        return JsonUtility.FromJson<MenuResponse>(obj);
    }
}