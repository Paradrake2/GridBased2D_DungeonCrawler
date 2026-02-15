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

    public List<SpellComponent> GetAllComponents() => allSpellComponents;
    public List<SpellComponent> GetUnlockedComponents() => unlockedSpellComponents;

    public List<SpellComponent> GetComponentsByTypeAll(SpellComponentType type)
        => allSpellComponents.FindAll(component => component != null && component.ComponentType == type);

    public List<SpellComponent> GetComponentsByTypeUnlocked(SpellComponentType type)
        => unlockedSpellComponents.FindAll(component => component != null && component.ComponentType == type);

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
