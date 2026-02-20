using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "SpellComponentDatabase", menuName = "Scriptable Objects/SpellComponentDatabase")]
public class SpellComponentDatabase : ScriptableObject
{
    [SerializeField] private List<SpellComponent> allSpellComponents = new List<SpellComponent>();
    public List<SpellComponent> AllSpellComponents => allSpellComponents;

    [SerializeField] private List<SpellComponent> unlockedSpellComponents = new List<SpellComponent>();
    public List<SpellComponent> UnlockedSpellComponents => unlockedSpellComponents;
    //[SerializeField] private List<SpellComponent> unlockedDFSpellComponents = new List<SpellComponent>();
    //public List<SpellComponent> UnlockedDFSpellComponents => unlockedDFSpellComponents;
    [SerializeField] private List<DFComponentGroup> dFComponentGroups = new List<DFComponentGroup>();
    public List<DFComponentGroup> DFComponentGroups => dFComponentGroups;
    public List<SpellComponent> GetAllComponents() => allSpellComponents;
    public List<SpellComponent> GetUnlockedComponents() => unlockedSpellComponents;
    //public List<SpellComponent> GetUnlockedDFComponents() => unlockedDFSpellComponents;

    public List<SpellComponent> GetComponentsByTypeAll(SpellComponentType type)
        => allSpellComponents.FindAll(component => component != null && component.ComponentType == type);

    public List<SpellComponent> GetComponentsByTypeUnlocked(SpellComponentType type)
        => unlockedSpellComponents.FindAll(component => component != null && component.ComponentType == type);
    public List<SpellComponent> GetAllComponentsByCategory(ComponentCategory category)
        => allSpellComponents.FindAll(component => component != null && component.ComponentCategory == category);
    public List<SpellComponent> GetUnlockedComponentsByCategory(ComponentCategory category)
        => unlockedSpellComponents.FindAll(component => component != null && component.ComponentCategory == category);
    public void UnlockComponent(SpellComponent component)
    {
        if (component == null || unlockedSpellComponents.Contains(component)) return;
        if (!allSpellComponents.Contains(component))
        {
            Debug.LogWarning($"Trying to unlock component '{component.ComponentName}' that is not in the database.");
            return;
        }
        unlockedSpellComponents.Add(component);
    }
    [ContextMenu("Refresh All Components (Resources)")]
    public void RefreshAllComponents()
    {
        const string resourcesPath = "Spells/SpellComponents"; // relative to Assets/Resources
        SpellComponent[] loaded = Resources.LoadAll<SpellComponent>(resourcesPath);

        allSpellComponents.Clear();
        allSpellComponents.AddRange(loaded);

        // Optional: stable ordering
        allSpellComponents.Sort((a, b) => string.Compare(a.name, b.name, StringComparison.Ordinal));

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
#endif
    }
}
