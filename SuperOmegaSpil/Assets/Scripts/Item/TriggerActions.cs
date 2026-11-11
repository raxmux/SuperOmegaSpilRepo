using System;
using UnityEngine;
[Serializable]
public abstract class TriggerActions
{
    public abstract void Execute(TriggerContext context = default);
}

[Serializable]
public class DebugLogAction : TriggerActions
{
    [SerializeField] private string message;

    public override void Execute(TriggerContext context = default)
    {
        Debug.Log(message);
    }
}
[Serializable]
public class TakeDmgAction : TriggerActions
{
    [SerializeField] private int dmg;

    public override void Execute(TriggerContext context = default)
    {
        Combatant player = GameObject.FindGameObjectWithTag("CombatPlayer").GetComponent<Combatant>();
        player.TakeDamage(dmg);

        Debug.Log($"Taking {dmg} damage.");
    }
}
