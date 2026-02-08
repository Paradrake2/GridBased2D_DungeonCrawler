using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class WallVectorDirections
{
    public static Dictionary<string, Vector2Int[]> directions = new Dictionary<string, Vector2Int[]>()
    {
        {"up", new Vector2Int[] { Vector2Int.up }},
        {"down", new Vector2Int[] { Vector2Int.down }},
        {"left", new Vector2Int[] { Vector2Int.left }},
        {"right", new Vector2Int[] { Vector2Int.right }},
        {"upLeft", new Vector2Int[] { Vector2Int.up, Vector2Int.left }},
        {"upRight", new Vector2Int[] { Vector2Int.up, Vector2Int.right }},
        {"downLeft", new Vector2Int[] { Vector2Int.down, Vector2Int.left }},
        {"downRight", new Vector2Int[] { Vector2Int.down, Vector2Int.right }},
    };
}

public enum TileContentType
{
    None,
    Trap,
    Enemy,
    Obstacle,
    Resource
}


// This big class contains static helper methods for generation algorithms
public static class GenerationAlgorithmComponents
{
    public static bool test = false;
    public static Vector2Int GetRandomDirection()
    {
        int direction = Random.Range(0, 4);
        switch (direction)
        {
            case 0: return Vector2Int.up;
            case 1: return Vector2Int.right;
            case 2: return Vector2Int.down;
            default: return Vector2Int.left;
        }
    }
    public static bool IsEdge()
    {
        return true;
    }
    public static void GenerateContents(RoomData rd, HashSet<Vector2Int> floorPositions)
    {
        Debug.Log("Generating contents for " + rd.roomName);
        foreach(var pos in floorPositions)
        {
            TileContentType contentType = GetTileContent(rd);
            // Debug.Log("Content type for tile at " + pos + ": " + contentType);
            switch (contentType)
            {
                case TileContentType.Enemy:
                    SpawnEnemy(rd, pos);
                    break;
                case TileContentType.Obstacle:
                    SpawnObstacle(rd, pos);
                    break;
                case TileContentType.Trap:
                    SpawnTrap(rd, pos);
                    break;
                case TileContentType.Resource:
                    SpawnResource(rd, pos);
                    break;
                default:
                    break;
            }
        }
        
    }
    public static Vector2 CalculateRoomCenter(HashSet<Vector2Int> floorPositions)
    {
        if (floorPositions.Count == 0) return Vector2.zero;
        
        int minX = int.MaxValue;
        int maxX = int.MinValue;
        int minY = int.MaxValue;
        int maxY = int.MinValue;
        
        foreach (Vector2Int pos in floorPositions)
        {
            if (pos.x < minX) minX = pos.x;
            if (pos.x > maxX) maxX = pos.x;
            if (pos.y < minY) minY = pos.y;
            if (pos.y > maxY) maxY = pos.y;
        }
        
        float centerX = (minX + maxX) / 2f;
        float centerY = (minY + maxY) / 2f;
        
        return new Vector2(centerX, centerY);
    }
    public static void GenerateWalls(HashSet<Vector2Int> floorPositions, RoomData rd, Tilemap map)
    {
        HashSet<Vector2Int> basicWallPos = FindWallsInDirections(floorPositions);
        if (!rd.useRuleTile)
        {
            foreach (var pos in basicWallPos)
            {
                bool floorUp = floorPositions.Contains(pos + Vector2Int.up);
                bool floorDown = floorPositions.Contains(pos + Vector2Int.down);
                bool floorLeft = floorPositions.Contains(pos + Vector2Int.left);
                bool floorRight = floorPositions.Contains(pos + Vector2Int.right);
                bool floorUpRight = floorPositions.Contains(pos + Vector2Int.up + Vector2Int.right);
                bool floorUpLeft = floorPositions.Contains(pos + Vector2Int.up + Vector2Int.left);
                bool floorDownRight = floorPositions.Contains(pos + Vector2Int.down + Vector2Int.right);
                bool floorDownLeft = floorPositions.Contains(pos + Vector2Int.down + Vector2Int.left);
                TileBase wallTileToPlace;

                if (floorUp)
                {
                    wallTileToPlace = rd.roomEnv.wallTile.up[Random.Range(0, rd.roomEnv.wallTile.up.Length)];
                } else if (floorDown)
                {
                    wallTileToPlace = rd.roomEnv.wallTile.down[Random.Range(0, rd.roomEnv.wallTile.down.Length)];
                }
                else if (floorLeft)
                {
                    wallTileToPlace = rd.roomEnv.wallTile.left[Random.Range(0, rd.roomEnv.wallTile.left.Length)];
                }
                else if (floorRight)
                {
                    wallTileToPlace = rd.roomEnv.wallTile.right[Random.Range(0, rd.roomEnv.wallTile.right.Length)];
                }
                else if (floorUpLeft)
                {
                    wallTileToPlace = rd.roomEnv.wallTile.upLeft[Random.Range(0, rd.roomEnv.wallTile.upLeft.Length)];
                }
                else if (floorUpRight)
                {
                    wallTileToPlace = rd.roomEnv.wallTile.upRight[Random.Range(0, rd.roomEnv.wallTile.upRight.Length)];
                }
                else if (floorDownLeft)
                {
                    wallTileToPlace = rd.roomEnv.wallTile.downLeft[Random.Range(0, rd.roomEnv.wallTile.downLeft.Length)];
                }
                else if (floorDownRight)
                {
                    wallTileToPlace = rd.roomEnv.wallTile.downRight[Random.Range(0, rd.roomEnv.wallTile.downRight.Length)];
                }
                else
                {
                    wallTileToPlace = rd.roomEnv.wallTile.single[Random.Range(0, rd.roomEnv.wallTile.single.Length)];
                }
                PlaceSingleTile(map, wallTileToPlace, pos, rd);
            }
        } else
        {
            foreach (var pos in basicWallPos)
            {
                PlaceSingleTile(map, rd.roomEnv.floorTile[Random.Range(0, rd.roomEnv.floorTile.Length)], pos, rd);
                PlaceSingleTile(map, rd.roomEnv.wallTile.ruleTile, pos, rd);
            }
        }
    }
    
    private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPositions)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        
        Vector2Int[] directions =
        {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.up + Vector2Int.left,
            Vector2Int.up + Vector2Int.right,
            Vector2Int.down + Vector2Int.left,
            Vector2Int.down + Vector2Int.right

        };
        foreach (var pos in floorPositions)
        {
            foreach (var dir in directions)
            {
                Vector2Int neighborPos = pos + dir;
                if (!floorPositions.Contains(neighborPos))
                {
                    wallPositions.Add(neighborPos);
                }
            }
        }
        return wallPositions;
    }
    public static void PlaceFloorTile(HashSet<Vector2Int> floorPositions, RoomData roomData, Tilemap map)
    {
        foreach (var pos in floorPositions)
        {
            PlaceSingleTile(map, roomData.roomEnv.floorTile[Random.Range(0, roomData.roomEnv.floorTile.Length)], pos, roomData);
        }
    }
    static void PlaceSingleTile(Tilemap map, TileBase tile, Vector2Int position, RoomData rd)
    {
        Vector3Int tilePosition = new Vector3Int(position.x, position.y, 0);
        map.SetTile(tilePosition, tile);
        //map.SetColor(tilePosition, RoomColor(rd));
    }
    /**
    public static Color RoomColor(RoomData rd)
    {
        switch (rd.roomAttribute)
        {
            case RoomAttribute.Light:
                return Color.yellow;
            case RoomAttribute.Dark:
                return Color.gray;
            case RoomAttribute.Fire:
                return Color.red;
            case RoomAttribute.Water:
                return Color.blue;
            case RoomAttribute.Wind:
                return Color.green;
            case RoomAttribute.Normal:
                return Color.white;
            default:
                return Color.white;
        }
    }
    **/
    public static void SpawnEnemy(RoomData rd, Vector2Int position)
    {
        Debug.LogWarning("Spawning enemy at " + position);
        PotentialEnemiesHolder potE = rd.GetPotentialEnemies();
        List<EnemyDefinition> potentialEnemies = new List<EnemyDefinition>(potE.GetPotentialEnemies());
        EnemyRarity selectedRarity = GetEnemyRarity(rd);
        potentialEnemies.RemoveAll(e => e.rarity != selectedRarity);
        if (potentialEnemies.Count == 0) return;
        EnemyDefinition enemyToSpawn = potentialEnemies[Random.Range(0, potentialEnemies.Count)];
        GameObject newEnemy = GameObject.Instantiate(enemyToSpawn.enemyPrefab, new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity);
    }
    public static void SpawnObstacle(RoomData rd, Vector2Int position)
    {
        List<GameObject> potentialObstacles = rd.GetPotentialObstacles();
        if (potentialObstacles.Count == 0) return;
        GameObject obstacleToSpawn = potentialObstacles[Random.Range(0, potentialObstacles.Count)];
        GameObject newObstacle = GameObject.Instantiate(obstacleToSpawn, new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity);
    }
    public static void SpawnTrap(RoomData rd, Vector2Int position)
    {
        List<GameObject> potentialTraps = rd.GetPotentialTraps();
        if (potentialTraps.Count == 0) return;
        GameObject trapToSpawn = potentialTraps[Random.Range(0, potentialTraps.Count)];
        GameObject newTrap = GameObject.Instantiate(trapToSpawn, new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity);
    }
    public static void SpawnResource(RoomData rd, Vector2Int position)
    {
        List<ResourceWeights> potentialResources = rd.GetPotentialResources();
        if (potentialResources.Count == 0) return;
        int totalWeight = 0;
        foreach (var res in potentialResources)
        {
            totalWeight += res.weight;
        }
        int randomValue = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;
        foreach (var res in potentialResources)
        {
            cumulativeWeight += res.weight;
            if (randomValue <= cumulativeWeight)
            {
                GameObject newResource = GameObject.Instantiate(res.resourcePrefab, new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity);
                break;
            }
        }
    }
    public static TileContentType GetTileContent(RoomData rd)
    {
        int totalWeight = 0;
        foreach (var tcw in rd.tileContentWeights)
        {
            totalWeight += tcw.weight;
        }
        int randomValue = Random.Range(0, totalWeight + 1);
        int cumulativeWeight = 0;
        foreach (var tcw in rd.tileContentWeights)
        {
            cumulativeWeight += tcw.weight;
            if (randomValue <= cumulativeWeight)
            {
                return tcw.tileContentType;
            }
        }
        return TileContentType.None;
    }
    public static EnemyRarity GetEnemyRarity(RoomData rd)
    {
        int totalWeight = 0;
        foreach (var weight in rd.enemyRarityWeights)
        {
            totalWeight += weight.weight;
        }
        int randomValue = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;
        foreach (var weight in rd.enemyRarityWeights)
        {
            cumulativeWeight += weight.weight;
            if (randomValue <= cumulativeWeight)
            {
                return weight.enemyRarity;
            }
        }
        return EnemyRarity.Common; // Default return value if none matched
    }
    public static Vector2Int CalculateRoomDimensions(HashSet<Vector2Int> floorPositions)
    {
        if (floorPositions.Count == 0) return Vector2Int.zero;
        
        int minX = int.MaxValue;
        int maxX = int.MinValue;
        int minY = int.MaxValue;
        int maxY = int.MinValue;
        
        foreach (Vector2Int pos in floorPositions)
        {
            if (pos.x < minX) minX = pos.x;
            if (pos.x > maxX) maxX = pos.x;
            if (pos.y < minY) minY = pos.y;
            if (pos.y > maxY) maxY = pos.y;
        }
        
        int width = maxX - minX + 1;
        int height = maxY - minY + 1;
        
        return new Vector2Int(width + 2, height + 2); // additional 2 is for the edges of the map
    }
}
