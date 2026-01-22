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
    
    public EnemyStatsHolder esh;
    public float currentHealth;

    public float GetAttributeValue(StatType attribute)
    {
        foreach (var attr in esh.enemyAttributesList)
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
    public void ModifyDefenseMultiplier(float multiplier)
    {
        esh.defense *= multiplier;
    }
    public void ModifyDefenseValue(float value)
    {
        esh.defense += value;
    }
    public void ModifyDamageMultiplier(float multiplier)
    {
        esh.damage *= multiplier;
    }
    public void ModifyDamageValue(float value)
    {
        esh.damage += value;
    }
    public void Initialize()
    {
        esh = Instantiate(esh);
        esh.maxHealth = esh.stats.GetStat(StatDatabase.Instance.GetStat("Health"));
        currentHealth = esh.maxHealth;
        esh.defense = esh.stats.GetStat(StatDatabase.Instance.GetStat("Defense"));
        esh.damage = esh.stats.GetStat(StatDatabase.Instance.GetStat("Damage"));
    }
    void Start()
    {
        Initialize();
    }
}
