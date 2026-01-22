using UnityEngine;
public enum Attribute
{
    Fire,
    Water,
    Earth,
    Air,
    Light,
    Dark
}

[CreateAssetMenu(fileName = "New Attribute Pairs", menuName = "Stats/Attribute Pairs")]
[System.Serializable]
public class AttributePairs : ScriptableObject
{
    public string pairName;
    public StatType attackAttribute;
    public StatType defenseAttribute;
    public StatType GetDamageAttribute()
    {
        return attackAttribute;
    }
    public StatType GetDefenseAttribute()
    {
        return defenseAttribute;
    }
}
