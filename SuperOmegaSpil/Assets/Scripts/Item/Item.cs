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
    public float MaxHealth;
    public float Armor;
    public Stats(bool hasStats, float health, float armor)
    {
        this.hasStats = hasStats;
        MaxHealth = health;
        Armor = armor;
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

    weapon, //Equip

    Head, //Equip
    Arms,
    Legs, //Equip
    Torso, 
    Face, //Equip
    Ears
}

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    [SerializeField] public string itemName;
    [SerializeField, TextArea(1, 15)] public string itemDesc;
    [SerializeField] public Sprite itemSprite;
    [SerializeField] public itemRarity itemRarity;
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