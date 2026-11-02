using System;
using UnityEngine;
public class Combatant : MonoBehaviour
{
    [SerializeField] float maxHp = 100f;
    [SerializeField] bool isPlayer;

    public float MaxHp => maxHp;
    public float Hp { get; private set; }
    public bool IsPlayer => isPlayer;
    public bool IsDead { get; private set; }

    public event Action<Combatant> HpChanged;
    public event Action<Combatant> Died;

    void Awake()
    {
        Hp = maxHp;
    }

    public void TakeDamage(float amount, GameObject source = null)
    {
        if (IsDead || amount <= 0f) return;

        Hp = Mathf.Max(0f, Hp - amount);
        HpChanged?.Invoke(this);

        if (isPlayer)
            TriggerManager.Instance.TriggerEvent(TriggerType.PlayerDamaged,
                new TriggerContext(source, gameObject, amount));

        if (Hp <= 0f) Die(source);
    }

    public void Heal(float amount, GameObject source = null)
    {
        if (IsDead || amount <= 0f) return;

        float before = Hp;
        Hp = Mathf.Min(maxHp, Hp + amount);
        float healed = Hp - before;
        if (healed <= 0f) return;

        HpChanged?.Invoke(this);

        if (isPlayer)
            TriggerManager.Instance.TriggerEvent(TriggerType.playerHealed,
                new TriggerContext(source, gameObject, healed));
    }

    void Die(GameObject killer)
    {
        IsDead = true;
        Died?.Invoke(this);

        if (!isPlayer)
            TriggerManager.Instance.TriggerEvent(TriggerType.EnemyKilled,
                new TriggerContext(killer, gameObject, 0f));
    }
}