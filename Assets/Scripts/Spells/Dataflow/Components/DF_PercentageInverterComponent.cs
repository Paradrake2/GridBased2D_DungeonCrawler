using UnityEngine;

public class DF_PercentageInverterComponent : SpellComponent
{
    public global::Directions inputPercentage = global::Directions.Left;
    public static void Evaluate(DFGridRuntime runtime, DFNodeInstance node, DF_PercentageInverterComponent component)
    {
        // Reads a percentage value and outputs its inverse (e.g., if input is 30%, output is 70%).
        float percentage = 0f;
        bool hasPercentage = DFEvaluator.TryReadInput(runtime, node, component.inputPercentage, out var sigPerc) && sigPerc.TryGetNumber(out percentage);

        if (!hasPercentage)
        {
            DFEvaluator.WriteOutputsToAllActiveDirections(node, DFSignal.None);
            return;
        }

        float inverted = 100f - percentage;
        DFEvaluator.WriteOutputsToAllActiveDirections(node, DFSignal.FromNumber(inverted));
    }
}
