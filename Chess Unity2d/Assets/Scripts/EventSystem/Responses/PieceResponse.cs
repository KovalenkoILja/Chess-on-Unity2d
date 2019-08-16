using System;
using UnityEngine;

[Serializable]
public class PieceResponse : EventResponse
{
    [SerializeField] private bool isBlackMove;

    public bool IsBlackMove
    {
        get => isBlackMove;
        set => isBlackMove = value;
    }

    public override string Serialize()
    {
        return JsonUtility.ToJson(this);
    }

    public override EventResponse Deserialize(string obj)
    {
        return JsonUtility.FromJson<PieceResponse>(obj);
    }
}