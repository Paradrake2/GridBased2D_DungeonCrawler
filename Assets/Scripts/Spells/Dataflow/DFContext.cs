using UnityEngine;

public sealed class DFContext
{
    // Data available to sensors during evaluation.
    // Keep this small/explicit so node behavior is deterministic and testable.
    public Player caster;
    public Enemy target;

    // Optional helpers so effectors can write results without reaching into singletons.
    // If null, evaluator may fall back to StatDatabase.Instance when available.
    public StatType damageStatType;

    // When true, evaluator emits diagnostic logs.
    public bool verbose;

    public DFContext(Player caster, Enemy target)
    {
        this.caster = caster;
        this.target = target;
    }
}
