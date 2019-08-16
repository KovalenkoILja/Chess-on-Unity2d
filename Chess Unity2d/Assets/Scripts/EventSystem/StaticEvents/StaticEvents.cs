using ChessEngine;
using UnityEngine;
using static EventsNames;

public static class StaticEvents
{
    public static void ReportOfTurnChange(bool isBlackTurn, int score)
    {
        Debug.Log("ReportOfTurnChange Event");
        EventManager.TriggerEvent(MoveColorWindowEvent,
            new EventParam
            {
                Response = new PieceResponse
                {
                    IsBlackMove = isBlackTurn
                },
                TypeOfEvent = EventResponseType.PieceManagerEvent
            });

        EventManager.TriggerEvent(ScoreWindowEvent,
            new EventParam
            {
                Response = new ScoreResponse
                {
                    Score = score
                },
                TypeOfEvent = EventResponseType.PieceManagerEvent
            });
    }

    public static void SettingsChangeEvent(Engine.Difficulty difficulty)
    {
        Debug.Log("SettingsChange Event");
        EventManager.TriggerEvent(SettingsEvent, new EventParam
        {
            TypeOfEvent = EventResponseType.GameManagerEvent,
            Response = new SettingsResponse
            {
                Difficulty = difficulty
            }
        });
    }

    public static void DialogEndEvent(GameSetup setup)
    {
        Debug.Log("DialogEnd Event");
        EventManager.TriggerEvent(EventsNames.DialogEndEvent, new EventParam
        {
            TypeOfEvent = EventResponseType.StartDialogResult,
            Response = new StartDialogResults
            {
                Setup = setup
            }
        });

        EventManager.TriggerEvent(LogEvent,
            new EventParam
            {
                Response = new LogResponse
                {
                    LogType = LogType.Warning,
                    Message = Extensions.SetupToString(setup)
                },
                TypeOfEvent = EventResponseType.GameManagerEvent
            });
    }

    public static void LogMsgEvent(string msg, LogType type)
    {
        EventManager.TriggerEvent(LogEvent,
            new EventParam
            {
                Response = new LogResponse
                {
                    LogType = type,
                    Message = msg
                },
                TypeOfEvent = EventResponseType.GameManagerEvent
            });
    }
}