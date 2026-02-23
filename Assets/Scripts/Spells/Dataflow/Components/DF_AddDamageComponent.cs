using UnityEngine;

[CreateAssetMenu(fileName = "DF_AddDamage", menuName = "Spells/Dataflow/Effector/Add Damage")]
public class DF_AddDamageComponent : SpellComponent
{
    [Header("Input")]
    public global::Directions inputDamage = global::Directions.Left;
    [SerializeField] private float costMult = 2.5f;
    public float GetCost(float inputDamage, float multiplier)
    {
        return inputDamage * costMult * multiplier;
    }
    public static void Evaluate(DFGridRuntime runtime, DFNodeInstance node, DF_AddDamageComponent component, DFEvaluationResult result, DFContext context)
    {
        Debug.Log("Evaluating AddDamageComponent at " + node.Position);
        if (result == null) return;
        // Effector: consumes a Number signal and accumulates it into the "Damage" stat.
        if (DFEvaluator.TryReadInput(runtime, node, component.inputDamage, out var sig) && sig.TryGetNumber(out float dmg))
        {
            //result.flatDamage += dmg;
            //result.spellStats.SetStat(StatDatabase.Instance.GetStat("Damage"), result.spellStats.GetStat(StatDatabase.Instance.GetStat("Damage")) + dmg);
            StatType damageStat = context != null ? context.damageStatType : null;
            if (damageStat == null)
            {
                Debug.LogWarning("Damage stat type not found in context; AddDamageComponent will write to flatDamage but not into spellStats.");
                var db = StatDatabase.Instance;
                damageStat = db != null ? db.GetStat("Damage") : null;
            }

            if (damageStat != null)
            {
                float current = result.spellStats.GetStat(damageStat);
                result.spellStats.SetStat(damageStat, current + dmg);
            }

            if (DFEvaluator.Verbose(context))
                Debug.Log("AddDamage at " + node.Position + " read " + dmg + " (flatDamage now " + result.flatDamage + ")");
            
            result.cost += component.GetCost(dmg, multiplier: 1f);
        }
    }
}
