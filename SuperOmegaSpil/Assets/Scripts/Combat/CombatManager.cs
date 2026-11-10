using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    [SerializeField] Combatant player;
    [SerializeField] Combatant enemy;
    [SerializeField] List<Move> baseMoves = new List<Move>();

    public bool InCombat { get; private set; }
    public EnemyData CurrentEnemy { get; private set; }
    public Combatant Player => player;
    public Combatant Enemy => enemy;

    public event Action CombatStarted;      // set up the enemy sprite, move buttons etc.
    public event Action<bool> CombatEnded;  // true = player won
    public event Action<Move> EnemyUsedMove; // for showing what the enemy did

    readonly List<Move> playerMoves = new List<Move>();

    static readonly equipmentType[] MoveSlots =
        { equipmentType.weapon, equipmentType.Head, equipmentType.Legs, equipmentType.Face };

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        TriggerManager.Instance.RegisterListener(TriggerType.TimeclockPoint, OnClockPoint);
        player.Died += OnPlayerDied;
        enemy.Died += OnEnemyDied;
    }

    void OnDestroy()
    {
        if (TriggerManager.Instance != null)
            TriggerManager.Instance.UnregisterListener(TriggerType.TimeclockPoint, OnClockPoint);
        if (player != null) player.Died -= OnPlayerDied;
        if (enemy != null) enemy.Died -= OnEnemyDied;
    }

    public void StartCombat(EnemyData data)
    {
        if (InCombat || data == null) return;

        CurrentEnemy = data;
        enemy.ResetHp(data.maxHp);
        InCombat = true;
        TimeClock.Instance.SetPoints(data.attackTimes);
        CombatStarted?.Invoke();
    }

    // Base moves + one move from each of the 4 move slots. Use this to build the move buttons.
    public IReadOnlyList<Move> GetPlayerMoves()
    {
        playerMoves.Clear();
        AddMoves(baseMoves);

        if (Inventory.instance != null)
        {
            foreach (equipmentType slot in MoveSlots)
            {
                Item item = Inventory.instance.equipment.Find(e => e != null && e.equipmentType == slot);
                Move move = FirstMove(item);
                if (move != null && !playerMoves.Contains(move)) playerMoves.Add(move);
            }
        }

        return playerMoves;
    }
    public static Move FirstMove(Item item)
    {
        if (item == null || item.moves == null || item.moves.Count == 0) return null;
        return item.moves[0];
    }
    public bool UseConsumable(Item item)
    {
        if (!InCombat || item == null || player.IsDead) return false;
        if (Inventory.instance == null || !Inventory.instance.consumables.Contains(item)) return false;

        Move move = FirstMove(item);
        if (move == null) return false;

        Inventory.instance.RemoveItem(item); // used up
        move.Use(player, enemy);
        TimeClock.Instance.AdvanceTime(move.timeCost);
        return true;
    }

    // The move happens first, then the clock moves forward.
    // So if the move kills the enemy, its attack during the time cost never happens.
    public bool UseMove(Move move)
    {
        if (!InCombat || move == null || player.IsDead) return false;
        if (!GetPlayerMoves().Contains(move)) return false;

        move.Use(player, enemy);
        TimeClock.Instance.AdvanceTime(move.timeCost);
        return true;
    }

    void OnClockPoint(TriggerContext context)
    {
        if (!InCombat || enemy.IsDead || player.IsDead) return;

        Move move = CurrentEnemy.PickMove(enemy.Hp / enemy.MaxHp);
        if (move == null) return;

        move.Use(enemy, player);
        EnemyUsedMove?.Invoke(move);
    }

    void OnEnemyDied(Combatant _) => EndCombat(true);
    void OnPlayerDied(Combatant _) => EndCombat(false);

    void EndCombat(bool playerWon)
    {
        if (!InCombat) return;

        InCombat = false;
        TimeClock.Instance.ResetPoints(); // back to the overworld points, so map enemies keep moving
        CombatEnded?.Invoke(playerWon);
        CurrentEnemy = null;
    }

    void AddMoves(List<Move> moves)
    {
        if (moves == null) return;
        foreach (var m in moves)
            if (m != null && !playerMoves.Contains(m)) playerMoves.Add(m);
    }
}