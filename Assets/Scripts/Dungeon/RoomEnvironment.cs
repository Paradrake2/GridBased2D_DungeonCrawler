using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class WallDirections
{
    public TileBase[] up;
    public TileBase[] down;
    public TileBase[] left;
    public TileBase[] right;
    public TileBase[] upLeft;
    public TileBase[] upRight;
    public TileBase[] downLeft;
    public TileBase[] downRight;
    public TileBase[] single;
    public TileBase ruleTile;
}


[CreateAssetMenu(fileName = "RoomEnvironment", menuName = "Scriptable Objects/RoomEnvironment")]
public class RoomEnvironment : ScriptableObject
{
    public TileBase[] floorTile;
    public WallDirections wallTile;
}
