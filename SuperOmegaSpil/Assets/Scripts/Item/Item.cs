using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


[System.Serializable]
public class Trigger
{
    public TriggerType triggerType;

    [SerializeReference]
    public List<TriggerActions> triggerAction = new List<TriggerActions>();
}

[System.Serializable]
public struct Stats
{
    public bool hasStats;
    [Header("Stats")]
    public float Health;
    public float Speed;
    public Stats(bool hasStats, float health, float speed)
    {
        this.hasStats = hasStats;
        Health = health;
        Speed = speed;
    }
}

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemSprite;
    [Header("")]
    [SerializeField] private Stats stats;
    [Header("")]
    [SerializeField] private Trigger trigger = new Trigger();

    public virtual void Pickup()
    {
        //add the trigger to the trigger manager so that it can be called when the event happens

        if (trigger.triggerType != TriggerType.none && trigger.triggerAction != null && trigger.triggerAction.Count > 0)
        {
            TriggerManager.Instance.RegisterListener(trigger.triggerType, 
                context =>
                {
                    foreach (TriggerActions action in trigger.triggerAction)
                    {
                        if (action != null) action.Execute(context);
                    }
                });
        }
        
        if(stats.hasStats)
        {
            //change the stats of the player when the item is picked up
        }

        OnPickup();
    }
    protected virtual void OnPickup()
    {
        //add functionality for when the item is picked up, Set in the child class

    }


}
