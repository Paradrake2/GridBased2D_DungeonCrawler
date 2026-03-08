using UnityEngine;

[CreateAssetMenu(fileName = "DF_ConstNumber", menuName = "Spells/Dataflow/Value/Constant Number")]
public class DF_ConstantNumberComponent : SpellComponent, IDFComponentEvaluator
{
    public float _value = 1f;
    public DFEvalTiming Timing => DFEvalTiming.EveryPass;
    public void Evaluate(DFGridRuntime runtime, DFNodeInstance node, DFContext context, DFEvaluationResult result, int pass, bool isFinalPass)
    {
        DFEvaluator.WriteOutputsToAllActiveDirections(node, DFSignal.FromNumber(_value));
    }

    public void SetValue(float value)
    {
        _value = value;
    }
}
