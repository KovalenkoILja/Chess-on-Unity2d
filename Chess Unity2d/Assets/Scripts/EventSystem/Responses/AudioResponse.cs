using System;
using UnityEngine;

[Serializable]
public class AudioResponse : EventResponse
{
    [SerializeField] private bool capture;

    [SerializeField] private bool check;

    [SerializeField] private bool move;

    [SerializeField] private bool promotion;

    [SerializeField] private bool reject;

    [SerializeField] private bool victory;

    public bool Move
    {
        get => move;
        set => move = value;
    }

    public bool Capture
    {
        get => capture;
        set => capture = value;
    }

    public bool Victory
    {
        get => victory;
        set => victory = value;
    }

    public bool Promotion
    {
        get => promotion;
        set => promotion = value;
    }

    public bool Check
    {
        get => check;
        set => check = value;
    }

    public bool Reject
    {
        get => reject;
        set => reject = value;
    }

    public override string Serialize()
    {
        return JsonUtility.ToJson(this);
    }

    public override EventResponse Deserialize(string obj)
    {
        return JsonUtility.FromJson<AudioResponse>(obj);
    }
}