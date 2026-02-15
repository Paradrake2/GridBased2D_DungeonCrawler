using System.Collections.Generic;
using UnityEngine;

public static class DFCompositionUtils
{
    public static SpellComposition ClonePlacedGrid(SpellComposition source)
    {
        if (source == null) return null;

        SpellComposition clone = new SpellComposition
        {
            spellName = source.spellName,
            magicRequired = source.magicRequired,
            requirements = new List<SpellCompositionRequirements>()
        };

        if (source.requirements != null)
        {
            foreach (var req in source.requirements)
            {
                if (req == null) continue;
                clone.requirements.Add(new SpellCompositionRequirements
                {
                    requiredType = req.requiredType,
                    minimumCount = req.minimumCount
                });
            }
        }

        clone.components = new List<SpellComponent>();
        clone.placedComponents = new List<PlacedSpellComponent>();

        if (source.placedComponents != null)
        {
            foreach (var placed in source.placedComponents)
            {
                if (placed == null || placed.component == null) continue;
                if (placed.x < 0 || placed.y < 0) continue;

                // Ensure each spell snapshot has its own component instances.
                SpellComponent componentClone = Object.Instantiate(placed.component);
                clone.components.Add(componentClone);
                clone.placedComponents.Add(new PlacedSpellComponent(componentClone, placed.x, placed.y));
            }
        }

        return clone;
    }
}
