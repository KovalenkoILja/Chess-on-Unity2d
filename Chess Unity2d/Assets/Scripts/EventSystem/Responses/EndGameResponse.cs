using System;
using UnityEngine;

[Serializable]
public class EndGameResponse : EventResponse
{
    [SerializeField] private EndGamesStates endGamesStates;

    [SerializeField] private string result;

    [SerializeField] private int score;

    public string Result
    {
        get => result;
        set => result = value;
    }

    public int Score
    {
        get => score;
        set => score = value;
    }

    public EndGamesStates EndGamesStates
    {
        get => endGamesStates;
        set => endGamesStates = value;
    }

    public override string Serialize()
    {
        return JsonUtility.ToJson(this);
    }

    public override EventResponse Deserialize(string obj)
    {
        return JsonUtility.FromJson<EndGameResponse>(obj);
    }
}