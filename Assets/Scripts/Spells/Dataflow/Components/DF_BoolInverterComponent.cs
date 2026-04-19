using UnityEngine;

[CreateAssetMenu(fileName = "DF_BoolInverter", menuName = "Spells/Dataflow/Operator/Bool Inverter")]
public class DF_BoolInverterComponent : SpellComponent, IDFComponentEvaluator
{
    [Header("Input")]
    public global::Directions inputBool = global::Directions.Left;

    public DFEvalTiming Timing => DFEvalTiming.EveryPass;

    public static void Evaluate(DFGridRuntime runtime, DFNodeInstance node, DF_BoolInverterComponent component)
    {
        bool value = false;
        bool hasInput = DFEvaluator.TryReadInput(runtime, node, component.inputBool, out var sig) && sig.TryGetBool(out value);

        if (!hasInput)
        {
            DFEvaluator.WriteOutputsToAllActiveDirections(node, DFSignal.None);
            return;
        }

        DFEvaluator.WriteOutputsToAllActiveDirections(node, DFSignal.FromBool(!value));
    }

    public void Evaluate(DFGridRuntime runtime, DFNodeInstance node, DFContext context, DFEvaluationResult result, int pass, bool isFinalPass)
    {
        Evaluate(runtime, node, this);
    }
}
