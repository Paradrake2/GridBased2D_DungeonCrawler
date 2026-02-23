using UnityEngine;

[CreateAssetMenu(fileName = "DF_Multiply", menuName = "Spells/Dataflow/Operator/Multiply Numbers")]
public class DF_MultiplyNumbersComponent : SpellComponent
{
    [Header("Inputs")]
    public global::Directions inputA = global::Directions.Left;
    public global::Directions inputB = global::Directions.Right;
    public static void Evaluate(DFGridRuntime runtime, DFNodeInstance node, DF_MultiplyNumbersComponent component)
    {
        // Reads two number inputs (configurable directions) and outputs a*b.
        float a = 0f;
        float b = 0f;
        bool hasA = DFEvaluator.TryReadInput(runtime, node, component.inputA, out var sigA) && sigA.TryGetNumber(out a);
        bool hasB = DFEvaluator.TryReadInput(runtime, node, component.inputB, out var sigB) && sigB.TryGetNumber(out b);

        if (!hasA || !hasB)
        {
            DFEvaluator.WriteOutputsToAllActiveDirections(node, DFSignal.None);
            return;
        }

        DFEvaluator.WriteOutputsToAllActiveDirections(node, DFSignal.FromNumber(a * b));
    }
}
