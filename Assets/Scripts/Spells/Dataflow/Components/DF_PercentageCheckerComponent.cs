using UnityEngine;

[CreateAssetMenu(fileName = "DF_PercentageChecker", menuName = "Spells/Dataflow/Value/Percentage Checker")]

public class DF_PercentageCheckerComponent : SpellComponent
{
    [Header("Inputs")]
    public global::Directions inputValue = global::Directions.Left;
    public global::Directions inputMaxValue = global::Directions.Right; // Optional max value input; if not provided, defaults to 100.
    public global::Directions inputPercentage = global::Directions.Up;
    public static void Evaluate(DFGridRuntime runtime, DFNodeInstance node, DF_PercentageCheckerComponent component)
    {
        // Reads a number and a percentage. Outputs true if the number is less than or equal to the percentage of some max value (e.g., player's max health), false otherwise.
        float value = 0f;
        float maxValue = 100f;
        float percentage = 0f;
        bool hasValue = DFEvaluator.TryReadInput(runtime, node, component.inputValue, out var sigVal) && sigVal.TryGetNumber(out value);
        bool hasMaxValue = DFEvaluator.TryReadInput(runtime, node, component.inputMaxValue, out var sigMax) && sigMax.TryGetNumber(out maxValue);
        if (!hasMaxValue) maxValue = 100f; // Default max value if
        bool hasPercentage = DFEvaluator.TryReadInput(runtime, node, component.inputPercentage, out var sigPerc) && sigPerc.TryGetNumber(out percentage);

        if (!hasValue || !hasPercentage)
        {
            DFEvaluator.WriteOutputsToAllActiveDirections(node, DFSignal.None);
            return;
        }

        bool result = percentage <= value/ maxValue;

        DFEvaluator.WriteOutputsToAllActiveDirections(node, DFSignal.FromBool(result));
    }
}
