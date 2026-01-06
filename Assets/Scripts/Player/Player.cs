using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerStats stats;
    public PlayerCombat combat;
    public EquipmentManager equipmentManager;
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
        equipmentManager = GetComponent<EquipmentManager>();
        
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
            float baseValue = stats.stats.GetStat(stat.GetStatID());
            statCol.SetStat(StatDatabase.Instance.GetStat(stat.GetStatID()), baseValue + stat.Value);
        }
        UpdateBasicStatValues();
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
            if (enemy != null)
            {
                combat.StartCombat(other.gameObject, attackSpeed);
                EnemyManager.instance.InCombatWith(enemy);
            }
        }
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
