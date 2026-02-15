using UnityEngine;

public static class DFPortMap
{
    // Converts a direction enum into a grid offset.
    // Note: Spell grid coordinates use (x right, y down) like UI row/column indices.
    // That means "Up" is y-1 and "Down" is y+1.
    public static Vector2Int ToOffset(Directions dir)
    {
        return dir switch
        {
            Directions.Up => new Vector2Int(0, -1),
            Directions.Down => new Vector2Int(0, 1),
            Directions.Left => new Vector2Int(-1, 0),
            Directions.Right => new Vector2Int(1, 0),
            _ => Vector2Int.zero
        };
    }

    public static Directions Opposite(Directions dir)
    {
        // Used to match an input direction to the neighbor's required output direction.
        return dir switch
        {
            Directions.Up => Directions.Down,
            Directions.Down => Directions.Up,
            Directions.Left => Directions.Right,
            Directions.Right => Directions.Left,
            _ => dir
        };
    }
}
