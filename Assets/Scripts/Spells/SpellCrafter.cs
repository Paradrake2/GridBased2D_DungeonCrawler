using System.Collections.Generic;
using UnityEngine;

public class SpellCrafter : MonoBehaviour
{
    public List<SpellComponentCost> spellComponentCosts;
    public void AddSpellComponentCost(SpellComponentCost cost)
    {
        spellComponentCosts.Add(cost);
    }
    public Spell CreateSpell(SpellComposition composition)
    {
        if (!composition.MeetsRequirements())
        {
            Debug.LogError("Spell composition does not meet the required component criteria.");
            return null;
        }
        Inventory inventory = FindAnyObjectByType<Inventory>();
        foreach (var item in spellComponentCosts)
        {
            if (!inventory.HasItem(item.item, item.amount))
            {
                Debug.LogError("Not enough resources to craft the spell.");
                return null;
            }
        }
        foreach (var item in spellComponentCosts)
        {
            inventory.RemoveItem(item.item, item.amount);
        }
        Spell newSpell = ScriptableObject.CreateInstance<Spell>();

        newSpell.name = "Custom Spell";
        return newSpell;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
