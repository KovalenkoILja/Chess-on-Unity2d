using System;
using UnityEngine;

[Serializable]
public class BoardResponse : EventResponse
{
    [SerializeField] protected BoardResponseType boardResponseType;

    [SerializeField] protected bool isBlackTurn;

    [SerializeField] protected string saveFileName;

    public string SaveFileName
    {
        get => saveFileName;
        set => saveFileName = value;
    }

    public bool IsBlackTurn
    {
        get => isBlackTurn;
        set => isBlackTurn = value;
    }

    public BoardResponseType BoardResponseType
    {
        get => boardResponseType;
        set => boardResponseType = value;
    }

    public override string Serialize()
    {
        return JsonUtility.ToJson(this);
    }

    public override EventResponse Deserialize(string obj)
    {
        return JsonUtility.FromJson<BoardResponse>(obj);
    }
}