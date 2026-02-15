using System.Collections.Generic;
using UnityEngine;

public sealed class DFGridRuntime
{
    // Runtime representation of the placed grid:
    // position (x,y) -> node instance. Used for constant-time neighbor lookups.
    private readonly Dictionary<Vector2Int, DFNodeInstance> _nodesByPos = new();

    public IEnumerable<DFNodeInstance> Nodes => _nodesByPos.Values;

    public bool TryGetNodeAt(Vector2Int pos, out DFNodeInstance node) => _nodesByPos.TryGetValue(pos, out node);

    public void AddNode(DFNodeInstance node)
    {
        _nodesByPos[node.Position] = node;
    }
}

public sealed class DFNodeInstance
{
    public Vector2Int Position { get; }
    public SpellComponent Component { get; }

    // Output signals per direction (Up/Down/Left/Right).
    // These are read by neighboring nodes via DFEvaluator.TryReadInput.
    private readonly Dictionary<Directions, DFSignal> _outputs = new();

    public DFNodeInstance(Vector2Int position, SpellComponent component)
    {
        Position = position;
        Component = component;
    }

    public DFSignal GetOutput(Directions dir)
    {
        return _outputs.TryGetValue(dir, out var sig) ? sig : DFSignal.None;
    }

    public void SetOutput(Directions dir, DFSignal sig)
    {
        _outputs[dir] = sig;
    }

    public void ClearOutputs()
    {
        _outputs.Clear();
    }
}
