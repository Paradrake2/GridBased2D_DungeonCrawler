using UnityEngine;

[CreateAssetMenu(fileName = "DF_ConstAttribute", menuName = "Spells/Dataflow/Value/Constant Attribute")]
public class DF_ConstantAttributeComponent : SpellComponent
{
    public StatType attribute;
    public void SetAttribute(StatType newAttribute)
    {
        attribute = newAttribute;
        ChangeIcon(newAttribute.icon, newAttribute);
    }
}
