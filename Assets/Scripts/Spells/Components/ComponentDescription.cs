using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ComponentDescriptionDirection
{
    Up,
    Down,
    Left,
    Right,
    AllSides
}
public class ComponentDescriptionIO
{
    public IOType IOType;
    public ComponentDescriptionDirection direction;
    public string GetDirectionString()
    {
        switch (direction)
        {
            case ComponentDescriptionDirection.Up:
                return "Up";
            case ComponentDescriptionDirection.Down:
                return "Down";
            case ComponentDescriptionDirection.Left:
                return "Left";
            case ComponentDescriptionDirection.Right:
                return "Right";
            case ComponentDescriptionDirection.AllSides:
                return "All Sides";
            default:
                return "";
        }
    }
}


[CreateAssetMenu(fileName = "ComponentDescription", menuName = "Spells/ComponentDescription")]
public class ComponentDescription : ScriptableObject
{
    [SerializeField] private string componentName;
    [SerializeField] private string descriptionText;
    [SerializeField] private bool useValue;
    public string ComponentName => componentName;
    public string DescriptionText => descriptionText;

    public string GetDescription(SpellComponent sc)
    {
        // getters
        Debug.Log("Getting description for " + sc.name);
        SpellComponentDirections directionData = sc.Directions;
        string desc = $"{ComponentName}\n{DescriptionText}\n";
        if (useValue) desc += $"Value: {sc.Value}\n";
        desc += UnpackMagicCost(sc);
        desc += UnpackManaCost(sc);
        desc += UnpackStats(sc);
        desc += UnpackDirectionData(directionData);
        return desc;
    }
    private string UnpackStats(SpellComponent sc)
    {
        if (sc.Stats == null || sc.Stats.Count == 0) return "";
        float strengthMult = 1f;
        if (sc.NeighboringComponents != null)
        {
            foreach (var neighbor in sc.NeighboringComponents)
            {
                if (neighbor == null || neighbor.ComponentType != SpellComponentType.Strength) continue;
                strengthMult *= neighbor.Value;
            }
        }
        string result = "";
        foreach (var stat in sc.Stats)
        {
            if (stat.stat == null) continue;
            float effectiveValue = stat.value * strengthMult;
            result += $"{stat.stat.name}: {effectiveValue}";
            if (strengthMult != 1f)
                result += $" (x{strengthMult} from Strengthen)";
            result += "\n";
        }
        return result;
    }
    private string UnpackMagicCost(SpellComponent sc)
    {
        return $"Magic Cost: {sc.MagicCost}\n";
    }
    private string UnpackManaCost(SpellComponent sc)
    {
        return $"Mana Cost: {sc.Cost}\n";
    }
    private string UnpackDirectionData(SpellComponentDirections directionData)
    {
        string result = "Input/Output:\n";
        foreach (var input in directionData.inputDirections)
        {
            result += $"Input {input.direction.ToString()}: {input.IOType}\n";
        }
        foreach (var output in directionData.outputDirections)
        {
            result += $"Output {output.direction.ToString()}: {output.IOType}\n";
        }
        return result;
    }
}
