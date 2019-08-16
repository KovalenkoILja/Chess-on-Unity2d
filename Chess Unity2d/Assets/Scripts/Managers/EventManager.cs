using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    #region UnityEvents

    private void Awake()
    {
        var objs = GameObject.FindGameObjectsWithTag("EventManager");

        if (objs.Length > 1) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    #endregion

    #region Methods

    private void Init()
    {
        if (eventDictionary == null) eventDictionary = new Dictionary<string, Action<EventParam>>();
    }

    public static void StartListening(string eventName, Action<EventParam> listener)
    {
        if (Instance.eventDictionary.TryGetValue(eventName, out var thisEvent))
        {
            //Add more event to the existing one
            thisEvent += listener;

            //Update the Dictionary
            Instance.eventDictionary[eventName] = thisEvent;
        }
        else
        {
            //Add event to the Dictionary for the first time
            thisEvent += listener;
            Instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, Action<EventParam> listener)
    {
        if (_eventManager == null)
            return;

        if (!Instance.eventDictionary.TryGetValue(eventName, out var thisEvent))
            return;

        if (listener != null)
            if (thisEvent != null)
                // ReSharper disable once DelegateSubtraction
                thisEvent -= listener;

        Instance.eventDictionary[eventName] = thisEvent;
    }

    public static void TriggerEvent(string eventName, EventParam eventParam)
    {
        if (Instance.eventDictionary.TryGetValue(eventName, out var thisEvent))
            try
            {
                thisEvent?.Invoke(eventParam);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                //Instance.eventDictionary[eventName](eventParam);
            }
    }

    #endregion

    #region Variables

    private Dictionary<string, Action<EventParam>> eventDictionary;

    private static EventManager _eventManager;

    private static EventManager Instance
    {
        get
        {
            // ReSharper disable once InvertIf
            if (!_eventManager)
            {
                _eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!_eventManager)
                {
                    Debug.Log(
                        "There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    if (_eventManager != null)
                        _eventManager.Init();
                }
            }

            return _eventManager;
        }
    }

    #endregion
}