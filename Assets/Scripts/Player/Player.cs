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
    [SerializeField] private float maxHealth;
    [SerializeField] private float damage;
    [SerializeField] private float defense;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float craftingEfficiency;
    [SerializeField] private float currentHealth;
    [SerializeField] private int magic;
    public Rigidbody2D rb;
    public Collider2D coll;
    public StatCollection statCol = new StatCollection();
    public PlayerAttributeSet attributeSet = new PlayerAttributeSet();
    public List<PlayerDebuffInflictorHolder> debuffInflictors = new List<PlayerDebuffInflictorHolder>();
    public List<PlayerDebuffResistanceHolder> debuffResistances = new List<PlayerDebuffResistanceHolder>();
    [SerializeField] private StatCollection spellStats = new StatCollection();
    [SerializeField] private PlayerAttributeSet tempAttributeSet = new PlayerAttributeSet();
    [SerializeField] private SpellBehaviour currentSpellBehaviour;
    public bool isInCombat = false;

    void Initialize()
    {
        stats = FindFirstObjectByType<PlayerStats>();
        combat = GetComponent<PlayerCombat>();
        equipmentManager = FindAnyObjectByType<EquipmentManager>();
        playerMovement = GetComponent<PlayerMovement>();
        debuffManager = GetComponent<PlayerDebuffManager>();
        inventory = FindAnyObjectByType<Inventory>();
        potionManager = FindAnyObjectByType<PotionManager>();
        maxHealth = stats.GetBaseHealth();
        currentHealth = maxHealth;
        damage = stats.GetBaseDamage();
        defense = stats.GetBaseDefense();
        attackSpeed = stats.GetBaseAttackSpeed();
        craftingEfficiency = stats.GetBaseCraftingEfficiency();
        magic = stats.GetMagic();
        
        StatDatabase db = StatDatabase.Instance;
        StatType Health = db.GetStat("Health");
        StatType Damage = db.GetStat("Damage");
        StatType Defense = db.GetStat("Defense");
        StatType AttackSpeed = db.GetStat("AttackSpeed");
        StatType CraftingEfficiency = db.GetStat("CraftingEfficiency");
        StatType Magic = db.GetStat("Magic");

        statCol.SetStat(Health, stats.GetBaseHealth());
        statCol.SetStat(Damage, stats.GetBaseDamage());
        statCol.SetStat(Defense, stats.GetBaseDefense());
        statCol.SetStat(AttackSpeed, stats.GetBaseAttackSpeed());
        statCol.SetStat(CraftingEfficiency, stats.GetBaseCraftingEfficiency());
        statCol.SetStat(Magic, stats.GetMagic());
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
        maxHealth = statCol.GetStat("Health");
        damage = statCol.GetStat("Damage");
        defense = statCol.GetStat("Defense");
        attackSpeed = statCol.GetStat("AttackSpeed");
        craftingEfficiency = statCol.GetStat("CraftingEfficiency");
        magic = (int)statCol.GetStat("Magic");
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
        maxHealth = stats.GetBaseHealth();
        damage = stats.GetBaseDamage();
        defense = stats.GetBaseDefense();
        attackSpeed = stats.GetBaseAttackSpeed();
        
        StatDatabase db = StatDatabase.Instance;
        StatType Health = db.GetStat("Health");
        StatType Damage = db.GetStat("Damage");
        StatType Defense = db.GetStat("Defense");
        StatType AttackSpeed = db.GetStat("AttackSpeed");
        StatType CraftingEfficiency = db.GetStat("CraftingEfficiency");

        statCol.SetStat(Health, stats.GetBaseHealth());
        statCol.SetStat(Damage, stats.GetBaseDamage());
        statCol.SetStat(Defense, stats.GetBaseDefense());
        statCol.SetStat(AttackSpeed, stats.GetBaseAttackSpeed());
        statCol.SetStat(CraftingEfficiency, stats.GetBaseCraftingEfficiency());
        ClearAllAttributes();
        SetUpAttributeValues();
        equipmentManager.SetAllEquipmentCountedFalse();
        UpdateFromEquipment();
        UpdateDebuffInflictors();
        PlayerStatsShower statsShower = FindAnyObjectByType<PlayerStatsShower>();
        statsShower.UpdateStats();
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
    public void TakeDamage(float amount, List<EnemyAttributes> attributeDamages)
    {
        List<StatValue> attributeValues = new List<StatValue>();
        foreach (var attr in attributeDamages)
        {
            attributeValues.Add(new StatValue(attr.attackAttribute, attr.attackAttributeValue));
        }
        currentHealth -= Mathf.Max(0, CalculateDamageTaken(amount, attributeDamages));
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public float CalculateDamageTaken(float damage, List<EnemyAttributes> attributeDamages)
    {
        float totalDamage = Mathf.Max(1, damage - defense);
        if (attributeDamages == null) return totalDamage;
        foreach (var attr in attributeDamages)
        {
            StatType playerDefenseAttr = AttributeManager.instance.GetCorrespondingDefenseAttribute(attr.attackAttribute);
            float playerDefenseValue = GetAttributeValue(playerDefenseAttr);
            float potentialDamage = Mathf.Max(0, attr.attackAttributeValue - playerDefenseValue);
            totalDamage += potentialDamage;
        }
        return totalDamage;
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
        Debug.Log("Player collided with " + other.gameObject.name);
        if (other.gameObject.layer == LayerMask.NameToLayer("EnemyHitbox") && !isInCombat && playerMovement.hasMoved)
        {
            Enemy enemy = other.GetComponentInParent<Enemy>();
            float damageMult = Mathf.Min(3f, playerMovement.GetDistanceTraveled()); // damage multiplier based on distance traveled, capped at 3x
            if (enemy != null)
            {
                isInCombat = true;
                combat.StartCombat(enemy.gameObject, attackSpeed, damageMult, debuffInflictors);
                EnemyManager.instance.InCombatWith(enemy);
            }
        }
        /**
        if (other.CompareTag("EnemyHitbox") && !isInCombat && playerMovement.hasMoved)
        {
            Enemy enemy = other.GetComponentInParent<Enemy>();
            float damageMult = Mathf.Min(3f, playerMovement.GetDistanceTraveled()); // damage multiplier based on distance traveled, capped at 3x
            if (enemy != null)
            {
                combat.StartCombat(enemy.gameObject, attackSpeed, GetAttackAttributes(), damageMult, debuffInflictors);
                EnemyManager.instance.InCombatWith(enemy);
                isInCombat = true;
            }
        }
        **/
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

    public StatCollection GetStats()
    {
        return statCol;
    }
    public PlayerAttributeSet GetAttributeSet()
    {
        return attributeSet;
    }
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public float GetHealth()
    {
        return currentHealth;
    }
    public float GetMaxHealth()
    {
        return maxHealth;
    }
    public float GetDefense()
    {
        return defense;
    }
    public float GetAttackSpeed()
    {
        return attackSpeed;
    }
    public float GetCraftingEfficiency()
    {
        return craftingEfficiency;
    }
    public float GetDamage()
    {
        return damage;
    }
    public int GetMagic()
    {
        return magic;
    }
    public StatCollection GetSpellStats()
    {
        return spellStats;
    }
    public void SetSpellStats(StatCollection stats)
    {
        spellStats = stats;
    }
    public void ClearSpellStats()
    {
        spellStats.Clear();
    }
    public PlayerAttributeSet GetTempPlayerAttributeSet()
    {
        return tempAttributeSet;
    }
    public void SetTempPlayerAttributeSet(PlayerAttributeSet set)
    {
        tempAttributeSet = set;
    }
    public void ClearTempAttributeSet()
    {
        tempAttributeSet.ClearAllAttributes();
    }
    public void ClearTempAttributeAttackStats()
    {
        tempAttributeSet.ClearAttackAttributes();
    }
    public void ClearTempAttributeDefenseStats()
    {
        tempAttributeSet.ClearDefenseAttributes();
    }
    public void SetSpellBehaviour(SpellBehaviour sb)
    {
        currentSpellBehaviour = sb;
    }
    public SpellBehaviour GetSpellBehaviour()
    {
        return currentSpellBehaviour;
    }
}
