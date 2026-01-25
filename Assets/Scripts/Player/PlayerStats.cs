using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerAttackAttributes
{
    public StatType attackAttribute;
    public float attackAttributeValue;
}
[System.Serializable]
public class PlayerDefenseAttributes
{
    public StatType defenseAttribute;
    public float defenseAttributeValue;
}
[System.Serializable]
public class PlayerAttributeSet
{
    public List<PlayerAttackAttributes> attackAttributes;
    public List<PlayerDefenseAttributes> defenseAttributes;
    public List<PlayerAttackAttributes> GetAttackAttributes()
    {
        return attackAttributes;
    }
    public List<PlayerDefenseAttributes> GetDefenseAttributes()
    {
        return defenseAttributes;
    }
    public void AddAttackAttribute(StatType attribute, float value)
    {
        PlayerAttackAttributes newAttr = new PlayerAttackAttributes
        {
            attackAttribute = attribute,
            attackAttributeValue = value
        };
        attackAttributes.Add(newAttr);
    }
    public void AddDefenseAttribute(StatType attribute, float value)
    {
        PlayerDefenseAttributes newAttr = new PlayerDefenseAttributes
        {
            defenseAttribute = attribute,
            defenseAttributeValue = value
        };
        defenseAttributes.Add(newAttr);
    }
    public void ClearAllAttributes()
    {
        foreach (var attr in attackAttributes)
        {
            attr.attackAttributeValue = 0f;
        }
        foreach (var attr in defenseAttributes)
        {
            attr.defenseAttributeValue = 0f;
        }
    }
}

public class PlayerStats : MonoBehaviour
{
    [Header("Player Stats")]
    public StatCollection stats = new StatCollection();
    public PlayerAttributeSet attributeSet = new PlayerAttributeSet();
    [SerializeField] private float baseHealth = 100f;
    [SerializeField] private float baseDamage = 2f;
    [SerializeField] private float baseDefense = 0f;
    [SerializeField] private float baseAttackSpeed = 1f;
    [SerializeField] private float baseCraftingEfficiency = 1f;
    public int goldAmount = 0;
    public static PlayerStats instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        SetupStats();
    }
    void SetupStats()
    {
        StatDatabase db = StatDatabase.Instance;
        StatType Health = db.GetStat("Health");
        StatType Damage = db.GetStat("Damage");
        StatType Defense = db.GetStat("Defense");
        StatType AttackSpeed = db.GetStat("AttackSpeed");

        goldAmount = 0;
        stats.SetStat(Health, baseHealth);
        stats.SetStat(Damage, baseDamage);
        stats.SetStat(Defense, baseDefense);
        stats.SetStat(AttackSpeed, baseAttackSpeed);
    }
    public float GetStatValue(string statID)
    {
        return stats.GetStat(statID);
    }
    public float GetAttackAttributeValue(StatType attackAttribute)
    {
        foreach (PlayerAttackAttributes attr in attributeSet.attackAttributes)
        {
            if (attr.attackAttribute == attackAttribute)
            {
                return attr.attackAttributeValue;
            }
        }
        return 0f;
    }
    public float GetDefenseAttributeValue(StatType defenseAttribute)
    {
        foreach (PlayerDefenseAttributes attr in attributeSet.defenseAttributes)
        {
            if (attr.defenseAttribute == defenseAttribute)
            {
                return attr.defenseAttributeValue;
            }
        }
        return 0f;
    }
    public PlayerAttributeSet GetPlayerAttributeSet()
    {
        PlayerAttributeSet copy = new PlayerAttributeSet();
        copy.attackAttributes = new List<PlayerAttackAttributes>();
        copy.defenseAttributes = new List<PlayerDefenseAttributes>();
        
        // Copy attack attributes
        foreach (var attr in attributeSet.attackAttributes)
        {
            copy.attackAttributes.Add(new PlayerAttackAttributes
            {
                attackAttribute = attr.attackAttribute,
                attackAttributeValue = attr.attackAttributeValue
            });
        }
        
        // Copy defense attributes
        foreach (var attr in attributeSet.defenseAttributes)
        {
            copy.defenseAttributes.Add(new PlayerDefenseAttributes
            {
                defenseAttribute = attr.defenseAttribute,
                defenseAttributeValue = attr.defenseAttributeValue
            });
        }
        
        return copy;
    }
    public void AddStat(StatType statType, float value)
    {
        stats.AddStat(statType, value);
    }
    public float GetBaseCraftingEfficiency()
    {
        return baseCraftingEfficiency;
    }
    public float GetBaseAttackSpeed()
    {
        return baseAttackSpeed;
    }
    public float GetBaseHealth()
    {
        return baseHealth;
    }
    public float GetBaseDamage()
    {
        return baseDamage;
    }
    public float GetBaseDefense()
    {
        return baseDefense;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
