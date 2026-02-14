using System.Collections.Generic;
using UnityEngine;

public class SpellCrafter : MonoBehaviour
{
    public List<SpellComponentCost> spellComponentCosts;
    public void AddSpellComponentCost(SpellComponentCost cost)
    {
        spellComponentCosts.Add(cost);
    }
    // Called when actually crafting the spell for the purposes of adding to inventory
    public void AddSpellToInventory(SpellComposition composition, SpellTemplate template = null)
    {
        Inventory inventory = FindAnyObjectByType<Inventory>();
        if (!MeetsAllCriteria(composition))
        {
            Debug.Log("Spell does not meet all criteria for crafting.");
            return;
        }
        Spell spell = CreateSpell(composition, template);
        if (spell != null)
        {
            //inventory.AddItem(spell);
            foreach (var item in spellComponentCosts)
            {
                inventory.RemoveItem(item.item, item.amount);
            }
        }
        inventory.AddSpellToInventory(spell);
    }
    // Create the spell, can be used for previews as well
    public Spell CreateSpell(SpellComposition composition, SpellTemplate template = null)
    {
        Spell newSpell = ScriptableObject.CreateInstance<Spell>();
        SpellComponent coreComponent = composition.components.Find(c => c.ComponentType == SpellComponentType.Core);
        List<SpellComponent> damageComponents = composition.components.FindAll(c => c.ComponentType == SpellComponentType.Damage);
        float damageMult = CalculateDamageMult(damageComponents) * coreComponent.Value;
        List<SpellComponent> healComponents = composition.components.FindAll(c => c.ComponentType == SpellComponentType.Healing);
        float healAmount = CalculateHealAmount(healComponents) * coreComponent.Value;
        List<SpellComponent> durationComponents = composition.components.FindAll(c => c.ComponentType == SpellComponentType.Duration);
        float duration = CalculateDuration(durationComponents);
        List<SpellComponent> costComponents = composition.components.FindAll(c => c.ComponentType == SpellComponentType.Cost);
        List<SpellComponent> strengthComponents = composition.components.FindAll(c => c.ComponentType == SpellComponentType.Strength);
        float costAmount = CalculateCost(coreComponent, costComponents, damageComponents, healComponents, durationComponents, strengthComponents);
        float magicCost = composition.CalculateSpellCost();
        List<SpellStat> statModifiers = new List<SpellStat>();
        List<SpellAttribute> spellAttributes = new List<SpellAttribute>();
        newSpell.spellEffect = new SpellBehaviour().SpellBehaviourConstructor(duration, damageMult, healAmount, costAmount, magicCost,
            statModifiers, spellAttributes);
        newSpell.name = composition.spellName; // will be determined by components or set by player
        newSpell.SetSpellName(composition.spellName);
        // place for special effects based on components
        return newSpell;
    }
    private float CalculateDamageMult(List<SpellComponent> damageComponents = null)
    {
        float totalMult = 1f;
        if (damageComponents != null)
        {
            if (damageComponents.Count == 0 || damageComponents[0].NeighboringComponents == null) return totalMult;
            foreach (var comp in damageComponents)
            {
                totalMult += CalculateStrengthenedValue(comp, comp.NeighboringComponents); // mult by adjacent strength components
            }
        }
        return totalMult;
    }
    private float CalculateHealAmount(List<SpellComponent> healComponents = null)
    {
        float totalHeal = 0f;
        if (healComponents != null)
        {
            if (healComponents.Count == 0 || healComponents[0].NeighboringComponents == null) return totalHeal;
            foreach (var comp in healComponents)
            {
                totalHeal += CalculateStrengthenedValue(comp, comp.NeighboringComponents); // mult by adjacent strength components
            }
        }
        return totalHeal;
    }
    private float CalculateDuration(List<SpellComponent> durationComponents = null)
    {
        float totalDuration = 0f;
        if (durationComponents != null)
        {
            foreach (var comp in durationComponents)
            {
                totalDuration += CalculateStrengthenedValue(comp, comp.NeighboringComponents); // mult by adjacent strength components
            }
        }
        return totalDuration;
    }
    private float CalculateCost(SpellComponent coreComponent, List<SpellComponent> costComponents = null, List<SpellComponent> damageComponents = null,
        List<SpellComponent> healComponents = null, List<SpellComponent> durationComponents = null, List<SpellComponent> strengthComponents = null)
    {
        float totalCost = 0f;
        if (damageComponents != null)
        {
            float damageCostFactor = 1f;
            foreach (var comp in damageComponents)
            {
                totalCost += comp.Cost * damageCostFactor;
                damageCostFactor *= 1.4f;
            }
        }
        if (healComponents != null)
        {
            float healCostFactor = 1f;
            foreach (var comp in healComponents)
            {
                totalCost += comp.Cost * healCostFactor;
                healCostFactor *= 1.2f;
            }
        }
        if (durationComponents != null)
        {
            float durationCostFactor = 1f;
            foreach (var comp in durationComponents)
            {
                totalCost += comp.Cost * durationCostFactor;
                durationCostFactor *= 2.5f;
            }
        }
        if (strengthComponents != null)
        {
            float strengthCostFactor = 1f;
            foreach (var comp in strengthComponents)
            {
                totalCost += comp.Cost * strengthCostFactor;
                strengthCostFactor *= 2.3f;
            }
        }
        if (costComponents != null)
        {
            foreach (var comp in costComponents)
            {
                totalCost += comp.Cost;
            }
            foreach (var comp in costComponents)
            {
                totalCost *= comp.Value; // cost components are not strengthened
            }
        }
        return totalCost;
    }
    private float CalculateStrengthenedValue(SpellComponent component, List<SpellComponent> adjacentComponents)
    {
        float strengthenedValue = component.Value;
        if (adjacentComponents.Count != 0)
        {
            foreach (var adjacent in adjacentComponents)
            {
                if (adjacent == null || adjacent.ComponentType != SpellComponentType.Strength) continue;
                if (adjacent.ComponentType == SpellComponentType.Strength)
                {
                    strengthenedValue *= adjacent.Value; // increase value based on strength component
                }
            }
            
        }
        return strengthenedValue;
    }
    bool HasRequiredItems(List<SpellComponentCost> costs)
    {
        Inventory inventory = FindAnyObjectByType<Inventory>();
        foreach (var cost in costs)
        {
            if (!inventory.HasItem(cost.item, cost.amount))
            {
                return false;
            }
        }
        return true;
    }
    bool MeetsAllCriteria(SpellComposition composition)
    {
        return composition.MeetsRequirements() && HasEnoughMagicPower(composition.CalculateSpellCost()) && HasRequiredItems(spellComponentCosts);
    }
    bool HasEnoughMagicPower(float cost)
    {
        Player player = GameObject.FindAnyObjectByType<Player>();
        return player.GetMagic() >= cost;
    }
    private string GenerateSpellName(SpellComposition composition) // auto generation, player will have option to custom name the spell
    {
        // placeholder name generation based on components, can be expanded with templates and attributes
        // check attributes, if single attribute that is in name, if multiple say "multi-attribute"
        // check if damage, healing, debuff, if single dominant one put that in name, if multiple say "multi-effect"
        string name = "";
        foreach (var comp in composition.components)
        {
            name += comp.ComponentName + " ";
        }
        return name.Trim();
    }
}
