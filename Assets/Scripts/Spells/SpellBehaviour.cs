using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpellBehaviour
{
    
    private float duration; // number of seconds the spell lasts
    private float damageMult;
    private float healAmount;
    private float costAmount;
    private List<SpellStat> statModifiers;
    private List<SpellAttribute> spellAttributes;
    public SpellBehaviour SpellBehaviourConstructor(float duration, float damageMult, float healAmount, float costAmount,
        List<SpellStat> statModifiers, List<SpellAttribute> spellAttributes)
    {
        this.duration = duration;
        this.damageMult = damageMult;
        this.healAmount = healAmount;
        this.costAmount = costAmount;
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
        
        // consume mana
        Debug.Log("Casting base spell");
    }
}
