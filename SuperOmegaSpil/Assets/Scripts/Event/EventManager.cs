using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    public List<Event> allEvents = new List<Event>();

    [Header("-- shop")]
    public GameObject shopUI;
    public List<GameObject> ShopButtons = new List<GameObject>();

    [Header("-- dialogue")]
    public GameObject dialogueUI;
    public TMP_Text dialoguePrompt;
    public List<GameObject> dialogueOptions = new List<GameObject>();
    public GameObject continueOption;

    [Header("-- enemy")]
    public GameObject enemyUI;


    [Header("")]
    [SerializeField] MapMovement mapMovement;


    Event currentEvent = null;

    public void StartEvent(EventType type)
    {
        mapMovement.moving = true;
        switch (type)
        {
            case EventType.Shop:

                Event[] shopEvents = new List<Event>(allEvents).FindAll(e => e.EventType == EventType.Shop).ToArray();

                currentEvent = shopEvents[Random.Range(0, shopEvents.Length)];
                currentEvent.StartEvent();
                break;
            case EventType.Dialogue:

                Event[] dialogueEvents = new List<Event>(allEvents).FindAll(e => e.EventType == EventType.Dialogue).ToArray();

                currentEvent = dialogueEvents[Random.Range(0, dialogueEvents.Length)];
                currentEvent.StartEvent();
                break;
            case EventType.Enemy:

                Event[] enemyEvents = new List<Event>(allEvents).FindAll(e => e.EventType == EventType.Enemy).ToArray();

                currentEvent = enemyEvents[Random.Range(0, enemyEvents.Length)];
                currentEvent.StartEvent();
                break;
        }
    }

    public void EndEvent()
    {
        mapMovement.ExitRoom();
        currentEvent.EndEvent();

        currentEvent = null;
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



    public void ShopBuyItem(int buttonIndex)
    {
        var shopEvent = currentEvent as ShopEvent;
        shopEvent.BuyItem(buttonIndex);
    }

    public void DialogueChooseOption(int buttonIndex)
    {
        var dialogueEvent = currentEvent as DialogueEvent;
        dialogueEvent.ChooseOption(buttonIndex);
    }
}
