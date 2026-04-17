using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomGenerator : MonoBehaviour
{
    public bool isBossLevel = false;
    public List<RoomData> possibleRooms = new List<RoomData>();
    private RoomData currentRoom;
    public Tilemap floorMap;
    public Tilemap wallMap;
    private List<RoomData> dungeonRooms = new List<RoomData>();
    public void LoadDungeonRooms() // Called when first entering the dungeon
    {
        foreach (var room in Resources.LoadAll<RoomData>("Rooms/Rooms"))
        {
            //if (room.tier <= playerStats.GetStatValue("Level"))
            dungeonRooms.Add(room);
        }
    }
    public void LoadDungeon()
    {
        currentRoom = null;
        floorMap.ClearAllTiles();
        wallMap.ClearAllTiles();
        if (dungeonRooms.Count > 0)
        {
            int randomIndex = Random.Range(0, dungeonRooms.Count);
            RoomData selectedRoom = dungeonRooms[randomIndex];
            selectedRoom.GenerateRoom();
            currentRoom = selectedRoom;
            SpawnPlayerOnFloor(selectedRoom);
        }
        else
        {
            Debug.LogWarning("No rooms available for the player's level.");
        }
    }

    private void SpawnPlayerOnFloor(RoomData room)
    {
        var algo = room.generationAlgorithm;
        if (algo == null || algo.floorPosTracker == null || algo.floorPosTracker.Count == 0)
        {
            Debug.LogWarning("RoomGenerator: No floor positions available to spawn player.");
            return;
        }

        Player player = FindAnyObjectByType<Player>();
        if (player == null)
        {
            Debug.LogWarning("RoomGenerator: No Player found in scene.");
            return;
        }

        // Pick a random floor tile position
        int pick = Random.Range(0, algo.floorPosTracker.Count);
        Vector2Int tilePos = algo.floorPosTracker.ElementAt(pick);
        player.transform.position = new Vector3(tilePos.x + 0.5f, tilePos.y + 0.5f, 0f);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadDungeonRooms();
        LoadDungeon();
    }
}
