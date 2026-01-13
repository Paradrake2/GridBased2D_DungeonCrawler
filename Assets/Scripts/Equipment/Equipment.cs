using System.Collections.Generic;
using UnityEngine;

public enum EquipmentSlot
{
    Helmet,
    Chest,
    Legs,
    Boots,
    Gauntlets,
    Weapon,
    Shield,
    Accessory
}


[CreateAssetMenu(fileName = "Equipment", menuName = "Scriptable Objects/Equipment")]
public class Equipment : ScriptableObject
{
    public string equipmentName;
    public Sprite equipmentIcon;
    public EquipmentSlot equipmentSlot;
    public StatCollection stats = new StatCollection();
    public List<PlayerDebuffInflictorHolder> debuffInflictors = new List<PlayerDebuffInflictorHolder>();
    public List<PlayerDebuffResistanceHolder> debuffResistances = new List<PlayerDebuffResistanceHolder>();
    public string id;
    public bool persistent = false;
    public bool alreadyCounted = false; // prevent equipment from being counted multiple times in stats calculations
    public void UpdatePersistance(bool value)
    {
        persistent = value;
    }
    public StatCollection GetStats()
    {
        return stats;
    }
    public List<PlayerDebuffInflictorHolder> GetDebuffInflictors()
    {
        return debuffInflictors;
    }
    public List<PlayerDebuffResistanceHolder> GetDebuffResistances()
    {
        return debuffResistances;
    }
    public void AddDebuffInflictor(PlayerDebuffInflictorHolder inflictor)
    {
        debuffInflictors.Add(inflictor);
    }
    public void AddDebuffResistance(PlayerDebuffResistanceHolder resistance)
    {
        debuffResistances.Add(resistance);
    }
}