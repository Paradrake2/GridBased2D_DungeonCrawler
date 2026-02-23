using UnityEngine;

[CreateAssetMenu(fileName = "DF_GateNumber", menuName = "Spells/Dataflow/Router/Gate Number")]
public class DF_GateNumberComponent : SpellComponent
{
    [Header("Inputs")]
    public global::Directions inputCondition = global::Directions.Up;
    public global::Directions inputValue = global::Directions.Left;
    public static void Evaluate(DFGridRuntime runtime, DFNodeInstance node, DF_GateNumberComponent component)
    {
        // Reads a boolean condition and a number. If condition is true, outputs the number; if false, outputs nothing.
        bool condition = false;
        float value = 0f;
        bool hasCondition = DFEvaluator.TryReadInput(runtime, node, component.inputCondition, out var sigCond) && sigCond.TryGetBool(out condition);
        bool hasValue = DFEvaluator.TryReadInput(runtime, node, component.inputValue, out var sigVal) && sigVal.TryGetNumber(out value);

        if (!hasCondition || !hasValue)
        {
            DFEvaluator.WriteOutputsToAllActiveDirections(node, DFSignal.None);
            return;
        }

        DFEvaluator.WriteOutputsToAllActiveDirections(node, condition ? DFSignal.FromNumber(value) : DFSignal.None);
    }
}
