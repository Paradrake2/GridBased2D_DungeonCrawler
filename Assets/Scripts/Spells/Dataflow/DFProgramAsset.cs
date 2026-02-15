using UnityEngine;

// Simple asset wrapper so a SpellComposition (including placed grid) can be saved and evaluated
// without needing to run the full crafting UI.
[CreateAssetMenu(fileName = "DF Program", menuName = "Spells/Dataflow/Program")]
public sealed class DFProgramAsset : ScriptableObject
{
    [SerializeField] public SpellComposition composition = new SpellComposition();

    private void OnEnable()
    {
        composition ??= new SpellComposition();
        composition.components ??= new System.Collections.Generic.List<SpellComponent>();
        composition.placedComponents ??= new System.Collections.Generic.List<PlacedSpellComponent>();
        composition.requirements ??= new System.Collections.Generic.List<SpellCompositionRequirements>();
    }
}
