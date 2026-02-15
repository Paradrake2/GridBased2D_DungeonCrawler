using UnityEngine;

[CreateAssetMenu(fileName = "DF_SetAttackAttribute", menuName = "Spells/Dataflow/Effector/Set Attack Attribute")]
public class DF_SetAttackAttributeComponent : SpellComponent
{
    [Header("Inputs")]
    public global::Directions inputAttribute = global::Directions.Left;
    public global::Directions inputValue = global::Directions.Right;
}
