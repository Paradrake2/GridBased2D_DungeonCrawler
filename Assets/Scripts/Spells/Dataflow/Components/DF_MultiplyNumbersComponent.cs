using UnityEngine;

[CreateAssetMenu(fileName = "DF_Multiply", menuName = "Spells/Dataflow/Operator/Multiply Numbers")]
public class DF_MultiplyNumbersComponent : SpellComponent
{
    [Header("Inputs")]
    public global::Directions inputA = global::Directions.Left;
    public global::Directions inputB = global::Directions.Right;
}
