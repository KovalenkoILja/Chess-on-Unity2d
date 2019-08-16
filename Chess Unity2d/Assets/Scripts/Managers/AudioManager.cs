using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private void PlaySoundFunc(EventParam eventParam)
    {
        if (eventParam.TypeOfEvent != EventResponseType.AudioManagerEvent)
            return;

        if (!(eventParam.Response is AudioResponse response))
            return;

        if (response.Move)
        {
            AudioSource.clip = moveSound;
            AudioSource.PlayOneShot(moveSound);
        }
        else if (response.Capture)
        {
            AudioSource.clip = captureSound;
            AudioSource.PlayOneShot(captureSound);
        }
        else if (response.Victory)
        {
            AudioSource.clip = victorySound;
            AudioSource.PlayOneShot(victorySound);
        }
        else if (response.Promotion)
        {
            AudioSource.clip = promotionSound;
            AudioSource.PlayOneShot(promotionSound);
        }
        else if (response.Check)
        {
            AudioSource.clip = checkSound;
            AudioSource.PlayOneShot(checkSound);
        }
        else if (response.Reject)
        {
            AudioSource.clip = rejectSound;
            AudioSource.PlayOneShot(rejectSound);
        }
    }

    #region Variables

    public AudioClip moveSound;
    public AudioClip captureSound;
    public AudioClip victorySound;
    public AudioClip promotionSound;
    public AudioClip checkSound;
    public AudioClip rejectSound;

    private Action<EventParam> PlaySoundAction;

    private AudioSource AudioSource => GetComponent<AudioSource>();

    #endregion

    #region UnityEvents

    private void Awake()
    {
        PlaySoundAction = PlaySoundFunc;
        var objs = GameObject.FindGameObjectsWithTag("FPS Display");

        if (objs.Length > 1) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        gameObject.AddComponent<AudioSource>();
        AudioSource.playOnAwake = false;
    }

    private void OnEnable()
    {
        EventManager.StartListening(EventsNames.AudioManagerEvent, PlaySoundAction);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventsNames.AudioManagerEvent, PlaySoundAction);
    }

    #endregion
}