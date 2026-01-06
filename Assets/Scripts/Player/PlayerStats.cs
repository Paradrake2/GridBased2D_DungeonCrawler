using System.Collections.Generic;
using UnityEngine;



public class PlayerStats : MonoBehaviour
{
    [Header("Player Stats")]
    public StatCollection stats = new StatCollection();
    public float baseHealth = 100f;
    public float baseDamage = 2f;
    public float baseDefense = 0f;
    public float baseAttackSpeed = 1f;
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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
