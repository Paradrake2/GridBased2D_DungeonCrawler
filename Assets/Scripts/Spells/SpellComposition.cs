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
    public List<PlacedSpellComponent> placedComponents;
    public List<SpellCompositionRequirements> requirements;
    public string spellName;
    public float magicRequired;
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
        placedComponents = new List<PlacedSpellComponent>();
        requirements = new List<SpellCompositionRequirements>();
    }
    public void AddComponent(SpellComponent component)
    {
        AddComponent(component, -1, -1);
    }

    public void AddComponent(SpellComponent component, int x, int y)
    {
        components.Add(component);
        // Only track grid placement when a valid cell coordinate is provided.
        // (Older code paths used AddComponent(component) without coordinates.)
        if (x >= 0 && y >= 0)
            placedComponents.Add(new PlacedSpellComponent(component, x, y));
    }
    public float CalculateSpellCost()
    {
        float cost = 0f;
        foreach (var component in components)
        {
            cost += component.MagicCost;
        }
        return cost;
    }
    public bool MeetsRequirements()
    {
        foreach (var req in requirements) // for templates
        {
            int count = components.FindAll(c => c.ComponentType == req.requiredType).Count;
            if (count < req.minimumCount)
            {
                Debug.Log("Requirement not met: " + req.requiredType + " count is " + count + ", but minimum required is " + req.minimumCount);
                return false;
            }
        }
        int numOfCores = components.FindAll(c => c.ComponentType == SpellComponentType.Core).Count;
        if (numOfCores != 1)
        {
            Debug.Log("Invalid number of Core components. Found: " + numOfCores + ", but exactly 1 is required.");
            return false;
        }
        // all components must have neighbors, no floating components allowed
        foreach (var component in components)
        {
            if (component.NeighboringComponents == null || component.NeighboringComponents.Count == 0)
            {
                Debug.Log("Component " + component.ComponentName + " has no neighboring components. All components must be adjacent to at least one other component.");
                return false;
            }
        }

        if (placedComponents == null || placedComponents.Count == 0)
        {
            Debug.Log("No components placed on the grid.");
            return false;
        }

        int numOfCostComponents = components.FindAll(c => c.ComponentType == SpellComponentType.Cost).Count;
        float coreValue = components.Find(c => c.ComponentType == SpellComponentType.Core).Tier;
        if (numOfCostComponents > coreValue){
            Debug.Log("Too many Cost components for the selected Core. Cost components: " + numOfCostComponents + ", Core value: " + coreValue);
            return false;
        }
        if (GameObject.FindFirstObjectByType<Player>().GetMagic() < CalculateSpellCost())
        {
            Debug.Log("Not enough magic power to cast this spell. Required: " + CalculateSpellCost() + ", Player Magic: " + GameObject.FindFirstObjectByType<Player>().GetMagic());
            return false;
        }
        return true;
    }
    public void RemoveComponent(SpellComponent component)
    {
        components.Remove(component);
        if (placedComponents != null)
            placedComponents.RemoveAll(p => p != null && p.component == component);
    }

    public bool TryGetPlacedAt(int x, int y, out PlacedSpellComponent placed)
    {
        placed = null;
        if (placedComponents == null) return false;
        placed = placedComponents.Find(p => p != null && p.x == x && p.y == y);
        return placed != null;
    }
}

[System.Serializable]
public class PlacedSpellComponent
{
    public SpellComponent component;
    public int x;
    public int y;

    public PlacedSpellComponent(SpellComponent component, int x, int y)
    {
        this.component = component;
        this.x = x;
        this.y = y;
    }
}
