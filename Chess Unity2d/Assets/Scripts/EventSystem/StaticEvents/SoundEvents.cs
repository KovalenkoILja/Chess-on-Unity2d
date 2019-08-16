using UnityEngine;
using static EventsNames;

public static class SoundEvents
{
    public static void PlayMoveSound()
    {
        Debug.Log("PlayMoveSound Event");

        EventManager.TriggerEvent(AudioManagerEvent, new EventParam
        {
            TypeOfEvent = EventResponseType.AudioManagerEvent,
            Response = new AudioResponse
            {
                Capture = true
            }
        });
    }

    public static void PlayCaptureSound()
    {
        Debug.Log("PlayCaptureSound Event");

        EventManager.TriggerEvent(AudioManagerEvent, new EventParam
        {
            TypeOfEvent = EventResponseType.AudioManagerEvent,
            Response = new AudioResponse
            {
                Move = true
            }
        });
    }

    public static void PlayVictorySound()
    {
        Debug.Log("PlayVictorySound Event");

        EventManager.TriggerEvent(AudioManagerEvent, new EventParam
        {
            TypeOfEvent = EventResponseType.AudioManagerEvent,
            Response = new AudioResponse
            {
                Victory = true
            }
        });
    }

    public static void PlayPromotionSound()
    {
        Debug.Log("PlayPromotionSound Event");

        EventManager.TriggerEvent(AudioManagerEvent, new EventParam
        {
            TypeOfEvent = EventResponseType.AudioManagerEvent,
            Response = new AudioResponse
            {
                Promotion = true
            }
        });
    }

    public static void PlayCheckSound()
    {
        Debug.Log("PlayCheckSound Event");

        EventManager.TriggerEvent(AudioManagerEvent, new EventParam
        {
            TypeOfEvent = EventResponseType.AudioManagerEvent,
            Response = new AudioResponse
            {
                Check = true
            }
        });
    }

    public static void PlayRejectSound()
    {
        Debug.Log("PlayRejectSound Event");

        EventManager.TriggerEvent(AudioManagerEvent, new EventParam
        {
            TypeOfEvent = EventResponseType.AudioManagerEvent,
            Response = new AudioResponse
            {
                Reject = true
            }
        });
    }
}