using UnityEngine;
using static EventsNames;

public static class EndGameEvents
{
    public static void EndGameEvent(EndGamesStates state, string result, int score)
    {
        Debug.Log("KingCheckmate Event");

        EventManager.TriggerEvent(BoardsEvent, new EventParam
        {
            TypeOfEvent = EventResponseType.BoardsEvents,
            Response = new EndGameResponse
            {
                EndGamesStates = state,
                Result = result,
                Score = score
            }
        });
    }
}