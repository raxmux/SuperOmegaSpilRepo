using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class RunStats
{
    public int floorsReached;
    public int roomsCleared;
    public int enemiesKilled;
    public float damageDealt;
    public float damageTaken;
    public float healingDone;
    public float biggestHit;
    public int movesUsed;
    public int consumablesUsed;
    public int itemsPickedUp;
    public int moneyEarned;
    public int moneySpent;
    public int moneyAtDeath;
    public int clockLoops;
    public float timeSurvived;
    public string killedBy = "";
}

public struct ScoreLine
{
    public string label;
    public string value;
    public int points;

    public ScoreLine(string label, string value, int points)
    {
        this.label = label;
        this.value = value;
        this.points = points;
    }
}

public static class ScoreCalculator
{
    // Evil ass score calculator
    public static List<ScoreLine> GetBreakdown(RunStats s)
    {
        var lines = new List<ScoreLine>();
        Add(lines, "Floors reached", s.floorsReached, 1000);
        Add(lines, "Rooms cleared", s.roomsCleared, 100);
        Add(lines, "Enemies killed", s.enemiesKilled, 250);
        Add(lines, "Damage dealt", s.damageDealt, 1);
        Add(lines, "Biggest hit", s.biggestHit, 5);
        Add(lines, "Healing done", s.healingDone, 1);
        Add(lines, "Moves used", s.movesUsed, 5);
        Add(lines, "Consumables used", s.consumablesUsed, 20);
        Add(lines, "Items picked up", s.itemsPickedUp, 50);
        Add(lines, "Money earned", s.moneyEarned, 10);
        Add(lines, "Clock loops", s.clockLoops, 25);
        lines.Add(new ScoreLine("Time survived", FormatTime(s.timeSurvived), Mathf.FloorToInt(s.timeSurvived)));
        return lines;
    }

    public static int Calculate(RunStats s)
    {
        int total = 0;
        foreach (ScoreLine line in GetBreakdown(s)) total += line.points;
        return total;
    }

    public static string FormatTime(float seconds)
    {
        return $"{Mathf.FloorToInt(seconds / 60f)}:{Mathf.FloorToInt(seconds % 60f):00}";
    }

    static void Add(List<ScoreLine> lines, string label, float value, int pointsEach)
    {
        lines.Add(new ScoreLine(label, Mathf.RoundToInt(value).ToString(), Mathf.RoundToInt(value * pointsEach)));
    }
}

public class RunTracker : MonoBehaviour
{
    public static RunStats LastRun { get; private set; }
    public RunStats Stats { get; private set; } = new RunStats();

    [SerializeField] string deathSceneName = "DeathScreen";
    [SerializeField] float deathDelay = 1.5f;

    float enemyLastHp;
    string currentEnemyName = "";
    bool dead;

    void OnEnable()
    {
        RoomSpawner.FloorSpawned += OnFloorSpawned;
    }

    void OnDisable()
    {
        RoomSpawner.FloorSpawned -= OnFloorSpawned;
    }

    void Start()
    {
        var triggers = TriggerManager.Instance;
        triggers.RegisterListener(TriggerType.EnemyKilled, OnEnemyKilled);
        triggers.RegisterListener(TriggerType.PlayerDamaged, OnPlayerDamaged);
        triggers.RegisterListener(TriggerType.playerHealed, OnPlayerHealed);
        triggers.RegisterListener(TriggerType.TimeclockReset, OnClockReset);

        var combat = CombatManager.Instance;
        combat.CombatStarted += OnCombatStarted;
        combat.PlayerUsedMove += OnPlayerUsedMove;
        combat.PlayerUsedConsumable += OnPlayerUsedConsumable;
        combat.Enemy.HpChanged += OnEnemyHpChanged;
        combat.Player.Died += OnPlayerDied;

        Inventory.instance.MoneyChanged += OnMoneyChanged;
        Inventory.instance.ItemAdded += OnItemAdded;

        EventManager.instance.EventEnded += OnEventEnded;
    }

    void OnDestroy()
    {
        if (TriggerManager.Instance != null)
        {
            TriggerManager.Instance.UnregisterListener(TriggerType.EnemyKilled, OnEnemyKilled);
            TriggerManager.Instance.UnregisterListener(TriggerType.PlayerDamaged, OnPlayerDamaged);
            TriggerManager.Instance.UnregisterListener(TriggerType.playerHealed, OnPlayerHealed);
            TriggerManager.Instance.UnregisterListener(TriggerType.TimeclockReset, OnClockReset);
        }

        var combat = CombatManager.Instance;
        if (combat != null)
        {
            combat.CombatStarted -= OnCombatStarted;
            combat.PlayerUsedMove -= OnPlayerUsedMove;
            combat.PlayerUsedConsumable -= OnPlayerUsedConsumable;
            if (combat.Enemy != null) combat.Enemy.HpChanged -= OnEnemyHpChanged;
            if (combat.Player != null) combat.Player.Died -= OnPlayerDied;
        }

        if (Inventory.instance != null)
        {
            Inventory.instance.MoneyChanged -= OnMoneyChanged;
            Inventory.instance.ItemAdded -= OnItemAdded;
        }

        if (EventManager.instance != null)
            EventManager.instance.EventEnded -= OnEventEnded;
    }

    void OnFloorSpawned() => Stats.floorsReached++;
    void OnEventEnded() => Stats.roomsCleared++;
    void OnEnemyKilled(TriggerContext _) => Stats.enemiesKilled++;
    void OnPlayerDamaged(TriggerContext c) => Stats.damageTaken += c.Amount;
    void OnPlayerHealed(TriggerContext c) => Stats.healingDone += c.Amount;
    void OnClockReset(TriggerContext _) => Stats.clockLoops++;
    void OnPlayerUsedMove(Move _) => Stats.movesUsed++;
    void OnPlayerUsedConsumable(Item _) => Stats.consumablesUsed++;
    void OnItemAdded(Item _) => Stats.itemsPickedUp++;

    void OnMoneyChanged(int amount)
    {
        if (amount > 0) Stats.moneyEarned += amount;
        else Stats.moneySpent -= amount;
    }

    void OnCombatStarted()
    {
        var combat = CombatManager.Instance;
        enemyLastHp = combat.Enemy.Hp;
        currentEnemyName = combat.CurrentEnemy != null ? combat.CurrentEnemy.enemyName : "";
    }

    void OnEnemyHpChanged(Combatant enemy)
    {
        if (enemy.Hp < enemyLastHp)
        {
            float hit = enemyLastHp - enemy.Hp;
            Stats.damageDealt += hit;
            Stats.biggestHit = Mathf.Max(Stats.biggestHit, hit);
        }
        enemyLastHp = enemy.Hp;
    }

    void OnPlayerDied(Combatant _)
    {
        if (dead) return;
        dead = true;

        Stats.timeSurvived = Time.timeSinceLevelLoad;
        Stats.moneyAtDeath = Inventory.instance != null ? Inventory.instance.money : 0;
        Stats.killedBy = currentEnemyName;
        LastRun = Stats;

        StartCoroutine(LoadDeathScreen());
    }

    IEnumerator LoadDeathScreen()
    {
        yield return new WaitForSecondsRealtime(deathDelay);
        SceneManager.LoadScene(deathSceneName);
    }
}