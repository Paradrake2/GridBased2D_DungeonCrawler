using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpellCompositionRequirements
{
    public SpellComponentType requiredType;
    public int minimumCount;
}


[System.Serializable]
public class SpellComposition
{
    public List<SpellComponent> components;
    public List<SpellCompositionRequirements> requirements;
    public SpellComposition()
    {
        components = new List<SpellComponent>();
        requirements = new List<SpellCompositionRequirements>();
    }
    public void AddComponent(SpellComponent component)
    {
        components.Add(component);
    }
    public bool MeetsRequirements()
    {
        foreach (var req in requirements)
        {
            int count = components.FindAll(c => c.ComponentType == req.requiredType).Count;
            if (count < req.minimumCount)
            {
                return false;
            }
        }
        return true;
    }

}
