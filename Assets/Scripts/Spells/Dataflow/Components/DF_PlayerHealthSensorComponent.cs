using UnityEngine;

[CreateAssetMenu(fileName = "DF_PlayerHealth", menuName = "Spells/Dataflow/Sensor/Player Health")]

public class DF_PlayerHealthSensorComponent : SpellComponent
{
    // Evaluated by DFEvaluator.
    public static void Evaluate(DFNodeInstance node, DFContext context)
    {
        float health = 0f;
        if (context != null && context.caster != null && context.caster.stats != null)
        {
            health = context.caster.GetHealth();
        }

        // Emits the player's current health as a Number signal.
        DFEvaluator.WriteOutputsToAllActiveDirections(node, DFSignal.FromNumber(health));
    }
}
