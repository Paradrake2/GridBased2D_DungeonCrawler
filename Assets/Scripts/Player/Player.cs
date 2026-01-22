using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerDebuffInflictorHolder
{
    public Debuff debuff;
    public float value;
}
[System.Serializable]
public class PlayerDebuffResistanceHolder
{
    public Resistance resistance;
    public float resistanceValue;
}


public class Player : MonoBehaviour
{
    public PlayerStats stats;
    public PlayerCombat combat;
    public EquipmentManager equipmentManager;
    public PlayerMovement playerMovement;
    public PlayerDebuffManager debuffManager;
    public Inventory inventory;
    public PotionManager potionManager;
    public int level = 1;
    public float health;
    public float damage;
    public float defense;
    public float attackSpeed;
    public float experience;
    public float experienceToNextLevel = 100f;
    public int gold; // amount of gold gained this run
    public Rigidbody2D rb;
    public Collider2D coll;
    public StatCollection statCol = new StatCollection();
    public PlayerAttributeSet attributeSet = new PlayerAttributeSet();
    public List<PlayerDebuffInflictorHolder> debuffInflictors = new List<PlayerDebuffInflictorHolder>();
    public List<PlayerDebuffResistanceHolder> debuffResistances = new List<PlayerDebuffResistanceHolder>();

    [SerializeField] private float healthOnLevelUp = 10f;
    [SerializeField] private float damageOnLevelUp = 1f;
    [SerializeField] private float defenseOnLevelUp = 0.5f;
    public void Levelup()
    {
        level++;
        health += healthOnLevelUp;
        damage += damageOnLevelUp;
        defense += defenseOnLevelUp;
        experience -= experienceToNextLevel;
        experienceToNextLevel *= 1.2f;
    }
    void Initialize()
    {
        stats = FindFirstObjectByType<PlayerStats>();
        combat = GetComponent<PlayerCombat>();
        equipmentManager = FindAnyObjectByType<EquipmentManager>();
        playerMovement = GetComponent<PlayerMovement>();
        debuffManager = GetComponent<PlayerDebuffManager>();
        inventory = FindAnyObjectByType<Inventory>();
        potionManager = FindAnyObjectByType<PotionManager>();
        health = stats.baseHealth;
        damage = stats.baseDamage;
        defense = stats.baseDefense;
        attackSpeed = stats.baseAttackSpeed;
        
        StatDatabase db = StatDatabase.Instance;
        StatType Health = db.GetStat("Health");
        StatType Damage = db.GetStat("Damage");
        StatType Defense = db.GetStat("Defense");
        StatType AttackSpeed = db.GetStat("AttackSpeed");

        gold = 0;
        statCol.SetStat(Health, stats.baseHealth);
        statCol.SetStat(Damage, stats.baseDamage);
        statCol.SetStat(Defense, stats.baseDefense);
        statCol.SetStat(AttackSpeed, stats.baseAttackSpeed);
        SetUpAttributeValues();
        UpdateFromEquipment();
    }
    public void UpdateStatValues(string statID, float value)
    {
        StatDatabase db = StatDatabase.Instance;
        StatType statType = db.GetStat(statID);
        float previousValue = statCol.GetStat(statType);
        statCol.SetStat(statType, previousValue + value);
        UpdateBasicStatValues();
    }
    public void AddItemToInventory(Item item)
    {
        inventory.AddItem(item, 1);
    }
    void UpdateBasicStatValues()
    {
        health = statCol.GetStat("Health");
        damage = statCol.GetStat("Damage");
        defense = statCol.GetStat("Defense");
        attackSpeed = statCol.GetStat("AttackSpeed");
    }
    public void UpdateFromEquipment()
    {
        StatCollection equipmentStats = equipmentManager.GetEquipmentStats();
        foreach (var stat in equipmentStats.Stats)
        {
            if (stat.StatType.category == StatCategory.Attribute)
            {
                AdjustAttributeValue(stat.StatType, stat.Value);
                Debug.Log("Adjusted attribute " + stat.StatType.displayName + " by " + stat.Value);
            } else
            {
                float baseValue = stats.stats.GetStat(stat.GetStatID());
                statCol.SetStat(StatDatabase.Instance.GetStat(stat.GetStatID()), baseValue + stat.Value);
            }
        }
        UpdateBasicStatValues();
    }
    public void UpdateDebuffInflictors()
    {
        debuffInflictors.Clear();
        debuffInflictors = equipmentManager.GetEquipmentDebuffInflictors();
        // get potion debuff inflictors
        foreach (var slotInfo in equipmentManager.equipment)
        {
            Equipment eq = slotInfo.equippedItem;
            {
                if (eq == null) continue;
                List<PlayerDebuffResistanceHolder> eqResistances = eq.GetDebuffResistances();
                debuffResistances.AddRange(eqResistances);
            }
        }
    }
    void SetUpAttributeValues() // Sets up the players attribute values from PlayerStats
    {
        attributeSet = stats.GetPlayerAttributeSet();
    }
    public void AdjustAttributeValue(StatType attribute, float value)
    {
        foreach (var attr in attributeSet.attackAttributes)
        {
            if (attr.attackAttribute == attribute)
            {
                attr.attackAttributeValue += value;
                return;
            }
        }
        foreach (var attr in attributeSet.defenseAttributes)
        {
            if (attr.defenseAttribute == attribute)
            {
                attr.defenseAttributeValue += value;
                return;
            }
        }
    }
    public void RecalculateAllValues()
    {
        health = stats.baseHealth;
        damage = stats.baseDamage;
        defense = stats.baseDefense;
        attackSpeed = stats.baseAttackSpeed;
        
        StatDatabase db = StatDatabase.Instance;
        StatType Health = db.GetStat("Health");
        StatType Damage = db.GetStat("Damage");
        StatType Defense = db.GetStat("Defense");
        StatType AttackSpeed = db.GetStat("AttackSpeed");

        gold = 0;
        statCol.SetStat(Health, stats.baseHealth);
        statCol.SetStat(Damage, stats.baseDamage);
        statCol.SetStat(Defense, stats.baseDefense);
        statCol.SetStat(AttackSpeed, stats.baseAttackSpeed);
        ClearAllAttributes();
        SetUpAttributeValues();
        equipmentManager.SetAllEquipmentCountedFalse();
        UpdateFromEquipment();
        UpdateDebuffInflictors();
    }
    public float GetAttributeValue(StatType attribute)
    {
        foreach (var attr in attributeSet.attackAttributes)
        {
            if (attr.attackAttribute == attribute)
            {
                return attr.attackAttributeValue;
            }
        }
        foreach (var attr in attributeSet.defenseAttributes)
        {
            if (attr.defenseAttribute == attribute)
            {
                return attr.defenseAttributeValue;
            }
        }
        return 0f;
    }
    public void TakeDamage(float amount)
    {
        health -= Mathf.Max(0, amount - defense);
        if (health <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        // Handle player death (e.g., respawn, game over)
    }
    public float GetDropChance()
    {
        return statCol.GetStat("DropChance");
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            float damageMult = Mathf.Min(3f, playerMovement.GetDistanceTraveled()); // damage multiplier based on distance traveled, capped at 3x
            if (enemy != null)
            {
                combat.StartCombat(other.gameObject, attackSpeed, GetAttackAttributes(), damageMult, debuffInflictors);
                EnemyManager.instance.InCombatWith(enemy);
            }
        }
    }
    public List<PlayerAttackAttributes> GetAttackAttributes()
    {
        return attributeSet.GetAttackAttributes();
    }
    public void ClearAllAttributes()
    {
        foreach (var attr in attributeSet.attackAttributes)
        {
            attr.attackAttributeValue = 0f;
        }
        foreach (var attr in attributeSet.defenseAttributes)
        {
            attr.defenseAttributeValue = 0f;
        }
        attributeSet = stats.attributeSet;
    }
    public void AddXP(float amount)
    {
        experience += amount;
        if (experience >= experienceToNextLevel)
        {
            Levelup();
        }
    }
    public void AddGold(int amount)
    {
        gold += amount;
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
