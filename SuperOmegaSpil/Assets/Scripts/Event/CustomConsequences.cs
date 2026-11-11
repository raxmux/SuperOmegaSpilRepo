using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consequence", menuName = "Consequence/New Consequence")]
public class CustomConsequences : ScriptableObject
{
    [SerializeReference, SubclassSelector]
    public List<ConsequenceEffect> effects = new List<ConsequenceEffect>();

    public void Execute()
    {
        foreach (ConsequenceEffect effect in effects)
        {
            effect?.Execute();
        }
    }
}

[Serializable]
public abstract class ConsequenceEffect
{
    public abstract void Execute();
}

[Serializable]
public class SpeedyMoveConsequence : ConsequenceEffect
{
    static readonly equipmentType[] MoveSlots =
    { equipmentType.weapon, equipmentType.Head, equipmentType.Legs, equipmentType.Face };
    public override void Execute()
    {
        if (Inventory.instance != null)
        {
            foreach (equipmentType slot in MoveSlots)
            {
                Item item = Inventory.instance.equipment.Find(e => e != null && e.equipmentType == slot);
                Move move = FirstMove(item);
                if (move != null)
                {
                    move.timeCost /= 1.5f;
                    Debug.Log($"SpeedyMoveConsequence applied to move: {move.moveName}, new timeCost: {move.timeCost}");
                    break;
                }
            }
        }
    }
    public static Move FirstMove(Item item)
    {
        if (item == null || item.moves == null || item.moves.Count == 0) return null;
        return item.moves[0];
    }
}
[Serializable]
public class ExtraMaxHealthConsequence : ConsequenceEffect
{
    public int healthToAdd;
    public override void Execute()
    {
        Combatant player = GameObject.FindGameObjectWithTag("CombatPlayer").GetComponent<Combatant>();
        player.maxHp += healthToAdd;
    }
}
[Serializable]
public class GoldenShrineCurseConsequence : ConsequenceEffect
{
    public Item cursedItem;
    public override void Execute()
    {
        Inventory.instance.GiveItem(cursedItem);
    }
}