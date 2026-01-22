using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using gac = GenerationAlgorithmComponents;


[CreateAssetMenu(fileName = "RandomWalk", menuName = "Generation/RandomWalk")]
public class RandomWalk : GenerationAlgorithm
{
    private int walkLength = 20;
    private int baseIterations = 10;
    private bool startRandomlyEachIteration = false;
    public override void GenerateMap(RoomData rd)
    {
        GetMaps();
        if (rd is CaveRoom caveRoom)
        {
            walkLength = caveRoom.walkLength;
            baseIterations = caveRoom.iterations;
            startRandomlyEachIteration = caveRoom.startRandomlyEachIteration;
        }
        HashSet<Vector2Int> floorPositions = GenerateRoomBase();
        Debug.LogWarning(startPos);
        gac.PlaceFloorTile(floorPositions, rd, floorMap);
        gac.GenerateWalls(floorPositions, rd, wallMap);
        gac.GenerateContents(rd, floorPositions);
    }
    public HashSet<Vector2Int> GenerateRoomBase()
    {
        var currentPos = startPos;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        for (int i = 0; i < baseIterations; i++)
        {
            var path = RunRandomWalk(currentPos, walkLength);
            floorPositions.UnionWith(path);
            if (startRandomlyEachIteration)
            {
                currentPos = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
            }
        }
        floorPosTracker = floorPositions;
        return floorPositions;
    }
    public HashSet<Vector2Int> RunRandomWalk(Vector2Int startPos, int walkLength)
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        Vector2Int currentPosition = startPos;
        floorPositions.Add(currentPosition);

        for (int i = 0; i < walkLength; i++)
        {
            Vector2Int direction = gac.GetRandomDirection();
            currentPosition += direction;
            floorPositions.Add(currentPosition);
        }

        return floorPositions;
    }
    

}