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
    public float GetStat(StatValue statValue)
    {
        var stat = stats.FirstOrDefault(s => s.StatType == statValue.StatType);
        return stat?.Value ?? (statValue.StatType?.defaultValue ?? 0f);
    }
    /// <summary>
    /// Get stat by ID string
    /// </summary>
    public bool TryGetStat(string statName, out float value)
    {
        value = 0f;
        if (string.IsNullOrWhiteSpace(statName)) return false;

        // If you use a Dictionary<string, float>:
        // return _stats != null && _stats.TryGetValue(statName, out value);

        // If you use a List/array of stat objects (example):
        if (stats == null) return false;
        for (int i = 0; i < stats.Count; i++)
        {
            if (stats[i] != null && stats[i].StatType.name == statName)
            {
                value = stats[i].Value;
                return true;
            }
        }

        return false;
    }
    public float GetStat(string statID)
    {
        return TryGetStat(statID, out float v) ? v : 0f; // default to 0 when missing
    }
    
    /// <summary>
    /// Check if this collection has a specific stat
    /// </summary>
    public bool HasStat(StatType statType)
    {
        return stats.Any(s => s.StatType == statType);
    }
    public void AddStat(StatValue statValue)
    {
        var existingStat = stats.FirstOrDefault(s => s.StatType == statValue.StatType);
        if (existingStat != null)
        {
            existingStat.SetValue(statValue.Value);
        }
        else
        {
            stats.Add(statValue);
        }
    }
    public void AddStat(StatType statType, float value)
    {
        AddStat(new StatValue(statType, value));
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