using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public List<Equipment> equipment = new List<Equipment>();
    public StatCollection GetEquipmentStats()
    {
        StatCollection totalStats = new StatCollection();
        foreach (Equipment eq in equipment)
        {
            if (eq.alreadyCounted) continue;
            StatCollection eqStats = eq.GetStats();
            foreach (var stat in eqStats.Stats)
            {
                float currentValue = totalStats.GetStat(stat.GetStatID());
                totalStats.SetStat(StatDatabase.Instance.GetStat(stat.GetStatID()), currentValue + stat.Value);
            }
            eq.alreadyCounted = true;
        }
        return totalStats;
    }
    public void SetAllEquipmentCountedFalse()
    {
        foreach (Equipment eq in equipment)
        {
            eq.alreadyCounted = false;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
