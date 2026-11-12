using System.Collections.Generic;
using UnityEngine;

public class HitFeedback : MonoBehaviour
{
    [Header("Screen shake (player hit)")]
    [SerializeField] List<Transform> cameras = new List<Transform>();
    [SerializeField] float screenShakeStrength = 0.3f;

    [Header("Enemy shake (enemy hit)")]
    [SerializeField] Transform enemyVisual;
    [SerializeField] float enemyShakeStrength = 0.2f;

    [Header("Shake feel")]
    [SerializeField] float duration = 0.25f;
    [SerializeField] float referenceDamage = 20f;
    [SerializeField] float minScale = 0.3f;
    [SerializeField] float maxScale = 2f;

    [Header("Hit particle (enemy hit)")]
    [SerializeField] GameObject hitParticle;
    [SerializeField] Vector3 particleOffset;
    [SerializeField] float particleLifetime = 0.5f;

    class Shake
    {
        public float time;
        public float strength;
        public Vector3 applied;
    }

    readonly Shake screen = new Shake();
    readonly Shake enemy = new Shake();
    float playerLastHp, enemyLastHp;

    void Start()
    {
        var combat = CombatManager.Instance;
        playerLastHp = combat.Player.Hp;
        enemyLastHp = combat.Enemy.Hp;
        combat.Player.HpChanged += OnPlayerHpChanged;
        combat.Enemy.HpChanged += OnEnemyHpChanged;
    }

    void OnDestroy()
    {
        var combat = CombatManager.Instance;
        if (combat == null) return;
        if (combat.Player != null) combat.Player.HpChanged -= OnPlayerHpChanged;
        if (combat.Enemy != null) combat.Enemy.HpChanged -= OnEnemyHpChanged;
    }

    void OnPlayerHpChanged(Combatant player)
    {
        float damage = playerLastHp - player.Hp;
        playerLastHp = player.Hp;
        if (damage > 0f) BeginShake(screen, screenShakeStrength, damage);
    }

    void OnEnemyHpChanged(Combatant e)
    {
        float damage = enemyLastHp - e.Hp;
        enemyLastHp = e.Hp;
        if (damage <= 0f) return;

        BeginShake(enemy, enemyShakeStrength, damage);

        if (hitParticle != null && enemyVisual != null)
        {
            GameObject particle = Instantiate(hitParticle, enemyVisual.position + particleOffset, Quaternion.identity);
            Destroy(particle, particleLifetime);
        }
    }

    void BeginShake(Shake shake, float baseStrength, float damage)
    {
        float scale = Mathf.Clamp(damage / referenceDamage, minScale, maxScale);
        shake.strength = Mathf.Max(shake.strength * (1f - shake.time / duration), baseStrength * scale);
        shake.time = 0f;
    }

    // Take last frame's shake off first, so other scripts see the real positions.
    void Update()
    {
        foreach (Transform cam in cameras)
            if (cam != null) cam.position -= screen.applied;
        screen.applied = Vector3.zero;

        if (enemyVisual != null) enemyVisual.position -= enemy.applied;
        enemy.applied = Vector3.zero;
    }

    void LateUpdate()
    {
        screen.applied = NextOffset(screen, false);
        foreach (Transform cam in cameras)
            if (cam != null) cam.position += screen.applied;

        enemy.applied = NextOffset(enemy, true);
        if (enemyVisual != null) enemyVisual.position += enemy.applied;
    }

    Vector3 NextOffset(Shake shake, bool sideToSide)
    {
        if (shake.time >= duration) return Vector3.zero;

        shake.time += Time.deltaTime;
        float fade = 1f - Mathf.Clamp01(shake.time / duration);
        float amount = shake.strength * fade * fade;

        Vector2 dir = sideToSide ? new Vector2(Random.Range(-1f, 1f), 0f) : Random.insideUnitCircle;
        return (Vector3)(dir * amount);
    }
}