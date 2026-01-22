using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class MaterialStats
{
    public StatCollection stats = new StatCollection();
    public DebuffCollection debuffCollection = new DebuffCollection();
    public float goldValue;
}


[CreateAssetMenu(fileName = "Item", menuName = "Item/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    [SerializeField] private Sprite itemIcon;
    public GameObject itemPrefab;
    [SerializeField] private MaterialStats stats;
    public List<PlayerDebuffInflictorHolder> debuffInflictors = new List<PlayerDebuffInflictorHolder>();
    public List<PlayerDebuffResistanceHolder> debuffResistances = new List<PlayerDebuffResistanceHolder>();
    [SerializeField] private Color color = Color.white;
    [SerializeField] private string id;
    [SerializeField] private bool persistent = false; // does this item disappear after run is over
    public void UpdatePersistence(bool value)
    {
        persistent = value;
    }
    public bool IsPersistent()
    {
        return persistent;
    }
    public string GetID()
    {
        return id;
    }
    public void SetID(string newID)
    {
        id = newID;
    }
    public float GetGoldValue()
    {
        return stats.goldValue;
    }
    public float GetStatValue(string statName)
    {
        return stats.stats.GetStat(statName);
    }
    public Sprite GetIcon()
    {
        return itemIcon;
    }
    public Color GetColor()
    {
        return color;
    }
    public void SetName(string newName)
    {
        itemName = newName;
    }
    public string GetName()
    {
        return itemName;
    }
    public StatCollection GetStats()
    {
        return stats.stats;
    }
}
