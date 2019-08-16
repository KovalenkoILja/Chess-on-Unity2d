using System;
using UnityEngine;

[Serializable]
public class PieceData
{
    [SerializeField] protected Color32 color;

    [SerializeField] protected string type;

    [SerializeField] protected int x;

    [SerializeField] protected int y;

    public string Type
    {
        get => type;
        set => type = value;
    }

    public Color32 Color
    {
        get => color;
        set => color = value;
    }

    public int X
    {
        get => x;
        set => x = value;
    }

    public int Y
    {
        get => y;
        set => y = value;
    }


    public static PieceData Deserialize(string obj)
    {
        return JsonUtility.FromJson<PieceData>(obj);
    }
}