using UnityEngine;
using static EventsNames;

public static class MenuEvents
{
    public static void StartGame(bool isNewGame)
    {
        Debug.Log("StartGame Event");
        EventManager.TriggerEvent(MoveColorWindowEvent,
            new EventParam
            {
                Response = new MenuResponse
                {
                    IsShowed = true
                },
                TypeOfEvent = EventResponseType.GameManagerEvent
            });
        EventManager.TriggerEvent(TimeWindowEvent,
            new EventParam
            {
                Response = new MenuResponse
                {
                    IsShowed = true
                },
                TypeOfEvent = EventResponseType.GameManagerEvent
            });
        EventManager.TriggerEvent(LogEvent,
            new EventParam
            {
                Response = new MenuResponse
                {
                    IsShowed = true
                },
                TypeOfEvent = EventResponseType.GameManagerEvent
            });
        EventManager.TriggerEvent(ScoreWindowEvent,
            new EventParam
            {
                Response = new MenuResponse
                {
                    IsShowed = true
                },
                TypeOfEvent = EventResponseType.GameManagerEvent
            });
        EventManager.TriggerEvent(LogEvent,
            new EventParam
            {
                Response = new LogResponse
                {
                    Message = "Старт",
                    IsNewGame = isNewGame
                },
                TypeOfEvent = EventResponseType.GameManagerEvent
            });
    }

    public static void PauseGame(bool isPaused)
    {
        Debug.Log("PauseGame Event");
        EventManager.TriggerEvent(PauseMenuEvent,
            new EventParam
            {
                Response = new MenuResponse
                {
                    IsShowed = isPaused
                },
                TypeOfEvent = EventResponseType.GameManagerEvent
            });

        EventManager.TriggerEvent(MoveColorWindowEvent,
            new EventParam
            {
                Response = new MenuResponse
                {
                    IsShowed = !isPaused
                },
                TypeOfEvent = EventResponseType.GameManagerEvent
            });
        EventManager.TriggerEvent(TimeWindowEvent,
            new EventParam
            {
                Response = new MenuResponse
                {
                    IsShowed = !isPaused
                },
                TypeOfEvent = EventResponseType.GameManagerEvent
            });
        EventManager.TriggerEvent(AiResponseEvent,
            new EventParam
            {
                Response = new MenuResponse
                {
                    IsShowed = isPaused
                },
                TypeOfEvent = EventResponseType.GameManagerEvent
            });
        EventManager.TriggerEvent(LogEvent,
            new EventParam
            {
                Response = new MenuResponse
                {
                    IsShowed = !isPaused
                },
                TypeOfEvent = EventResponseType.GameManagerEvent
            });
        EventManager.TriggerEvent(ScoreWindowEvent,
            new EventParam
            {
                Response = new MenuResponse
                {
                    IsShowed = !isPaused
                },
                TypeOfEvent = EventResponseType.GameManagerEvent
            });
    }

    public static void RestartGame()
    {
        Debug.Log("RestartGame Event");
        EventManager.TriggerEvent(BoardsEvent,
            new EventParam
            {
                Response = new BoardResponse
                {
                    BoardResponseType = BoardResponseType.RestartGame
                },
                TypeOfEvent = EventResponseType.BoardsEvents
            });

        EventManager.TriggerEvent(TimeWindowEvent,
            new EventParam
            {
                Response = new MenuResponse
                {
                    IsShowed = true,
                    IsGameEnd = false,
                    IsGameRestart = true
                },
                TypeOfEvent = EventResponseType.GameManagerEvent
            });
        EventManager.TriggerEvent(MoveColorWindowEvent,
            new EventParam
            {
                Response = new MenuResponse
                {
                    IsShowed = true,
                    IsGameEnd = false,
                    IsGameRestart = true
                },
                TypeOfEvent = EventResponseType.GameManagerEvent
            });
        EventManager.TriggerEvent(LogEvent,
            new EventParam
            {
                Response = new MenuResponse
                {
                    IsShowed = true,
                    IsGameRestart = true
                },
                TypeOfEvent = EventResponseType.GameManagerEvent
            });
        EventManager.TriggerEvent(LogEvent,
            new EventParam
            {
                Response = new LogResponse
                {
                    Message = "Рестарт"
                },
                TypeOfEvent = EventResponseType.GameManagerEvent
            });
    }

    public static void SaveGame(string filename)
    {
        EventManager.TriggerEvent("BoardsEvent",
            new EventParam
            {
                Response = new BoardResponse
                {
                    SaveFileName = filename,
                    BoardResponseType = BoardResponseType.SaveGame
                },
                TypeOfEvent = EventResponseType.BoardsEvents
            });
        EventManager.TriggerEvent(LogEvent,
            new EventParam
            {
                Response = new LogResponse
                {
                    Message = "Сохранение"
                },
                TypeOfEvent = EventResponseType.GameManagerEvent
            });
    }

    public static void EndGame()
    {
        Debug.Log("EndGame Event");
        EventManager.TriggerEvent(MoveColorWindowEvent,
            new EventParam
            {
                Response = new MenuResponse
                {
                    IsShowed = false,
                    IsGameEnd = true
                },
                TypeOfEvent = EventResponseType.GameManagerEvent
            });
        EventManager.TriggerEvent(TimeWindowEvent,
            new EventParam
            {
                Response = new MenuResponse
                {
                    IsShowed = false,
                    IsGameEnd = true
                },
                TypeOfEvent = EventResponseType.GameManagerEvent
            });

        EventManager.TriggerEvent(EndGameWindowEvent,
            new EventParam
            {
                Response = new MenuResponse
                {
                    IsShowed = true,
                    IsGameEnd = true
                },
                TypeOfEvent = EventResponseType.GameManagerEvent
            });
    }
}