using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipmentTemplateStat
{
    public StatType stat;
    public float minValue;
    public float maxValue;
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
    public int minDebuffs = 0;
    public int maxDebuffs = 1;
    public List<EquipmentTemplateBuff> potentialBuffs;
    public int minBuffs = 0;
    public int maxBuffs = 1;

    public Equipment GenerateEquipment()
    {
        Equipment newEquipment = Instantiate(baseEquipment);
        foreach (var stat in stats)
        {
            float value = Random.Range(stat.minValue, stat.maxValue);
            newEquipment.stats.AddStat(stat.stat, value);
        }
        int debuffCount = GetRandomInt(minDebuffs, maxDebuffs);
        for (int i = 0; i < debuffCount; i++)
        {
            PlayerDebuffInflictorHolder inflictor = GenerateDebuff();
            newEquipment.AddDebuffInflictor(inflictor);
        }
        // Generate buffs
        return newEquipment;
    }
    public PlayerDebuffInflictorHolder GenerateDebuff()
    {
        EquipmentTemplateDebuff debuff = potentialDebuffs[Random.Range(0, potentialDebuffs.Count)];
        float value = Random.Range(debuff.minValue, debuff.maxValue);
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
