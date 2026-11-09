using UnityEngine;

[CreateAssetMenu(fileName = "New Move", menuName = "Combat/Move")]
public class Move : ScriptableObject
{
    public string moveName;
    [TextArea] public string description;
    public Sprite icon;
    [Min(0f)] public float timeCost = 1f;
    [Min(0f)] public float damage;
    [Min(0f)] public float heal;

    public void Use(Combatant user, Combatant opponent)
    {
        if (user == null || user.IsDead) return;

        if (damage > 0f && opponent != null)
            opponent.TakeDamage(damage, user.gameObject);

        if (heal > 0f)
            user.Heal(heal, user.gameObject);
    }
}