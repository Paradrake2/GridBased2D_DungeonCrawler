using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "EnemyStatsHolder", menuName = "Enemy/EnemyStatsHolder")]
public class EnemyStatsHolder : ScriptableObject
{
    public StatCollection stats = new StatCollection();
    public List<EnemyAttributes> enemyAttributesList = new List<EnemyAttributes>();
    public LootTable dropTable;
    public float maxHealth; // set from stats
    public float defense; // set from stats
    public float damage; // set from stats
    public float goldDropped;
    public float experienceDropped;
}
