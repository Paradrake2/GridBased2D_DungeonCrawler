using System;
using UnityEngine;

/// <summary>
/// Represents a single stat value with its type and amount
/// This replaces the need for hardcoded stat enums
/// </summary>
[Serializable]
public class StatValue
{
    [SerializeField] private StatType statType;
    [SerializeField] private float value;
    
    public StatType StatType => statType;
    public float Value => value;
    
    public StatValue(StatType statType, float value)
    {
        this.statType = statType;
        this.value = value;
    }
    
    /// <summary>
    /// Get the formatted display string for this stat
    /// </summary>
    public string GetDisplayText()
    {
        if (statType == null) return "Unknown Stat";
        
        string prefix = value >= 0 ? "+" : "";
        return $"{prefix}{statType.FormatValue(value)} {statType.displayName}";
    }
    
    /// <summary>
    /// Get the stat ID for lookups
    /// </summary>
    public string GetStatID()
    {
        return statType != null ? statType.StatID : "";
    }

    /// <summary>
    /// Set the value with validation
    /// </summary>
    public void SetValue(float newValue)
    {
        if (statType != null)
        {
            value = Mathf.Clamp(newValue, statType.minValue, statType.maxValue);
        }
        else
        {
            value = newValue;
        }
    }
    public Sprite GetIcon()
    {
        return statType != null ? statType.icon : null;
    }
    public Color GetColor()
    {
        return statType.displayColor;
    }
}