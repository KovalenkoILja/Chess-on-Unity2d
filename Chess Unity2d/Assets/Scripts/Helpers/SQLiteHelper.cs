using System;
using System.Collections.Generic;
using System.Linq;
using SQLiteTypes;

// ReSharper disable once InconsistentNaming
public static class SQLiteHelper
{
    #region DataService

    private static readonly DataService DataService = new DataService("RecordsDB.db");

    #endregion

    #region WorkWithDB

    // ReSharper disable once MemberCanBePrivate.Global
    public static IEnumerable<GameSetups> GetGameSetups()
    {
        return DataService.GetGameSetups();
    }

    public static IEnumerable<GameResult> GetGameResults()
    {
        return DataService.GetGameResults();
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static IEnumerable<SQLiteTypes.EndGamesStates> GetEndGamesStates()
    {
        return DataService.GetEndGamesStates();
    }

    public static bool InsertResult()
    {
        var setups = GetGameSetups().ToArray();
        var states = GetEndGamesStates().ToArray();

        if (!setups.Any())
            return false;
        if (!states.Any())
            return false;

        string setupStr;
        switch (GameStatData.GameSetup)
        {
            case GameSetup.PlayerWhiteVsPlayerBlack:
                setupStr = "Игрок против Игрока";
                break;
            case GameSetup.PlayerWhiteVsAiBlack:
                setupStr = "Игрок за Белых против Компьютера";
                break;
            case GameSetup.PlayerBlackVsAiWhite:
                setupStr = "Игрок за Черных против Компьютера";
                break;
            case GameSetup.AiWhiteVsAiBlack:
                setupStr = "Компьютер против Компьютера";
                break;
            case GameSetup.None:
                setupStr = "Никакой";
                break;
            default:
                return false;
        }

        var setupId = 0;
        foreach (var setup in setups)
            if (setup.Setup == setupStr)
                setupId = setup.SetipId;

        if (setupId == 0)
            return false;

        var stateId = 0;
        foreach (var state in states)
            if (state.EndGameState == GameStatData.Result)
                stateId = state.EndGameStateId;
        if (stateId == 0)
            return false;

        var result = new GameResult
        {
            Score = GameStatData.Score,
            Playtime = GameStatData.Playtime,
            PGN = GameStatData.PGN,
            BlackTime = GameStatData.BlackTime,
            WhiteTime = GameStatData.WhiteTime,
            GameSetupId = setupId,
            EndGameStateId = stateId
        };

        return DataService.InsertResult(result) > 0;
    } // ReSharper disable StringLiteralTypo
    public static string[] GetRecords()
    {
        var strings = GetGameResults().ToArray();
        var states = GetEndGamesStates().ToArray();
        var setups = GetGameSetups().ToArray();

        if (!strings.Any())
            return null;

        var arrOfStr = new string[strings.Length];

        for (var i = 0; i < strings.Length; i++)
        {
            var stateStr = states.Where((gamesStates, i1)
                    => gamesStates.EndGameStateId == strings[i].EndGameStateId).FirstOrDefault()
                ?.EndGameState;
            var setupStr = setups.Where((gameSetups, i1) => gameSetups.SetipId == strings[i].GameSetupId)
                .FirstOrDefault()
                ?.Setup;

            var str = $"№{strings[i].ResultId} " +
                      $"{stateStr} " +
                      $"({setupStr}) " +
                      $"[{strings[i].Score}]\n" +
                      $"Время: {TimeSpan.FromSeconds(strings[i].Playtime):hh\\:mm\\:ss\\:fff}" +
                      $" (Б: {TimeSpan.FromSeconds(strings[i].WhiteTime):hh\\:mm\\:ss\\:fff}" +
                      $" Ч: {TimeSpan.FromSeconds(strings[i].BlackTime):hh\\:mm\\:ss\\:fff})\n";
            arrOfStr[i] = str;
        }

        return arrOfStr;
    }

    #endregion
}