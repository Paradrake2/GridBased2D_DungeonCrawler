using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class EnemyAttributes
{
    public StatType attackAttribute;
    public float attackAttributeValue;
    public StatType defenseAttribute;
    public float defenseAttributeValue;
}

public class EnemyStats : MonoBehaviour
{
    public StatCollection stats = new StatCollection();
    public List<EnemyAttributes> enemyAttributesList = new List<EnemyAttributes>();
    public float maxHealth;
    public float currentHealth;
    public float defense;
    public float damage;
    public float goldDropped;
    public float experienceDropped;
    public float GetAttributeValue(StatType attribute)
    {
        foreach (var attr in enemyAttributesList)
        {
            if (attr.attackAttribute == attribute)
            {
                return attr.attackAttributeValue;
            }
            if (attr.defenseAttribute == attribute)
            {
                return attr.defenseAttributeValue;
            }
        }
        return 0f;
    }
    public void Initialize()
    {
        maxHealth = stats.GetStat(StatDatabase.Instance.GetStat("Health"));
        currentHealth = maxHealth;
        defense = stats.GetStat(StatDatabase.Instance.GetStat("Defense"));
        damage = stats.GetStat(StatDatabase.Instance.GetStat("Damage"));
    }
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
