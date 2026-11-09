using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    public List<Event> allEvents = new List<Event>();

    [Header("")]
    public GameObject shopUI;
    public GameObject dialogueUI;

    public void StartEvent(EventType type)
    {
        



    }

    public void EndEvent()
    {




    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("-- FUCK GUYS NOOO theres more than one instance of EventManager!!!");
            return;
        }
        instance = this;
    }
}
