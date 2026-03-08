using UnityEngine;

public class DF_HealComponent : SpellComponent, IDFComponentEvaluator
{
    public global::Directions inputHealAmount = global::Directions.Left; // The amount to heal, expected as flat value.
    public DFEvalTiming Timing => DFEvalTiming.FinalPassOnly;

    public static void Evaluate(DFGridRuntime runtime, DFNodeInstance node, DF_HealComponent component)
    {
        // This component would read the amount to heal and apply it to the target. For now, we'll just throw a NotImplementedException.
        throw new System.NotImplementedException();
    }
    public void Evaluate(DFGridRuntime runtime, DFNodeInstance node, DFContext context, DFEvaluationResult result, int pass, bool isFinalPass)
    {
        Evaluate(runtime, node, this);
    }
}
