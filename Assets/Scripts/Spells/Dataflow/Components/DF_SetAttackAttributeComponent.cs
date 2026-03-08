using UnityEngine;

[CreateAssetMenu(fileName = "DF_SetAttackAttribute", menuName = "Spells/Dataflow/Effector/Set Attack Attribute")]
public class DF_SetAttackAttributeComponent : SpellComponent, IDFComponentEvaluator
{
    [Header("Inputs")]
    public global::Directions inputAttribute = global::Directions.Left;
    public global::Directions inputValue = global::Directions.Right;
    public DFEvalTiming Timing => DFEvalTiming.FinalPassOnly;
    public static void Evaluate(DFGridRuntime runtime, DFNodeInstance node, DF_SetAttackAttributeComponent component, DFEvaluationResult result, DFContext context)
    {
        if (result == null) return;

        // Effector: consumes an Attribute + a Number and writes it into the temp attack attributes.
        StatType attr = null;
        float v = 0f;
        bool hasAttr = DFEvaluator.TryReadInput(runtime, node, component.inputAttribute, out var sigAttr) && sigAttr.TryGetAttribute(out attr);
        bool hasVal = DFEvaluator.TryReadInput(runtime, node, component.inputValue, out var sigVal) && sigVal.TryGetNumber(out v);

        if (hasAttr && attr != null && hasVal)
        {
            result.tempAttributeSet.AddOrSetAttackAttribute(attr, v);
            if (DFEvaluator.Verbose(context))
                Debug.Log("Set attack attribute: " + attr.displayName + " to " + v);
        }
    }

    public void Evaluate(DFGridRuntime runtime, DFNodeInstance node, DFContext context, DFEvaluationResult result, int pass, bool isFinalPass)
    {
        Evaluate(runtime, node, this, result, context);
    }
}
