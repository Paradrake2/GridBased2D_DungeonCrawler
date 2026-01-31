using UnityEngine;

public class SpellBehaviour
{
    public SpellComposition composition;
    public void Initialize(SpellComposition composition)
    {
        this.composition = composition;
    }
    public void Prepare()
    {
        // Prepare spell based on its composition
        foreach (var component in composition.components)
        {
            Debug.Log($"Preparing component: {component.ComponentName} of type {component.ComponentType} with value {component.Value}");
        }
    }
    public virtual void Cast()
    {
        Debug.Log("Casting base spell");
    }
}
