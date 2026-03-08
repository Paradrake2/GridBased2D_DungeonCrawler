using UnityEngine;

[CreateAssetMenu(fileName = "DF_PlayerHealth", menuName = "Spells/Dataflow/Sensor/Player Health")]

public class DF_PlayerHealthSensorComponent : SpellComponent, IDFComponentEvaluator
{
    // Evaluated by DFEvaluator.
    public DFEvalTiming Timing => DFEvalTiming.EveryPass;
    public static void Evaluate(DFNodeInstance node, DFContext context)
    {
        float health = 0f;
        if (context != null && context.caster != null && context.caster.stats != null)
        {
            health = context.caster.GetHealth();
        }

        // Emits the player's current health as a Number signal.
        DFEvaluator.WriteOutputsToAllActiveDirections(node, DFSignal.FromNumber(health));
    }

    public void Evaluate(DFGridRuntime runtime, DFNodeInstance node, DFContext context, DFEvaluationResult result, int pass, bool isFinalPass)
    {
        Evaluate(node, context);
    }
}
