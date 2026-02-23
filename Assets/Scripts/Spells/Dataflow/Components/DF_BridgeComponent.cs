using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bridge", menuName = "Spells/Dataflow/Bridge Component")]
public class DF_BridgeComponent : SpellComponent
{
    // these fields are legacy, not actually used by DFEvaluator. They just get set by the UI and can be read by custom evaluators if needed.
    public StatType attribute;
    public float _value;
    public bool boolValue;
    public static void Evaluate(DFGridRuntime runtime, DFNodeInstance node, DF_BridgeComponent component)
    {
        // Pass-through: read a single incoming signal and replicate it to all active outputs.
        // Input/output directions are fully defined by the component's active ports.
        if (runtime == null || node == null || component == null)
            return;

        DFSignal signal = DFSignal.None;

        bool hasSignal = DFEvaluator.TryReadAnyActiveInput(runtime, node, out signal);

        DFEvaluator.WriteOutputsToAllActiveDirections(node, hasSignal ? signal : DFSignal.None);
    }
}
