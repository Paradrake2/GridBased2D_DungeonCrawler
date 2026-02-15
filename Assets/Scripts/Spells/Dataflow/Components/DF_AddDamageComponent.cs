using UnityEngine;

[CreateAssetMenu(fileName = "DF_AddDamage", menuName = "Spells/Dataflow/Effector/Add Damage")]
public class DF_AddDamageComponent : SpellComponent
{
    [Header("Input")]
    public global::Directions inputDamage = global::Directions.Left;
}
