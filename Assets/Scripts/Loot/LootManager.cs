using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipmentLootManagerData
{
    public List<EquipmentDropTable> equipmentDropTables;
    public int minFloor;
    public int maxFloor;
}
[System.Serializable]
public class ItemLootManagerData
{
    public List<LootTable> itemDropTables;
    public int minFloor;
    public int maxFloor;
}

public class LootManager : MonoBehaviour
{
    public List<EquipmentLootManagerData> equipmentLootManagerData;
    public List<ItemLootManagerData> itemLootManagerData;
    public static LootManager Instance;

    public EquipmentDropTable GetEquipmentDropTableForFloor(int floor)
    {
        EquipmentLootManagerData data = equipmentLootManagerData.Find(d => d.minFloor <= floor && d.maxFloor >= floor);
        if (data != null && data.equipmentDropTables.Count > 0)
        {
            int randomIndex = Random.Range(0, data.equipmentDropTables.Count);
            return data.equipmentDropTables[randomIndex];
        }
        return null;
    }
    public LootTable GetItemDropTableForFloor(int floor)
    {
        ItemLootManagerData data = itemLootManagerData.Find(d => d.minFloor <= floor && d.maxFloor >= floor);
        if (data != null && data.itemDropTables.Count > 0)
        {
            int randomIndex = Random.Range(0, data.itemDropTables.Count);
            return data.itemDropTables[randomIndex];
        }
        return null;
    }
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
