using System;
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

    public GameObject enemyOptionButton;

    [Header("")]
    [SerializeField] MapMovement mapMovement;
    public GameObject endUI;


    Event currentEvent = null;

    public event Action EventEnded;

    public void StartEvent(EventType type)
    {
        mapMovement.moving = true;
        switch (type)
        {
            case EventType.Shop:

                Event[] shopEvents = new List<Event>(allEvents).FindAll(e => e.EventType == EventType.Shop).ToArray();

                currentEvent = shopEvents[UnityEngine.Random.Range(0, shopEvents.Length)];
                currentEvent.StartEvent();
                break;
            case EventType.Dialogue:

                Event[] dialogueEvents = new List<Event>(allEvents).FindAll(e => e.EventType == EventType.Dialogue).ToArray();

                currentEvent = dialogueEvents[UnityEngine.Random.Range(0, dialogueEvents.Length)];
                currentEvent.StartEvent();
                break;
            case EventType.Enemy:

                Event[] enemyEvents = new List<Event>(allEvents).FindAll(e => e.EventType == EventType.Enemy).ToArray();

                currentEvent = enemyEvents[UnityEngine.Random.Range(0, enemyEvents.Length)];
                currentEvent.StartEvent();
                break;
            case EventType.end:
                Event[] endEvents = new List<Event>(allEvents).FindAll(e => e.EventType == EventType.end).ToArray();
                currentEvent = endEvents[UnityEngine.Random.Range(0, endEvents.Length)];
                currentEvent.StartEvent();
                break;

        }
    }

    public void EndEvent(bool end = false)
    {
        mapMovement.ExitRoom();
        if(!end) currentEvent.EndEvent();

        currentEvent = null;
        EventEnded?.Invoke();
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


    #region end
    public void BackButton()
    {
        endUI.SetActive(false);

        EndEvent(true);
    }
    public void EndButton()
    {
        endUI.SetActive(false);
        MapMovement.instance.gameObject.transform.position = new Vector3(0, 0, 0);
        MapMovement.instance.roomSpawner.SpawnRooms();

        EndEvent(true);
    }

    #endregion
}