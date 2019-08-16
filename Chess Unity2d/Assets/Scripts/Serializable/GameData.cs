using System;
using System.Collections.Generic;
using ChessEngine;
using UnityEngine;

[Serializable]
public class GameData
{
    [SerializeField] public List<PieceData> cells;

    [SerializeField] public string fen;

    [SerializeField] public GameSetup gameSetup;

    [SerializeField] public List<Log> logs;

    [SerializeField] public float timeFromStartGame;

    [SerializeField] public ChessPieceColor whoIsMove;

    public GameData()
    {
        cells = new List<PieceData>();
        logs = new List<Log>();
    }

    public string Serialize()
    {
        return JsonUtility.ToJson(this, true);
    }

    public static GameData Deserialize(string obj)
    {
        return JsonUtility.FromJson<GameData>(obj);
    }
}