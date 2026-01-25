using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipmentDropChance
{
    public EquipmentTemplate equipmentDrop;
    public int weight;
}

[CreateAssetMenu(fileName = "EquipmentDropTable", menuName = "EquipmentLoot/EquipmentDropTable")]
public class EquipmentDropTable : ScriptableObject
{
    public List<EquipmentDropChance> potentialEquipmentDrops;
    public EquipmentTemplate GetEquipmentTemplate()
    {
        int totalWeight = 0;
        List<EquipmentTemplate> weightedDrops = new List<EquipmentTemplate>();
        foreach (var dropChance in potentialEquipmentDrops)
        {
            totalWeight += dropChance.weight;
            for (int i = 0; i < dropChance.weight; i++)
            {
                weightedDrops.Add(dropChance.equipmentDrop);
            }
        }
        if (totalWeight > 0)
        {
            int randomIndex = Random.Range(0, totalWeight);
            return weightedDrops[randomIndex];
        }

        return null; // No equipment dropped
    }
    public Equipment GetEquipment()
    {
        EquipmentTemplate template = GetEquipmentTemplate();
        if (template != null)
        {
            return template.GenerateEquipment();
        }
        return null;
    }
}
