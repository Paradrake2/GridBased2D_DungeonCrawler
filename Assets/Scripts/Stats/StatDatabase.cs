using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;


[CreateAssetMenu(fileName = "Stat Database", menuName = "Stats/Stat Database")]
public class StatDatabase : ScriptableObject
{
    [Header("All Available Stats")]
    [SerializeField] private List<StatType> allStats = new List<StatType>();
    

    // Dictionary for fast lookups by stat ID

    private Dictionary<string, StatType> statLookup;
    
    private void OnEnable()
    {
        BuildLookupDictionary();
    }
    
    private void OnValidate()
    {
        BuildLookupDictionary();
    }
    
    private void BuildLookupDictionary()
    {
        statLookup = new Dictionary<string, StatType>();
        foreach (var stat in allStats.Where(s => s != null))
        {
            statLookup[stat.StatID] = stat;
        }
    }
    

    // Get a stat type by its ID

    public StatType GetStat(string statID)
    {
        if (statLookup == null) BuildLookupDictionary();
        statLookup.TryGetValue(statID, out StatType stat);
        return stat;
    }
    
    // Get all stats of a specific category

    public List<StatType> GetStatsByCategory(StatCategory category)
    {
        return allStats.Where(s => s != null && s.category == category).ToList();
    }
    
    // Get all available stats

    public IReadOnlyList<StatType> GetAllStats()
    {
        return allStats.AsReadOnly();
    }
    

    // Add a new stat type (editor only)
    public void AddStat(StatType stat)
    {
        if (stat != null && !allStats.Contains(stat))
        {
            allStats.Add(stat);
            BuildLookupDictionary();
        }
    }
    
    // Remove a stat type (editor only)
    public void RemoveStat(StatType stat)
    {
        if (allStats.Remove(stat))
        {
            BuildLookupDictionary();
        }
    }

    internal IEnumerable<StatType> GetAllStatTypes()
    {
        return allStats.AsReadOnly();
    }

    // Singleton pattern for easy access
    private static StatDatabase instance;
    public static StatDatabase Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<StatDatabase>("StatDatabase");
                if (instance == null)
                {
                    Debug.LogWarning("StatDatabase not found in Resources folder. Please create one at Resources/StatDatabase");
                }
            }
            return instance;
        }
    }
}