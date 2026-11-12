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
        player.TakeDamage(dmg, null, true);

        Debug.Log($"Taking {dmg} damage.");
    }
}
[Serializable]
public class ChangeMaxHpAction : TriggerActions
{
    [SerializeField] private int changeAmmount;

    public override void Execute(TriggerContext context = default)
    {
        Combatant player = GameObject.FindGameObjectWithTag("CombatPlayer").GetComponent<Combatant>();
        player.maxHp += changeAmmount;

        Debug.Log($"Changing max HP by {changeAmmount}.");
    }
}

[Serializable]
public class HealAction : TriggerActions
{
    [SerializeField] private int healAmount;

    public override void Execute(TriggerContext context = default)
    {
        Combatant player = CombatManager.Instance != null ? CombatManager.Instance.Player : null;
        if (player == null) return;

        player.Heal(healAmount);
    }
}

[Serializable]
public class DamageEnemyAction : TriggerActions
{
    [SerializeField] private int dmg;

    public override void Execute(TriggerContext context = default)
    {
        CombatManager combat = CombatManager.Instance;
        if (combat == null || !combat.InCombat) return;

        combat.Enemy.TakeDamage(dmg, combat.Player.gameObject);
    }
}

[Serializable]
public class AddMoneyAction : TriggerActions
{
    [SerializeField] private int amount;

    public override void Execute(TriggerContext context = default)
    {
        if (Inventory.instance == null) return;

        Inventory.instance.AddMoney(amount);
    }
}