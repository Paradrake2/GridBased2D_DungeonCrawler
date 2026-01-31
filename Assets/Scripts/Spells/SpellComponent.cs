using System.Collections.Generic;
using UnityEngine;

public enum SpellComponentType
{
    Damage,
    Healing,
    Cost,
    Strength,
    Duration,
    Attribute,
    Core
}

[System.Serializable]
public class SpellComponentDirections
{
    // Four cardinal directions represented as Vector2
    public List<Vector2> inputDirections;
    public List<Vector2> outputDirections;
}

[CreateAssetMenu(fileName = "SpellComponent", menuName = "Spells/SpellComponent")]
public class SpellComponent : ScriptableObject
{
    [SerializeField] private string componentName;
    [SerializeField] protected Sprite icon;
    [SerializeField] protected SpellComponentType componentType;
    [SerializeField] protected float value;
    [SerializeField] protected List<SpellComponent> neighboringComponents; // for determining adjacent components
    [SerializeField] protected List<SpellComponentType> compatibleWith; // for compatibility checks
    [SerializeField] protected SpellComponentDirections directions;

    public string ComponentName => componentName;
    public Sprite Icon => icon;
    public SpellComponentType ComponentType => componentType;
    public float Value => value;
    public List<SpellComponent> NeighboringComponents => neighboringComponents;
    public List<SpellComponentType> CompatibleWith => compatibleWith;
    public SpellComponentDirections Directions => directions;

}
