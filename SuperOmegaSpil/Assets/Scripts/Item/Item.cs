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

    // Flavor text followed by the stats, built from the item's own numbers.
    public string GetFullDescription()
    {
        string statsText = GetStatsText();
        if (string.IsNullOrEmpty(statsText)) return itemDesc;
        if (string.IsNullOrEmpty(itemDesc)) return statsText;
        return itemDesc + "\n" + statsText;
    }

    public string GetStatsText()
    {
        var lines = new List<string>();

        Move move = moves != null && moves.Count > 0 ? moves[0] : null;
        if (move != null)
        {
            var parts = new List<string>();
            parts.Add(move.timeCost > 0f ? $"{move.timeCost:0.#}s" : "free");
            if (move.damage > 0f) parts.Add($"{move.damage:0.#} dmg");
            if (move.heal > 0f) parts.Add($"{move.heal:0.#} heal");

            string label = itemType == itemType.Consumable ? "Use" : "Move";
            lines.Add($"{label}: {move.moveName} ({string.Join(" · ", parts)})");
        }

        if (stats.hasStats)
        {
            var parts = new List<string>();
            if (stats.MaxHealth != 0f) parts.Add($"{stats.MaxHealth:+0.#;-0.#} Max Health");
            if (stats.Armor != 0f) parts.Add($"{stats.Armor:+0.#;-0.#} Armor");
            if (parts.Count > 0) lines.Add(string.Join(" · ", parts));
        }

        return string.Join("\n", lines);
    }

    public virtual void OnPickup()
    {
        //add functionality for when the item is picked up, Set in the child class

    }


}