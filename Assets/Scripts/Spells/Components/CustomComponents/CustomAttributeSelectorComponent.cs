using UnityEngine;

public enum CustomAttributeSelectorAction
{
    GetWeakness,
    GetDefense,
}

public class CustomAttributeSelectorComponent : CustomSpellComponent
{
    public SpellAttribute attributeToCheck;
    public CustomAttributeSelectorAction action;
    public override void CustomEffect()
    {
        switch (action)
        {
            case CustomAttributeSelectorAction.GetWeakness:
                // logic to get weakness multiplier for the specified attribute and store it in value1
                break;
            case CustomAttributeSelectorAction.GetDefense:
                // logic to get enemy defense for the specified attribute and store it in value1
                break;
        }
    }
    public override void SetAction()
    {
        // get action
        
    }
}
