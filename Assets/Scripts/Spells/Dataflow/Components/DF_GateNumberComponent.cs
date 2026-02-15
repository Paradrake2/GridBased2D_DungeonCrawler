using UnityEngine;

[CreateAssetMenu(fileName = "DF_GateNumber", menuName = "Spells/Dataflow/Router/Gate Number")]
public class DF_GateNumberComponent : SpellComponent
{
    [Header("Inputs")]
    public global::Directions inputCondition = global::Directions.Up;
    public global::Directions inputValue = global::Directions.Left;
}
