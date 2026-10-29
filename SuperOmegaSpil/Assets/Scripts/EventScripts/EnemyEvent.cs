using UnityEngine;

public class EnemyEvent : Event
{
    protected override EventType EventType => EventType.Enemy;

    [Header("Fight settings")]
    //list enemies to spawn in the event
    //idk more stuff like that..

    [Header("reward settings")]
    //list of rewards to give the player after the event is completed

    [Header("EventUI")]
    [SerializeField] protected GameObject EnemyUI;

    protected override void StartEvent()
    {
        base.StartEvent();
        
        EnemyUI.SetActive(true);

    }

}
