using UnityEngine;

[CreateAssetMenu(fileName = "CustomComponent", menuName = "Spells/CustomSpellComponent")]

public class CustomSpellComponent : SpellComponent
{
    public bool isTrue; // for logic
    public bool isBuff; // for buffs/debuffs
    public float value1;
    public float value2;
    public float value3;
    public string string1;
    public string string2;
    public string string3;
    public float CalculateCustomCost() // placeholder right now
    {
        return 0;
    }
    public virtual void CustomEffect()
    {
        // implement in subclasses for custom behavior
    }
    public virtual void SetAction()
    {
        
    }
}
