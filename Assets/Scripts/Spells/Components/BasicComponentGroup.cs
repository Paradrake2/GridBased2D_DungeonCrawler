using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class BCGComponentEntry
{
    public SpellComponent component;
    public bool isUnlocked;
    public int index; // higher index means stronger component
}


// Might not be used, but could be useful for organizing components in the future
[CreateAssetMenu(fileName = "BasicComponentGroup", menuName = "Spells/BasicComponentGroup")]
public class BasicComponentGroup : ScriptableObject
{
    [SerializeField] private Sprite icon;
    [SerializeField] private string groupName;
    [SerializeField] private List<BCGComponentEntry> components;
    public Sprite sprite => icon;
    public string GroupName => groupName;
    public List<SpellComponent> GetComponents() => components != null ? components.ConvertAll(entry => entry.component) : new List<SpellComponent>();
    public List<SpellComponent> GetUnlockedComponents() => components != null ? components.FindAll(entry => entry.isUnlocked).ConvertAll(entry => entry.component) : new List<SpellComponent>();
    public void UnlockComponent(SpellComponent component)
    {
        if (components == null) return;
        BCGComponentEntry entry = components.Find(e => e.component == component);
        if (entry != null)
        {
            entry.isUnlocked = true;
        }
    }
    public bool IsComponentUnlocked(SpellComponent component)
    {
        if (components == null) return false;
        BCGComponentEntry entry = components.Find(e => e.component == component);
        return entry != null && entry.isUnlocked;
    }
    public SpellComponent StrongestUnlockedComponent
    {
        get
        {
            if (components == null) return null;
            BCGComponentEntry strongest = null;
            foreach (var entry in components)
            {
                if (entry.isUnlocked && (strongest == null || entry.index > strongest.index))
                {
                    strongest = entry;
                }
            }
            return strongest != null ? strongest.component : null;
        }
    }

}
