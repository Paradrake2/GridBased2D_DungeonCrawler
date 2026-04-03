using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataflowSpellBehaviour : SpellBehaviour
{
    // The grid/program for this spell. This is evaluated at cast-time so spells can be fully
    // player-authored without hardcoding every combination.
    [SerializeField] private SpellComposition composition;

    public DataflowSpellBehaviour Initialize(
        SpellComposition composition,
        float duration,
        float damageMult,
        float healAmount,
        float costAmount,
        float magicCost,
        List<SpellStat> statModifiers,
        List<SpellAttribute> spellAttributes)
    {
        // Defensive copy: the crafting UI keeps mutating its SpellComposition as the player edits the grid.
        // Even though SpellCrafter clones at craft time, keeping this here prevents accidental regressions.
        this.composition = DFCompositionUtils.ClonePlacedGrid(composition);
        SpellBehaviourConstructor(duration, damageMult, healAmount, costAmount, magicCost, statModifiers, spellAttributes);
        return this;
    }

    public override void Cast()
    {
        // Keep integration minimal: evaluate the grid, then write results into the same
        // Player fields the existing combat code already reads.
        Player player = GameObject.FindAnyObjectByType<Player>();
        if (player == null)
        {
            Debug.LogWarning("No Player found to apply spell effects.");
            return;
        }

        // check if player is currently in combat

        // Target selection is intentionally simple for MVP.
        Enemy target = GetActiveEnemyTarget();

        var context = new DFContext(player, target)
        {
            damageStatType = StatDatabase.Instance != null ? StatDatabase.Instance.GetStat("Damage") : null,
            verbose = false
        };
        var eval = DFEvaluator.Evaluate(composition, context);
        float cost = eval != null ? eval.cost : 0f;
        if (player.GetMagic() < cost)
        {
            Debug.Log("Not enough magic to cast spell. Required: " + cost + ", Available: " + player.GetMagic());
            return;
        }
        if (eval != null)
        {
            // If evaluator couldn't resolve StatType wiring, still try to map flatDamage into the game's Damage stat.
            //if (context.damageStatType != null && eval.GetFlatDamage() != 0f && !eval.spellStats.HasStat(context.damageStatType))
              //  eval.spellStats.SetStat(context.damageStatType, eval.GetFlatDamage());

            player.spellManager.SetSpellStats(eval.spellStats);
            player.spellManager.SetTempPlayerAttributeSet(eval.tempAttributeSet);
            player.spellManager.SetPendingSpellFlatDamage(eval.GetFlatDamage());

            if (eval.healAmount > 0f)
            {
                player.Heal(eval.healAmount);
                Debug.Log($"Spell healed player for {eval.healAmount} HP");
            }
        }

        // Let combat know what spell behaviour is currently active (used for damage multiplier etc.).
        player.spellManager.SetSpellBehaviour(this);
        Debug.Log("Casting dataflow spell");
    }
    private void InCombatCast()
    {
        
    }
    private Enemy GetActiveEnemyTarget()
    {
        PlayerCombat combat = GameObject.FindAnyObjectByType<PlayerCombat>();
        if (combat != null && combat.targeted != null)        {
            return combat.targeted;
        }

        return null;
    }
}
