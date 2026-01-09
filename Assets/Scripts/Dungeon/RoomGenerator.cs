using System.Collections.Generic;
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
        }
        else
        {
            Debug.LogWarning("No rooms available for the player's level.");
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadDungeonRooms();
        LoadDungeon();
    }
}
