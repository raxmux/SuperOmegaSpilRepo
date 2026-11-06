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
public enum itemType
{
    relic,
    Equipment,
    Consumable
}
public enum itemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}
public enum equipmentType
{
    none,

    weapon,

    Head,
    Arms,
    Legs,
    Torso,
    Face,
    Ears
}

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField, TextArea(1, 15)] private string itemDesc;
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private itemRarity itemRarity;
    [SerializeField] public int value;
    [Header("")]
    public itemType itemType;
    public equipmentType equipmentType;
    [Header("")]
    public Stats stats;
    [Header("")]
    public Trigger trigger = new Trigger();

    public List<Move> moves = new List<Move>();

    public virtual void OnPickup()
    {
        //add functionality for when the item is picked up, Set in the child class

    }


}