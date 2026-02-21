using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class DFEvaluationResult
{
    // Output 1: numeric spell stats that plug into the existing combat pipeline.
    // (Right now we only write "Damage", but this is meant to scale.)
    public StatCollection spellStats = new StatCollection();

    // Convenience output so effectors can be tested without needing StatDatabase/StatType wiring.
    // DataflowSpellBehaviour will still map this into the game's StatType-based stats when possible.
    public float flatDamage;

    // Output 2: temporary attack attributes (element type + magnitude) used by weakness/defense logic.
    public PlayerAttributeSet tempAttributeSet = new PlayerAttributeSet();
}

public static class DFEvaluator
{
    public const int DefaultMaxPasses = 8;

    private static bool Verbose(DFContext context) => context != null && context.verbose;

    // Used only for optional verbose diagnostics during a single Evaluate() call.
    // Unity runs this on the main thread, so this is safe for our use.
    private static DFContext currentContext;

    // Some of your component assets use SpellComponentDirectionPart.direction (Vector2) to define ports,
    // while the dataflow runtime historically used SpellComponentDirectionPart.directions (enum).
    // That mismatch makes the UI show "correct" arrows, but evaluation can't find connections.
    // This helper makes evaluation treat both representations consistently.
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

        // Outputs may depend on previous passes (simple feedback/propagation). Keep bounded so we
        // never hang even if the grid forms cycles.
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

    private static bool TryReadInput(DFGridRuntime runtime, DFNodeInstance node, Directions inputDir, out DFSignal signal)
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
        // Example: if we want input from Left, the neighbor is at (x-1,y) and must output Right.
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

    private static void WriteOutputsToAllActiveDirections(DFNodeInstance node, DFSignal signal)
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
        StatType weakness = null;
        if (context != null && context.target != null && context.target.stats != null && context.target.stats.esh != null)
        {
            weakness = context.target.stats.esh.weakness != null ? context.target.stats.esh.weakness.attribute : null;
        }

        // Emits the enemy's weakness attribute (e.g., Fire) as an Attribute signal.
        //Debug.Log("Enemy weakness detected: " + (weakness != null ? weakness.displayName : "None"));
        //Debug.Log(context.target.name);
        WriteOutputsToAllActiveDirections(node, DFSignal.FromAttribute(weakness));
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
        // Pass-through: read a single incoming signal and replicate it to all active outputs.
        // Input/output directions are fully defined by the component's active ports.
        if (runtime == null || node == null || component == null)
            return;

        DFSignal signal = DFSignal.None;

        bool hasSignal = TryReadAnyActiveInput(runtime, node, out signal);

        WriteOutputsToAllActiveDirections(node, hasSignal ? signal : DFSignal.None);
    }

    private static bool TryReadAnyActiveInput(DFGridRuntime runtime, DFNodeInstance node, out DFSignal signal)
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
        // Reads two number inputs (configurable directions) and outputs a*b.
        float a = 0f;
        float b = 0f;
        bool hasA = TryReadInput(runtime, node, component.inputA, out var sigA) && sigA.TryGetNumber(out a);
        bool hasB = TryReadInput(runtime, node, component.inputB, out var sigB) && sigB.TryGetNumber(out b);

        if (!hasA || !hasB)
        {
            WriteOutputsToAllActiveDirections(node, DFSignal.None);
            return;
        }

        WriteOutputsToAllActiveDirections(node, DFSignal.FromNumber(a * b));
    }

    private static void EvaluateEqualsAttribute(DFGridRuntime runtime, DFNodeInstance node, DF_EqualsAttributeComponent component)
    {
        StatType inputAttr = null;
        StatType compareAttr = null;

        bool hasInput =
            TryReadInput(runtime, node, component.inputAttribute, out var sigInput) &&
            sigInput.TryGetAttribute(out inputAttr);

        bool hasCompareInput =
            TryReadInput(runtime, node, component.compareAttribute, out var sigCompare) &&
            sigCompare.TryGetAttribute(out compareAttr);

        if (!hasCompareInput)
            compareAttr = component.compareTo;

        bool match = hasInput && inputAttr != null && compareAttr != null && inputAttr == compareAttr;

        WriteOutputsToAllActiveDirections(node, DFSignal.FromBool(match));
    }

    private static void EvaluateGateNumber(DFGridRuntime runtime, DFNodeInstance node, DF_GateNumberComponent component)
    {
        // If condition is true, forward the number. Otherwise output 0.
        bool cond = false;
        float value = 0f;

        bool hasCond = TryReadInput(runtime, node, component.inputCondition, out var sigCond) && sigCond.TryGetBool(out cond);
        bool hasVal = TryReadInput(runtime, node, component.inputValue, out var sigVal) && sigVal.TryGetNumber(out value);

        if (!hasCond || !hasVal)
        {
            WriteOutputsToAllActiveDirections(node, DFSignal.None);
            return;
        }

        WriteOutputsToAllActiveDirections(node, DFSignal.FromNumber(cond ? value : 0f));
    }

    private static void EvaluateAddDamage(DFGridRuntime runtime, DFNodeInstance node, DF_AddDamageComponent component, DFEvaluationResult result, DFContext context)
    {
        if (result == null) return;
        // Effector: consumes a Number signal and accumulates it into the "Damage" stat.
        if (TryReadInput(runtime, node, component.inputDamage, out var sig) && sig.TryGetNumber(out float dmg))
        {
            result.flatDamage += dmg;

            StatType damageStat = context != null ? context.damageStatType : null;
            if (damageStat == null)
            {
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
        }

        // Effect nodes don't forward signals unless you want them to.
        node.ClearOutputs();
    }

    private static void EvaluateSetAttackAttribute(DFGridRuntime runtime, DFNodeInstance node, DF_SetAttackAttributeComponent component, DFEvaluationResult result, DFContext context)
    {
        if (result == null) return;

        // Effector: consumes an Attribute + a Number and writes it into the temp attack attributes.
        StatType attr = null;
        float v = 0f;
        bool hasAttr = TryReadInput(runtime, node, component.inputAttribute, out var sigAttr) && sigAttr.TryGetAttribute(out attr);
        bool hasVal = TryReadInput(runtime, node, component.inputValue, out var sigVal) && sigVal.TryGetNumber(out v);

        if (hasAttr && attr != null && hasVal)
        {
            result.tempAttributeSet.AddOrSetAttackAttribute(attr, v);
            if (Verbose(context))
                Debug.Log("Set attack attribute: " + attr.displayName + " to " + v);
        }

        node.ClearOutputs();
    }
}
