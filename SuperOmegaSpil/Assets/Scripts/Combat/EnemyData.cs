using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyMove
{
    public Move move;

    [Min(0f)] public float weight = 1f; //Higher chance for moves with higher weight

    [Min(0f)] public float lowHpWeight = 1f; //Higher chance lower HP
}

// One enemy type. Create > Combat > Enemy.
[CreateAssetMenu(fileName = "New Enemy", menuName = "Combat/Enemy")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public Sprite sprite;
    [Min(1f)] public float maxHp = 50f;

    public List<float> attackTimes = new List<float> { 12f }; //Times the enemy attacks

    [Range(0f, 1f)] public float lowHpThreshold = 0.3f; //Low hp threshold for picking moves with lowHpWeight instead of weight

    public List<EnemyMove> moves = new List<EnemyMove>();

    public Move PickMove(float hpFraction)
    {
        bool low = hpFraction <= lowHpThreshold;

        float total = 0f;
        foreach (var m in moves)
            if (m.move != null) total += low ? m.lowHpWeight : m.weight;

        if (total <= 0f) return null;

        // Roll a number, then walk the list until the roll lands inside a move's share.
        float roll = UnityEngine.Random.Range(0f, total);
        Move last = null;
        foreach (var m in moves)
        {
            if (m.move == null) continue;
            float w = low ? m.lowHpWeight : m.weight;
            if (w <= 0f) continue;

            if (roll < w) return m.move;
            roll -= w;
            last = m.move;
        }
        return last; // only reached if the roll was exactly the total
    }
}