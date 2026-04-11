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
        Player player = GameObject.FindAnyObjectByType<Player>();
        if (player == null)
        {
            Debug.LogWarning("No Player found to apply spell effects.");
            return;
        }

        if (player.GetMagic() < magicCost)
        {
            Debug.Log($"Not enough magic to cast spell. Required: {magicCost}, Available: {player.GetMagic()}");
            return;
        }

        // Build a StatCollection from statModifiers
        StatCollection spellStatCollection = new StatCollection();
        if (statModifiers != null)
        {
            foreach (SpellStat spellStat in statModifiers)
            {
                if (spellStat.stat == null) continue;

                float effectiveValue = spellStat.value;
                if (spellStat.modifier != 0f)
                    effectiveValue += player.statCol.GetStat(spellStat.stat) * spellStat.modifier;

                spellStatCollection.SetStat(spellStat.stat, effectiveValue);
            }
        }

        player.spellManager.SetSpellStats(spellStatCollection);
        player.spellManager.SetSpellBehaviour(this);

        if (healAmount > 0f)
        {
            player.Heal(healAmount);
            Debug.Log($"Spell healed player for {healAmount} HP");
        }

        Debug.Log("Casting regular spell");
    }
}
