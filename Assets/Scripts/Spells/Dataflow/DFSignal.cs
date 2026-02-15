using UnityEngine;

public enum DFSignalType
{
    None,
    Number,
    Bool,
    Attribute
}

[System.Serializable]
public struct DFSignal
{
    // A tiny tagged-union value type for passing data between grid nodes.
    // Keeping this as a struct avoids allocations during evaluation.
    public DFSignalType type;
    public float number;
    public bool boolean;
    public StatType attribute;

    public static DFSignal None => new DFSignal { type = DFSignalType.None };
    public static DFSignal FromNumber(float value) => new DFSignal { type = DFSignalType.Number, number = value };
    public static DFSignal FromBool(bool value) => new DFSignal { type = DFSignalType.Bool, boolean = value };
    public static DFSignal FromAttribute(StatType value) => new DFSignal { type = DFSignalType.Attribute, attribute = value };

    public bool TryGetNumber(out float value)
    {
        value = 0f;
        if (type != DFSignalType.Number) return false;
        value = number;
        return true;
    }

    public bool TryGetBool(out bool value)
    {
        value = false;
        if (type != DFSignalType.Bool) return false;
        value = boolean;
        return true;
    }

    public bool TryGetAttribute(out StatType value)
    {
        value = null;
        if (type != DFSignalType.Attribute) return false;
        value = attribute;
        return true;
    }

    public override string ToString()
    {
        return type switch
        {
            DFSignalType.Number => number.ToString(),
            DFSignalType.Bool => boolean.ToString(),
            DFSignalType.Attribute => attribute != null ? attribute.displayName : "(null)",
            _ => "(none)"
        };
    }
}
