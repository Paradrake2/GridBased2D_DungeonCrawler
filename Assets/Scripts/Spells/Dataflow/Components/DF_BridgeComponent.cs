using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bridge", menuName = "Spells/Dataflow/Bridge Component")]
public class DF_BridgeComponent : SpellComponent
{
    // these fields are legacy, not actually used by DFEvaluator. They just get set by the UI and can be read by custom evaluators if needed.
    public StatType attribute;
    public float _value;
    public bool boolValue;
}
