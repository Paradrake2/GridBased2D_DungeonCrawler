using UnityEngine;

[CreateAssetMenu(fileName = "Resistance", menuName = "Resistance/Resistance")]
[System.Serializable]
public class Resistance : ScriptableObject
{
    public string resistanceName;
    public StatType associatedStat;

    public StatType GetAssociatedStat()
    {
        return associatedStat;
    }
    public string GetResistanceName()
    {
        return resistanceName;
    }
}
