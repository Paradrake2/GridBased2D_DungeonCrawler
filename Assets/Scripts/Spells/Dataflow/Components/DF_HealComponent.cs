using UnityEngine;

[CreateAssetMenu(fileName = "DF_Heal", menuName = "Spells/Dataflow/Effector/Heal")]

public class DF_HealComponent : SpellComponent, IDFComponentEvaluator
{
    public global::Directions inputHealAmount = global::Directions.Left; // The amount to heal, expected as flat value.
    public DFEvalTiming Timing => DFEvalTiming.FinalPassOnly;

    public static void Evaluate(DFGridRuntime runtime, DFNodeInstance node, DF_HealComponent component, DFEvaluationResult result)
    {
        float healAmount = DFEvaluator.TryReadInput(runtime, node, component.inputHealAmount, out var sig) && sig.TryGetNumber(out float heal) ? heal : 0;
        if (result != null)
            result.healAmount += healAmount;
        Debug.Log($"Healing for {healAmount} HP");
    }
    public void Evaluate(DFGridRuntime runtime, DFNodeInstance node, DFContext context, DFEvaluationResult result, int pass, bool isFinalPass)
    {
        Evaluate(runtime, node, this, result);
    }
}
