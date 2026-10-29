using UnityEngine;

public class DialogueEvent : Event
{
    protected override EventType EventType => EventType.Dialogue;

    [Header("Dialogue settings")]
    //list of dialogue options to display
    //idk more stuff like that..

    [Header("reward settings")]
    //list of rewards to give the player after the event is completed

    [Header("EventUI")]
    [SerializeField] protected GameObject DialogueUI;

    protected override void StartEvent()
    {
        base.StartEvent();

        DialogueUI.SetActive(true);

    }


}
