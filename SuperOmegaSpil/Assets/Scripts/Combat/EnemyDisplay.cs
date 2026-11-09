using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyDisplay : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    void Start()
    {
        CombatManager.Instance.CombatStarted += OnCombatStarted;
        CombatManager.Instance.CombatEnded += OnCombatEnded;
    }

    void OnDestroy()
    {
        if (CombatManager.Instance != null)
        {
            CombatManager.Instance.CombatStarted -= OnCombatStarted;
            CombatManager.Instance.CombatEnded -= OnCombatEnded;
        }
    }

    void OnCombatStarted()
    {
        spriteRenderer.sprite = CombatManager.Instance.CurrentEnemy.sprite;
        spriteRenderer.enabled = true;
    }

    void OnCombatEnded(bool playerWon)
    {
        spriteRenderer.enabled = false;
    }
}