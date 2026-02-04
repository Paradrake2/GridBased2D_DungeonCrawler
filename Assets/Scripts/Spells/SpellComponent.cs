using System.Collections.Generic;
using UnityEngine;

public enum SpellComponentType
{
    Damage, // for damage dealing components
    Healing, // for healing components
    Cost, // for cost related components
    Strength, // how strong the spell is
    Duration, // how long the spell lasts (typically measured in seconds)
    Attribute, // elemental or special attributes of the spell
    Core, // core of spell, can only be one per spell
    Bridge // connects other components
}
public enum SpellAttribute
{
    None,
    FireDamage,
    FireDefense,
    WaterDamage,
    WaterDefense,
    EarthDamage,
    EarthDefense,
    WindDamage,
    WindDefense,
    LightDamage,
    LightDefense,
    DarkDamage,
    DarkDefense
}
public enum Directions
{
    Up,
    Down,
    Left,
    Right
}
[System.Serializable]
public class SpellComponentDirectionPart
{
    public Vector2 direction;
    public Directions directions;
    public bool isActive = true;
}
[System.Serializable]
public class SpellStat
{
    public StatType stat;
    public float modifier;
    public float value;
}
[System.Serializable]
public class SpellComponentDirections
{
    // Four cardinal directions represented as Vector2
    public List<SpellComponentDirectionPart> inputDirections;
    public List<SpellComponentDirectionPart> outputDirections;
}
[System.Serializable]
public class SpellComponentCost
{
    public Item item;
    public int amount;
}
[CreateAssetMenu(fileName = "SpellComponent", menuName = "Spells/SpellComponent")]
public class SpellComponent : ScriptableObject
{
    [SerializeField] private string componentName;
    [SerializeField] protected Sprite icon;
    [SerializeField] protected int tier;
    [SerializeField] protected SpellComponentType componentType;
    [SerializeField] private float value;
    [SerializeField] protected float cost;
    [SerializeField] protected List<SpellComponent> neighboringComponents; // for determining adjacent components
    [SerializeField] protected List<SpellComponentType> compatibleWith; // for compatibility checks
    [SerializeField] protected SpellComponentDirections directions;
    [SerializeField] protected List<SpellComponentCost> costs;
    [SerializeField] protected List<SpellStat> stats;
    [SerializeField] protected SpellAttribute spellAttributes;
    public string ComponentName => componentName;
    public Sprite Icon => icon;
    public int Tier => tier;
    public SpellComponentType ComponentType => componentType;
    public float Value => value;
    public float Cost => cost;
    public List<SpellComponent> NeighboringComponents => neighboringComponents;
    public List<SpellComponentType> CompatibleWith => compatibleWith;
    public SpellComponentDirections Directions => directions;
    public List<SpellComponentCost> Costs => costs;
    public List<SpellStat> Stats => stats;
    public SpellAttribute SpellAttributes => spellAttributes;
    public bool IsCompatibleWith(SpellComponent other)
    {
        return compatibleWith.Contains(other.ComponentType);
    }
    public void AddAdjacentComponent(SpellComponent component)
    {
        neighboringComponents.Add(component);
    }
    public void RemoveAdjacentComponent(SpellComponent component)
    {
        neighboringComponents.Remove(component);
    }
}
