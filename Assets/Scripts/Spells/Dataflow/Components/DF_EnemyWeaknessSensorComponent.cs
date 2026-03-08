using UnityEngine;

[CreateAssetMenu(fileName = "DF_EnemyWeakness", menuName = "Spells/Dataflow/Sensor/Enemy Weakness")]
public class DF_EnemyWeaknessSensorComponent : SpellComponent, IDFComponentEvaluator
{
    // Evaluated by DFEvaluator.
    public DFEvalTiming Timing => DFEvalTiming.EveryPass;
    public static void Evaluate(DFGridRuntime runtime, DFNodeInstance node, DFContext context)
    {
        StatType weakness = null;
        if (context != null && context.target != null && context.target.stats != null && context.target.stats.esh != null)
        {
            weakness = context.target.stats.esh.weakness != null ? context.target.stats.esh.weakness.attribute : null;
        }

        // Emits the enemy's weakness attribute (e.g., Fire) as an Attribute signal.
        //Debug.Log("Enemy weakness detected: " + (weakness != null ? weakness.displayName : "None"));
        //Debug.Log(context.target.name);
        DFEvaluator.WriteOutputsToAllActiveDirections(node, DFSignal.FromAttribute(weakness));
    }

    public void Evaluate(DFGridRuntime runtime, DFNodeInstance node, DFContext context, DFEvaluationResult result, int pass, bool isFinalPass)
    {
        Evaluate(runtime, node, context);
    }
}
