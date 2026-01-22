using UnityEngine;

[CreateAssetMenu(fileName = "CaveRoom", menuName = "Rooms/CaveRoom")]
public class CaveRoom : RoomData
{
    public int minRooms = 1;
    public int maxRooms = 3;
    public int walkLength = 10;
    public int iterations = 10;
    public bool startRandomlyEachIteration = true;
    public override void GenerateRoom()
    {
        if (generationAlgorithm != null)
        {
            generationAlgorithm.GenerateMap(this);
        }
        else
        {
            Debug.LogWarning("No generation algorithm assigned for room: " + roomName);
        }
    }
}
