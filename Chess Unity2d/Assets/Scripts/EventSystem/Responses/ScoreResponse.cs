using System;
using UnityEngine;

[Serializable]
public class ScoreResponse : EventResponse
{
    [SerializeField] private bool isShow;

    [SerializeField] private int score;

    public bool IsShow
    {
        get => isShow;
        set => isShow = value;
    }

    public int Score
    {
        get => score;
        set => score = value;
    }

    public override string Serialize()
    {
        return JsonUtility.ToJson(this);
    }

    public override EventResponse Deserialize(string obj)
    {
        return JsonUtility.FromJson<ScoreResponse>(obj);
    }
}