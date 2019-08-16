using System;
using ChessEngine;
using UnityEngine;

[Serializable]
public class SettingsResponse : EventResponse
{
    [SerializeField] protected Engine.Difficulty difficulty;

    public Engine.Difficulty Difficulty
    {
        get => difficulty;
        set => difficulty = value;
    }

    public override string Serialize()
    {
        return JsonUtility.ToJson(this);
    }

    public override EventResponse Deserialize(string obj)
    {
        return JsonUtility.FromJson<SettingsResponse>(obj);
    }
}