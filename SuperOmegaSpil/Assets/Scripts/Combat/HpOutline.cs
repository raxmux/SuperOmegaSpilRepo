using UnityEngine;

// 0 = full HP, 1 = dead. Put it on the object with the outline's SpriteRenderer,
// and drag in the Combatant it should show (the enemy or the player).
[RequireComponent(typeof(Renderer))]
public class HpOutline : MonoBehaviour
{
    [SerializeField] Combatant combatant;

    static readonly int DamageId = Shader.PropertyToID("_damage");
    Renderer rend;
    MaterialPropertyBlock block;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        block = new MaterialPropertyBlock();
    }

    void OnEnable()
    {
        if (combatant != null) combatant.HpChanged += Refresh;
    }

    void OnDisable()
    {
        if (combatant != null) combatant.HpChanged -= Refresh;
    }

    void Start()
    {
        if (combatant != null) Refresh(combatant);
    }

    void Refresh(Combatant c)
    {
        float damage = c.MaxHp > 0f ? 1f - c.Hp / c.MaxHp : 1f;
        rend.GetPropertyBlock(block);
        block.SetFloat(DamageId, damage);
        rend.SetPropertyBlock(block);
    }
}