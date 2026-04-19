using UnityEngine;

[CreateAssetMenu(fileName = "DF_ConstAttribute", menuName = "Spells/Dataflow/Value/Constant Attribute")]
public class DF_ConstantAttributeComponent : SpellComponent, IDFComponentEvaluator
{
    public StatType attribute;
    public DFEvalTiming Timing => DFEvalTiming.EveryPass;
    public void SetAttribute(StatType newAttribute)
    {
        attribute = newAttribute;
        ChangeIcon(newAttribute.icon, newAttribute);
    }

    public void Evaluate(DFGridRuntime runtime, DFNodeInstance node, DFContext context, DFEvaluationResult result, int pass, bool isFinalPass)
    {
        StatType attr = attribute;
        if (attr == null && AttributeManager.instance != null && AttributeManager.instance.allAttributePairs.Count > 0)
            attr = AttributeManager.instance.allAttributePairs[0].GetDamageAttribute();

        DFEvaluator.WriteOutputsToAllActiveDirections(node, DFSignal.FromAttribute(attr));
    }
}
