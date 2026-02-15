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

    public PlayerAttributeSet()
    {
        attackAttributes ??= new List<PlayerAttackAttributes>();
        defenseAttributes ??= new List<PlayerDefenseAttributes>();
    }

    private void EnsureLists()
    {
        attackAttributes ??= new List<PlayerAttackAttributes>();
        defenseAttributes ??= new List<PlayerDefenseAttributes>();
    }

    public List<PlayerAttackAttributes> GetAttackAttributes()
    {
        EnsureLists();
        return attackAttributes;
    }
    public List<PlayerDefenseAttributes> GetDefenseAttributes()
    {
        EnsureLists();
        return defenseAttributes;
    }
    public void AddAttackAttribute(StatType attribute, float value)
    {
        EnsureLists();
        PlayerAttackAttributes newAttr = new PlayerAttackAttributes
        {
            attackAttribute = attribute,
            attackAttributeValue = value
        };
        attackAttributes.Add(newAttr);
    }

    public void AddOrSetAttackAttribute(StatType attribute, float value)
    {
        EnsureLists();
        for (int i = 0; i < attackAttributes.Count; i++)
        {
            if (attackAttributes[i] != null && attackAttributes[i].attackAttribute == attribute)
            {
                attackAttributes[i].attackAttributeValue = value;
                return;
            }
        }
        AddAttackAttribute(attribute, value);
    }
    public void AddDefenseAttribute(StatType attribute, float value)
    {
        EnsureLists();
        PlayerDefenseAttributes newAttr = new PlayerDefenseAttributes
        {
            defenseAttribute = attribute,
            defenseAttributeValue = value
        };
        defenseAttributes.Add(newAttr);
    }
    public void ClearAllAttributes()
    {
        EnsureLists();
        foreach (var attr in attackAttributes)
        {
            attr.attackAttributeValue = 0f;
        }
        foreach (var attr in defenseAttributes)
        {
            attr.defenseAttributeValue = 0f;
        }
    }
    public void ClearAttackAttributes()
    {
        EnsureLists();
        attackAttributes.Clear();
    }
    public void ClearDefenseAttributes()
    {
        EnsureLists();
        defenseAttributes.Clear();
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
    [SerializeField] private float baseDropChance = 0.05f;
    [SerializeField] private int goldAmount = 0; // not a StatType
    [SerializeField] private float xp = 0; // total xp
    [SerializeField] private float experienceToNextLevel = 100f;
    [SerializeField] private float currentXP = 0f;
    [SerializeField] private int playerLevel = 1;
    [SerializeField] private int magic = 0;
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
        StatType CraftingEfficiency = db.GetStat("CraftingEfficiency");
        goldAmount = 0;
        stats.SetStat(Health, baseHealth);
        stats.SetStat(Damage, baseDamage);
        stats.SetStat(Defense, baseDefense);
        stats.SetStat(AttackSpeed, baseAttackSpeed);
        stats.SetStat(CraftingEfficiency, baseCraftingEfficiency);
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
    public void LevelUp()
    {
        playerLevel++;
        experienceToNextLevel *= 1.2f;

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
    public float GetBaseDropChance()
    {
        return baseDropChance;
    }
    public void SetBaseDropChance(float newChance)
    {
        baseDropChance = newChance;
    }
    public int GetGoldAmount()
    {
        return goldAmount;
    }
    public void SetGoldAmount(int newAmount)
    {
        goldAmount = newAmount;
    }
    public void AddGold(int amount)
    {
        goldAmount += amount;
    }
    public float GetExperience()
    {
        return xp;
    }
    public float GetCurrentExperience()
    {
        return currentXP;
    }
    public float GetExperienceToNextLevel()
    {
        return experienceToNextLevel;
    }
    public void SetExperience(float newXP)
    {
        xp = newXP;
    }
    public void AddExperience(float amount)
    {
        xp += amount;
    }
    public int GetPlayerLevel()
    {
        return playerLevel;
    }
    public int GetMagic()
    {
        return magic;
    }
    public void SetMagic(int newMagic)
    {
        magic = newMagic;
    }
}