using System.Collections.Generic;
using UnityEngine;

public enum EquipmentRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}


[System.Serializable]
public class EquipmentTemplateStat
{
    public StatType stat;
    public float minValue;
    public float maxValue;
    public bool isInt = true;
}
[System.Serializable]
public class EquipmentTemplateDebuff
{
    public Debuff debuffType;
    public float minValue;
    public float maxValue;
}
[System.Serializable]
public class EquipmentTemplateBuff
{
    public Buff buffType;
    public float minValue;
    public float maxValue;
}

[CreateAssetMenu(fileName = "EquipmentTemplate", menuName = "EquipmentLoot/EquipmentTemplate")]
public class EquipmentTemplate : ScriptableObject
{
    public string templateName;
    public Equipment baseEquipment;
    public List<EquipmentTemplateStat> stats;
    public List<EquipmentTemplateDebuff> potentialDebuffs;
    public List<EquipmentTemplateStat> guaranteedStats;
    public int minDebuffs = 0;
    public int maxDebuffs = 1;
    public List<EquipmentTemplateBuff> potentialBuffs;
    public int minBuffs = 0;
    public int maxBuffs = 1;
    public int minStats = 1;
    public int maxStats = 3;
    public EquipmentRarity rarity;
    public Equipment GenerateEquipment()
    {
        Equipment newEquipment = Instantiate(baseEquipment);
        int statCount = GetRandomInt(minStats, maxStats);

        for (int i = 0; i < statCount; i++)
        {
            EquipmentTemplateStat stat = stats[Random.Range(0, stats.Count)];
            if (stat.isInt)
            {
                int value = (int)Random.Range(stat.minValue, stat.maxValue) * Manager.instance.currentFloor;
                newEquipment.stats.AddStat(stat.stat, value);
            }
            else
            {
                float value = Random.Range(stat.minValue, stat.maxValue) * Manager.instance.currentFloor;
                value = Mathf.Round(value * 100f) / 100f; // Round to 2 decimal places
                newEquipment.stats.AddStat(stat.stat, value);
            }
        }

        int debuffCount = GetRandomInt(minDebuffs, maxDebuffs);
        for (int i = 0; i < debuffCount; i++)
        {
            PlayerDebuffInflictorHolder inflictor = GenerateDebuff();
            newEquipment.AddDebuffInflictor(inflictor);
        }
        foreach (var guaranteedStat in guaranteedStats)
        {
            if (guaranteedStat.isInt)
            {
                int value = (int)Random.Range(guaranteedStat.minValue, guaranteedStat.maxValue) * Manager.instance.currentFloor;
                newEquipment.stats.AddStat(guaranteedStat.stat, value);
            }
            else
            {
                float value = Random.Range(guaranteedStat.minValue, guaranteedStat.maxValue) * Manager.instance.currentFloor;
                value = Mathf.Round(value * 100f) / 100f; // Round to 2 decimal places
                newEquipment.stats.AddStat(guaranteedStat.stat, value);
            }
        }
        // Generate buffs
        return newEquipment;
    }
    public PlayerDebuffInflictorHolder GenerateDebuff()
    {
        EquipmentTemplateDebuff debuff = potentialDebuffs[Random.Range(0, potentialDebuffs.Count)];
        float value = Random.Range(debuff.minValue, debuff.maxValue) * Manager.instance.currentFloor;
        PlayerDebuffInflictorHolder inflictor = new PlayerDebuffInflictorHolder
        {
            debuff = debuff.debuffType,
            value = value
        };
        return inflictor;
    }
    public int GetRandomInt(int min, int max)
    {
        return Random.Range(min, max + 1);
    }
}
