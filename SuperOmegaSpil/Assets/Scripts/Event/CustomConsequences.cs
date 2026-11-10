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
    public override void Execute()
    {
        Debug.Log("Speedy Move!");
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