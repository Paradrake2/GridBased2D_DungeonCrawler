using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpellBehaviour
{
    // Cooldown formula: baseCooldown + cooldownPerComponent * componentCount
    public static float BaseCooldown = 1.5f;
    public static float CooldownPerComponent = 0.3f;

    [SerializeField] private float duration;
    [SerializeField] private float damageMult;
    [SerializeField] private float healAmount;
    [SerializeField] private float costAmount;
    [SerializeField] private float defenseBoost;
    [SerializeField] private float cooldownDuration;
    private float cooldownEndsAt = -1f;
    [SerializeField] private List<SpellStat> statModifiers;
    [SerializeField] private List<SpellAttributeWithValue> spellAttributes;

    public SpellBehaviour SpellBehaviourConstructor(float duration, float damageMult, float healAmount, float costAmount, float magicCost,
        List<SpellStat> statModifiers, List<SpellAttributeWithValue> spellAttributes)
    {
        this.duration = duration;
        this.damageMult = damageMult;
        this.healAmount = healAmount;
        this.costAmount = costAmount;
        this.statModifiers = statModifiers;
        this.spellAttributes = spellAttributes;
        // magicCost param kept for signature compatibility; cooldown is set via SetCooldown()
        cooldownDuration = BaseCooldown;
        return this;
    }

    public void SetCooldown(float duration)
    {
        cooldownDuration = duration;
    }

    public bool IsOnCooldown()
    {
        return Time.time < cooldownEndsAt;
    }

    public void TriggerCooldown()
    {
        cooldownEndsAt = Time.time + cooldownDuration;
    }

    public float GetCooldownRemaining()
    {
        return Mathf.Max(0f, cooldownEndsAt - Time.time);
    }

    public float GetCooldownDuration() => cooldownDuration;

    public void SetDefenseBoost(float boost) => defenseBoost = boost;
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
    public List<SpellAttributeWithValue> GetSpellAttributes()
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

        if (IsOnCooldown())
        {
            Debug.Log($"Spell is on cooldown. {GetCooldownRemaining():F1}s remaining.");
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

        // Wire attribute components into the temp attribute set
        if (spellAttributes != null && spellAttributes.Count > 0)
        {
            PlayerAttributeSet attrSet = BuildAttributeSet(spellAttributes);
            player.spellManager.SetTempPlayerAttributeSet(attrSet);
        }

        if (defenseBoost > 0f)
            player.spellManager.SetPendingDefenseBoost(defenseBoost);

        if (healAmount > 0f)
        {
            player.Heal(healAmount);
            Debug.Log($"Spell healed player for {healAmount} HP");
        }

        TriggerCooldown();
        Debug.Log("Casting regular spell");
    }

    protected PlayerAttributeSet BuildAttributeSet(List<SpellAttributeWithValue> attrs)
    {
        PlayerAttributeSet set = new PlayerAttributeSet();
        foreach (SpellAttributeWithValue entry in attrs)
        {
            if (entry.attribute == SpellAttribute.None) continue;

            float attrValue = entry.value;

            if (entry.attribute == SpellAttribute.AllAttributeDamage)
            {
                if (AttributeManager.instance != null)
                    foreach (var pair in AttributeManager.instance.allAttributePairs)
                        set.attackAttributes.Add(new PlayerAttackAttributes { attackAttribute = pair.attackAttribute, attackAttributeValue = attrValue });
                continue;
            }

            if (entry.attribute == SpellAttribute.AllAttributeDefense)
            {
                if (AttributeManager.instance != null)
                    foreach (var pair in AttributeManager.instance.allAttributePairs)
                        set.defenseAttributes.Add(new PlayerDefenseAttributes { defenseAttribute = pair.defenseAttribute, defenseAttributeValue = attrValue });
                continue;
            }

            string attrName = entry.attribute.ToString();
            StatType statType = StatDatabase.Instance != null ? StatDatabase.Instance.GetStat(attrName) : null;
            if (statType == null)
            {
                Debug.LogWarning($"SpellBehaviour: Could not find StatType for SpellAttribute '{attrName}'.");
                continue;
            }

            if (AttributeManager.instance != null && AttributeManager.instance.IsAttackAttribute(statType))
                set.attackAttributes.Add(new PlayerAttackAttributes { attackAttribute = statType, attackAttributeValue = attrValue });
            else
                set.defenseAttributes.Add(new PlayerDefenseAttributes { defenseAttribute = statType, defenseAttributeValue = attrValue });
        }
        return set;
    }
}