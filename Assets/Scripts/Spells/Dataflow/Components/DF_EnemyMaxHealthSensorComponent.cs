using UnityEngine;

public class DF_EnemyMaxHealthSensorComponent : SpellComponent, IDFComponentEvaluator
{
    public DFEvalTiming Timing => DFEvalTiming.EveryPass;

    public void Evaluate(DFGridRuntime runtime, DFNodeInstance node, DFContext context, DFEvaluationResult result, int pass, bool isFinalPass)
    {
        throw new System.NotImplementedException();
    }
}
