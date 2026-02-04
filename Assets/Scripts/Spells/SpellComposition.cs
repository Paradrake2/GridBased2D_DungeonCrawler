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
    public bool IsComplete => MeetsRequirements();
    public bool numOfCoresValid
    {
        get
        {
            int numOfCores = components.FindAll(c => c.ComponentType == SpellComponentType.Core).Count;
            return numOfCores == 1;
        }
    }
    public bool numOfCostComponentsValid
    {
        get
        {
            int numOfCostComponents = components.FindAll(c => c.ComponentType == SpellComponentType.Cost).Count;
            float coreValue = components.Find(c => c.ComponentType == SpellComponentType.Core).Tier;
            return numOfCostComponents <= coreValue;
        }
    }
    public SpellComposition()
    {
        components = new List<SpellComponent>();
        requirements = new List<SpellCompositionRequirements>();
    }
    public void AddComponent(SpellComponent component)
    {
        components.Add(component);
        foreach (var comp in components)
        {
            Debug.Log($"Component in composition: {comp.ComponentName}");
        }
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
        int numOfCores = components.FindAll(c => c.ComponentType == SpellComponentType.Core).Count;
        if (numOfCores != 1)
            return false;
        int numOfCostComponents = components.FindAll(c => c.ComponentType == SpellComponentType.Cost).Count;
        float coreValue = components.Find(c => c.ComponentType == SpellComponentType.Core).Tier;
        if (numOfCostComponents > coreValue)
            return false;
        return true;
    }
    public void RemoveComponent(SpellComponent component)
    {
        components.Remove(component);
    }
}
