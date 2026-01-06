using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Container for multiple stat values with easy lookup and management
/// </summary>
[System.Serializable]
public class StatCollection
{
    [SerializeField] private List<StatValue> stats = new List<StatValue>();
    
    public IReadOnlyList<StatValue> Stats => stats;
    
    /// <summary>
    /// Add or update a stat value
    /// </summary>
    public void SetStat(StatType statType, float value)
    {
        var existingStat = stats.FirstOrDefault(s => s.StatType == statType);
        if (existingStat != null)
        {
            existingStat.SetValue(value);
        }
        else
        {
            stats.Add(new StatValue(statType, value));
        }
    }
    
    public float GetStat(StatType statType)
    {
        var stat = stats.FirstOrDefault(s => s.StatType == statType);
        return stat?.Value ?? (statType?.defaultValue ?? 0f);
    }
    
    /// <summary>
    /// Get stat by ID string
    /// </summary>
    public float GetStat(string statID)
    {
        var stat = stats.FirstOrDefault(s => s.GetStatID() == statID);
        return stat?.Value ?? 0f;
    }
    
    /// <summary>
    /// Check if this collection has a specific stat
    /// </summary>
    public bool HasStat(StatType statType)
    {
        return stats.Any(s => s.StatType == statType);
    }
    
    /// <summary>
    /// Remove a stat from the collection
    /// </summary>
    public void RemoveStat(StatType statType)
    {
        stats.RemoveAll(s => s.StatType == statType);
    }

    /// <summary>
    /// Add another stat collection to this one
    /// </summary>
    public void AddStats(StatCollection other)
    {
        foreach (var stat in other.Stats)
        {
            if (stat.StatType != null)
            {
                float currentValue = GetStat(stat.StatType);
                SetStat(stat.StatType, currentValue + stat.Value);
            }
        }
    }
    
    public StatType GetStatTypeByName(string statName)
    {
        var stat = stats.FirstOrDefault(s => s.StatType != null && s.StatType.name == statName);
        return stat?.StatType;
    }
    
    /// <summary>
    /// Get all stats grouped by category
    /// </summary>
    public Dictionary<StatCategory, List<StatValue>> GetStatsByCategory()
    {
        return stats
            .Where(s => s.StatType != null)
            .GroupBy(s => s.StatType.category)
            .ToDictionary(g => g.Key, g => g.ToList());
    }
    
    /// <summary>
    /// Clear all stats
    /// </summary>
    public void Clear()
    {
        stats.Clear();
    }
}