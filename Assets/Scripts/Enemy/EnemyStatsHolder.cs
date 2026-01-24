using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "EnemyStatsHolder", menuName = "Enemy/EnemyStatsHolder")]
public class EnemyStatsHolder : ScriptableObject
{
    public StatCollection stats = new StatCollection();
    public List<EnemyAttributes> enemyAttributesList = new List<EnemyAttributes>();
    public StatType weakness = null;
    public float weaknessMultiplier = 1.5f; // e.g., 1.5 means 50% more damage taken from weakness
    public LootTable dropTable;
    public float maxHealth; // set from stats
    public float defense; // set from stats
    public float damage; // set from stats
    public float goldDropped;
    public float experienceDropped;
}
