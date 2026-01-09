using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class MaterialStats
{
    public StatCollection stats = new StatCollection();
    public DebuffCollection debuffCollection = new DebuffCollection();
    public float goldValue;
}


[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public GameObject itemPrefab;
    public MaterialStats stats;
    public List<PlayerDebuffInflictorHolder> debuffInflictors = new List<PlayerDebuffInflictorHolder>();
    public List<PlayerDebuffResistanceHolder> debuffResistances = new List<PlayerDebuffResistanceHolder>();
    public List<CraftingSlot> craftingSlots;
    public string id;
    public bool persistent = false;
    public void UpdatePersistance(bool value)
    {
        persistent = value;
    }
    public float GetGoldValue()
    {
        return stats.goldValue;
    }
    public float GetStatValue(string statName)
    {
        return stats.stats.GetStat(statName);
    }
    public List<CraftingSlot> GetCraftingSlots()
    {
        return craftingSlots;
    }
}
