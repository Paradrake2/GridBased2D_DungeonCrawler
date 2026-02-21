using UnityEngine;

[CreateAssetMenu(fileName = "DF_ConstNumber", menuName = "Spells/Dataflow/Value/Constant Number")]
public class DF_ConstantNumberComponent : SpellComponent
{
    public float _value = 1f;
    public void SetValue(float value)
    {
        _value = value;
    }
}
