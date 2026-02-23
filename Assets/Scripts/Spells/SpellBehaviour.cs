using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpellBehaviour
{
    
    [SerializeField] private float duration; // number of seconds the spell lasts
    [SerializeField] private float damageMult;
    [SerializeField] private float healAmount;
    [SerializeField] private float costAmount;
    [SerializeField] private float magicCost;
    [SerializeField] private List<SpellStat> statModifiers;
    [SerializeField] private List<SpellAttribute> spellAttributes;
    public SpellBehaviour SpellBehaviourConstructor(float duration, float damageMult, float healAmount, float costAmount, float magicCost,
        List<SpellStat> statModifiers, List<SpellAttribute> spellAttributes)
    {
        this.duration = duration;
        this.damageMult = damageMult;
        this.healAmount = healAmount;
        this.costAmount = costAmount;
        this.magicCost = magicCost;
        this.statModifiers = statModifiers;
        this.spellAttributes = spellAttributes;
        return this;
    }
    public float GetDamageMult()
    {
        return damageMult;
    }
    public float GetHealAmount()
    {
        return healAmount;
    }
    public float GetDuration()
    {
        return duration;
    }
    public float GetCostAmount()
    {
        return costAmount;
    }
    public List<SpellStat> GetStatModifiers()
    {
        return statModifiers;
    }
    public List<SpellAttribute> GetSpellAttributes()
    {
        return spellAttributes;
    }
    public virtual void Cast()
    {
        // spell attributes mapped to respective stats in another function
        Player player = GameObject.FindAnyObjectByType<Player>();
        // check if enough mana
        // check if enough magic
        // consume mana
        // apply effects
        Debug.Log("Casting base spell");
    }
}
