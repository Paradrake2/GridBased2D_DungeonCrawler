using UnityEngine;


[CreateAssetMenu(fileName = "New Stat Type", menuName = "Stats/Stat Type")]
public class StatType : ScriptableObject
{
    [Header("Basic Info")]
    public string displayName;
    [TextArea(2, 4)]
    public string description;
    public Sprite icon;
    
    [Header("Stat Properties")]
    public StatCategory category;
    public bool isPercentage;
    public bool capped;
    public float minValue = 0f;
    public float maxValue = 1000f;
    public float defaultValue = 0f;
    public float value = 0f;
    
    [Header("Display")]
    public string suffix = "";
    public int decimalPlaces = 0;
    public Color displayColor = Color.white;
    

    public string StatID => name;

    public string FormatValue(float value)
    {
        string formatted = value.ToString($"F{decimalPlaces}");
        if (isPercentage)
            formatted += "%";
        else if (!string.IsNullOrEmpty(suffix))
            formatted += suffix;
        return formatted;
    }
    
    public float GetValue()
    {
        return value;
    }
}

public enum StatCategory
{
    AttackModifier,
    DefenseModifier,
    Debuff,
    Buff,
    Utility
}

