using UnityEngine;

public class SpellBehaviour
{
    public SpellComposition composition;
    public void Initialize(SpellComposition composition)
    {
        this.composition = composition;
    }
    public virtual void Cast()
    {
        Debug.Log("Casting base spell");
    }
}
