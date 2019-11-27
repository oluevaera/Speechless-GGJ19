using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    private static EventManager instance;
    public static EventManager Instance 
    {
        get 
        {
            if (instance == null)
            {
                var go = new GameObject("AUTO - EventManager");
                instance = go.AddComponent<EventManager>();
                Debug.LogWarning("Automatically created an EventManager, you should create one yourself!");
            }
            return instance;
        }
    }

    private Dictionary<string, UnityEvent> eventDictionary = new Dictionary<string, UnityEvent>();

    private void Awake() 
    {
        instance = this;
    }

    public static void AddListener(string eventName, UnityAction listener) 
    {
        UnityEvent thisEvent = null;
        if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            Instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void RemoveListener(string eventName, UnityAction listener) 
    {
        UnityEvent thisEvent = null;
        if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
        else
        {
            Debug.LogError($"Cannot remove listener from event {1:eventName} that doesn't exist!");
        }
    }

    public static void PostEvent(string eventName) 
    {
        UnityEvent thisEvent = null;
        if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }
}