using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Event", menuName = "Event/Enemy Event")]

public class EnemyEvent : Event
{
    public override EventType EventType => EventType.Enemy;

    [Header("Fight settings")]
    [SerializeField] EnemyData enemy; // the enemy you fight in this event

    [Header("reward settings")]
    //list of rewards to give the player after the event is completed
    int stopComplainingWanker;

    public override void StartEvent()
    {
        base.StartEvent();

        EventManager.instance.enemyUI.SetActive(true);
        CombatManager.Instance.StartCombat(enemy);
    }
    public override void EndEvent()
    {
        base.EndEvent();

        EventManager.instance.enemyUI.SetActive(false);
    }

}