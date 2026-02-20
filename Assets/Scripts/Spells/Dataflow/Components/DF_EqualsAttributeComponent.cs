using UnityEngine;

[CreateAssetMenu(fileName = "DF_EqualsAttribute", menuName = "Spells/Dataflow/Operator/Equals Attribute")]
public class DF_EqualsAttributeComponent : SpellComponent
{
    [Header("Input")]
    public global::Directions inputAttribute = global::Directions.Left;
    public global::Directions compareAttribute = global::Directions.Right;
    [Header("Compare Fallback")]
    public StatType compareTo;
}
