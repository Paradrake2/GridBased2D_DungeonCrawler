using UnityEngine;

[CreateAssetMenu(fileName = "DF_EqualsAttribute", menuName = "Spells/Dataflow/Operator/Equals Attribute")]
public class DF_EqualsAttributeComponent : SpellComponent, IDFComponentEvaluator
{
    [Header("Input")]
    public global::Directions inputAttribute = global::Directions.Left;
    public global::Directions compareAttribute = global::Directions.Right;
    [Header("Compare Fallback")]
    public StatType compareTo;
    public DFEvalTiming Timing => DFEvalTiming.EveryPass;
    public static void Evaluate(DFGridRuntime runtime, DFNodeInstance node, DF_EqualsAttributeComponent component)
    {
        StatType inputAttr = null;
        StatType compareAttr = null;

        bool hasInput =
            DFEvaluator.TryReadInput(runtime, node, component.inputAttribute, out var sigInput) &&
            sigInput.TryGetAttribute(out inputAttr);

        bool hasCompareInput =
            DFEvaluator.TryReadInput(runtime, node, component.compareAttribute, out var sigCompare) &&
            sigCompare.TryGetAttribute(out compareAttr);

        if (!hasCompareInput)
            compareAttr = component.compareTo;

        bool match = hasInput && inputAttr != null && compareAttr != null && inputAttr == compareAttr;

        DFEvaluator.WriteOutputsToAllActiveDirections(node, DFSignal.FromBool(match));
    }

    public void Evaluate(DFGridRuntime runtime, DFNodeInstance node, DFContext context, DFEvaluationResult result, int pass, bool isFinalPass)
    {
        Evaluate(runtime, node, this);
    }
}
