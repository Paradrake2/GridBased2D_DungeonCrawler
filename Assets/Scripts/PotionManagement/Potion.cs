using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PotionEffect
{
    public StatCollection addedStats = new StatCollection();
    public List<PlayerDebuffInflictorHolder> debuffInflictors = new List<PlayerDebuffInflictorHolder>();
    public List<PlayerDebuffResistanceHolder> debuffResistances = new List<PlayerDebuffResistanceHolder>();
    public void AddStat(string statName, float value)
    {
        float previousValue = addedStats.GetStat(statName);
        addedStats.SetStat(StatDatabase.Instance.GetStat(statName), previousValue + value);
    }
    public void AddDebuffInflictor(Debuff debuff, float value)
    {
        PlayerDebuffInflictorHolder holder = new PlayerDebuffInflictorHolder
        {
            debuff = debuff,
            value = value
        };
        debuffInflictors.Add(holder);
    }
    public StatCollection GetAddedStats()
    {
        return addedStats;
    }
    public List<PlayerDebuffInflictorHolder> GetDebuffInflictors()
    {
        return debuffInflictors;
    }
    // get resistances when implemented
}


[System.Serializable]
[CreateAssetMenu(fileName = "Potion", menuName = "Scriptable Objects/Potion")]
public class Potion : ScriptableObject
{
    public string potionName;
    public Color color;
    public PotionEffect effect;
    public Sprite icon;
}
