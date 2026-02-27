using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class DFEvaluationResult
{
    public StatCollection spellStats = new StatCollection();

    // Convenience output so effectors can be tested without needing StatDatabase/StatType wiring. <-- IOC violation
    // dataflowSpellBehaviour will still map this into the game's StatType-based stats when possible. <-- IOC violation but keeps things simpler for now, remember to fix
    public float flatDamage;
    public float cost;

    // temporary attack attributes (element type + magnitude) used by weakness/defense logic.
    public PlayerAttributeSet tempAttributeSet = new PlayerAttributeSet();
}

public static class DFEvaluator
{
    public const int DefaultMaxPasses = 8; // DO NOT CHANGE: WILL CAUSE INFINITE LOOPS


    // Used only for optional verbose diagnostics during a single Evaluate() call.
    public static bool Verbose(DFContext context) => context != null && context.verbose;
    // Unity runs this on the main thread
    private static DFContext currentContext;

    private static Directions GetPortDirection(SpellComponentDirectionPart part)
    {
        if (part == null) return Directions.Up;

        if (part.direction != Vector2.zero)
        {
            Vector2 d = part.direction;
            if (Mathf.Abs(d.x) >= Mathf.Abs(d.y))
                return d.x >= 0f ? Directions.Right : Directions.Left;
            return d.y >= 0f ? Directions.Up : Directions.Down;
        }

        return part.directions;
    }

    public static DFEvaluationResult Evaluate(SpellComposition composition, DFContext context, int maxPasses = DefaultMaxPasses)
    {
        var result = new DFEvaluationResult();
        if (composition == null || composition.placedComponents == null || composition.placedComponents.Count == 0)
            return result;

        currentContext = context;
        try
        {
            // Compile the placed grid (x,y -> node) into a runtime structure for fast neighbor lookups.
            DFGridRuntime runtime = Compile(composition);
            if (!runtime.Nodes.Any())
                return result;

            // Outputs may depend on previous passes (simple feedback/propagation). Keep bounded so it
            // never hangs even if the grid forms cycles.
            for (int pass = 0; pass < maxPasses; pass++)
            {
                bool isFinalPass = pass == maxPasses - 1;
                foreach (var node in runtime.Nodes.OrderBy(n => n.Position.y).ThenBy(n => n.Position.x))
                {
                    if (node.Component == null) continue;
                    // Sensors
                    if (node.Component is DF_EnemyWeaknessSensorComponent)
                    {
                        EvaluateEnemyWeaknessSensor(runtime, node, context);
                        continue;
                    }
                    if (node.Component is DF_BridgeComponent bridge)
                    {
                        EvaluateBridge(runtime, node, bridge);
                        continue;
                    }
                    if (node.Component is DF_ConstantNumberComponent constNumber)
                    {
                        EvaluateConstantNumber(node, constNumber);
                        continue;
                    }
                    if (node.Component is DF_ConstantAttributeComponent constAttr)
                    {
                        EvaluateConstantAttribute(node, constAttr);
                        continue;
                    }

                    // Operators
                    if (node.Component is DF_MultiplyNumbersComponent multiply)
                    {
                        EvaluateMultiply(runtime, node, multiply);
                        continue;
                    }
                    if (node.Component is DF_EqualsAttributeComponent eqAttr)
                    {
                        EvaluateEqualsAttribute(runtime, node, eqAttr);
                        continue;
                    }
                    if (node.Component is DF_GateNumberComponent gateNum)
                    {
                        EvaluateGateNumber(runtime, node, gateNum);
                        continue;
                    }

                    // Effectors
                    if (isFinalPass)
                    {
                        if (node.Component is DF_AddDamageComponent addDmg)
                        {
                            EvaluateAddDamage(runtime, node, addDmg, result, context);
                            continue;
                        }
                        if (node.Component is DF_SetAttackAttributeComponent setAttr)
                        {
                            EvaluateSetAttackAttribute(runtime, node, setAttr, result, context);
                            continue;
                        }
                    }
                }
            }

            return result;
        }
        finally
        {
            currentContext = null;
        }
    }

    private static DFGridRuntime Compile(SpellComposition composition)
    {
        DFGridRuntime runtime = new DFGridRuntime();

        foreach (var placed in composition.placedComponents)
        {
            if (placed == null || placed.component == null) continue;
            if (placed.x < 0 || placed.y < 0) continue;

            runtime.AddNode(new DFNodeInstance(new Vector2Int(placed.x, placed.y), placed.component));
        }

        return runtime;
    }

    private static bool HasActiveInput(SpellComponent component, Directions inputDir)
    {
        if (component == null || component.Directions == null || component.Directions.inputDirections == null) return false;
        return component.Directions.inputDirections.Any(p => p != null && p.isActive && GetPortDirection(p) == inputDir);
    }

    private static bool HasActiveOutput(SpellComponent component, Directions outputDir)
    {
        if (component == null || component.Directions == null || component.Directions.outputDirections == null) return false;
        return component.Directions.outputDirections.Any(p => p != null && p.isActive && GetPortDirection(p) == outputDir);
    }

    public static bool TryReadInput(DFGridRuntime runtime, DFNodeInstance node, Directions inputDir, out DFSignal signal)
    {
        signal = DFSignal.None;

        // Node must declare it accepts input from this direction.
        if (!HasActiveInput(node.Component, inputDir))
        {
            if (Verbose(currentContext))
                Debug.Log($"[DF] Input blocked: {node.Component?.GetType().Name} at {node.Position} has no active input {inputDir}");
            return false;
        }

        // "inputDir" is the direction *the signal comes from* relative to this node.
        // Example: if I want input from Left, the neighbor is at (x-1,y) and must output Right.
        Vector2Int fromPos = node.Position + DFPortMap.ToOffset(inputDir);
        if (!runtime.TryGetNodeAt(fromPos, out var fromNode))
        {
            if (Verbose(currentContext))
                Debug.Log($"[DF] Input missing: {node.Component?.GetType().Name} at {node.Position} expects neighbor at {fromPos} (from {inputDir})");
            return false;
        }

        Directions fromOutputDir = DFPortMap.Opposite(inputDir);
        if (!HasActiveOutput(fromNode.Component, fromOutputDir))
        {
            if (Verbose(currentContext))
                Debug.Log($"[DF] Input blocked: neighbor {fromNode.Component?.GetType().Name} at {fromPos} has no active output {fromOutputDir}");
            return false;
        }

        signal = fromNode.GetOutput(fromOutputDir);
        bool ok = signal.type != DFSignalType.None;
        if (!ok && Verbose(currentContext))
            Debug.Log($"[DF] Input empty: neighbor {fromNode.Component?.GetType().Name} at {fromPos} output {fromOutputDir} is None");
        return ok;
    }

    public static void WriteOutputsToAllActiveDirections(DFNodeInstance node, DFSignal signal)
    {
        if (node.Component == null || node.Component.Directions == null || node.Component.Directions.outputDirections == null) return;

        foreach (var outDir in node.Component.Directions.outputDirections)
        {
            if (outDir == null || !outDir.isActive) continue;
            node.SetOutput(GetPortDirection(outDir), signal);
        }
    }

    private static void EvaluateEnemyWeaknessSensor(DFGridRuntime runtime, DFNodeInstance node, DFContext context)
    {
        DF_EnemyWeaknessSensorComponent.Evaluate(runtime, node, context);
    }

    private static void EvaluateConstantNumber(DFNodeInstance node, DF_ConstantNumberComponent component)
    {
        WriteOutputsToAllActiveDirections(node, DFSignal.FromNumber(component._value));
    }

    private static void EvaluateConstantAttribute(DFNodeInstance node, DF_ConstantAttributeComponent component)
    {
        WriteOutputsToAllActiveDirections(node, DFSignal.FromAttribute(component.attribute));
    }

    private static void EvaluateBridge(DFGridRuntime runtime, DFNodeInstance node, DF_BridgeComponent component)
    {
        DF_BridgeComponent.Evaluate(runtime, node, component);
    }

    public static bool TryReadAnyActiveInput(DFGridRuntime runtime, DFNodeInstance node, out DFSignal signal)
    {
        signal = DFSignal.None;
        if (runtime == null || node == null || node.Component == null) return false;
        if (node.Component.Directions == null || node.Component.Directions.inputDirections == null) return false;

        foreach (var inDir in node.Component.Directions.inputDirections)
        {
            if (inDir == null || !inDir.isActive) continue;
            var dir = GetPortDirection(inDir);
            if (TryReadInput(runtime, node, dir, out signal))
                return true;
        }

        return false;
    }

    private static void EvaluateMultiply(DFGridRuntime runtime, DFNodeInstance node, DF_MultiplyNumbersComponent component)
    {
        DF_MultiplyNumbersComponent.Evaluate(runtime, node, component);
    }

    private static void EvaluateEqualsAttribute(DFGridRuntime runtime, DFNodeInstance node, DF_EqualsAttributeComponent component)
    {
        DF_EqualsAttributeComponent.Evaluate(runtime, node, component);
    }

    private static void EvaluateGateNumber(DFGridRuntime runtime, DFNodeInstance node, DF_GateNumberComponent component)
    {
        DF_GateNumberComponent.Evaluate(runtime, node, component);
    }

    private static void EvaluateAddDamage(DFGridRuntime runtime, DFNodeInstance node, DF_AddDamageComponent component, DFEvaluationResult result, DFContext context)
    {

        DF_AddDamageComponent.Evaluate(runtime, node, component, result, context);
        /**
        if (result == null) return;
        // Effector: consumes a Number signal and accumulates it into the "Damage" stat.
        if (TryReadInput(runtime, node, component.inputDamage, out var sig) && sig.TryGetNumber(out float dmg))
        {
            //result.flatDamage += dmg;
            result.spellStats.SetStat(StatDatabase.Instance.GetStat("Damage"), result.spellStats.GetStat(StatDatabase.Instance.GetStat("Damage")) + dmg);
            StatType damageStat = context != null ? context.damageStatType : null;
            if (damageStat == null)
            {
                Debug.LogWarning("Damage stat type not found in context; AddDamageComponent will write to flatDamage but not into spellStats.");
                var db = StatDatabase.Instance;
                damageStat = db != null ? db.GetStat("Damage") : null;
            }

            if (damageStat != null)
            {
                float current = result.spellStats.GetStat(damageStat);
                result.spellStats.SetStat(damageStat, current + dmg);
            }

            if (Verbose(context))
                Debug.Log("AddDamage at " + node.Position + " read " + dmg + " (flatDamage now " + result.flatDamage + ")");
            
            result.cost += component.GetCost(dmg, multiplier: 1f);
        }
        **/
        // Effect nodes don't forward signals unless you want them to.
        node.ClearOutputs();
    }

    private static void EvaluateSetAttackAttribute(DFGridRuntime runtime, DFNodeInstance node, DF_SetAttackAttributeComponent component, DFEvaluationResult result, DFContext context)
    {
        DF_SetAttackAttributeComponent.Evaluate(runtime, node, component, result, context);
        node.ClearOutputs();
    }
}
