using System;
using UnityEngine;

[Serializable]
public class StartDialogResults : EventResponse
{
    [SerializeField] protected GameSetup setup;

    public GameSetup Setup
    {
        get => setup;
        set => setup = value;
    }

    public override string Serialize()
    {
        return JsonUtility.ToJson(this);
    }

    public override EventResponse Deserialize(string obj)
    {
        return JsonUtility.FromJson<StartDialogResults>(obj);
    }
}