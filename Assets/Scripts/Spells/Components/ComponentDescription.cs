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
        desc += UnpackDirectionData(directionData);
        return desc;
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
