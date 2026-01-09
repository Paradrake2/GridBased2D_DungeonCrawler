using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomContents
{
    public List<GameObject> potentialObstacles;
    public List<GameObject> potentialTraps;
    public PotentialEnemiesHolder potentialEnemies;
    public List<ResourceWeights> potentialResources;
}
[System.Serializable]
public class TileContentWeights
{
    public TileContentType tileContentType;
    public int weight = 1;
}
[System.Serializable]
public class ResourceWeights
{
    public GameObject resourcePrefab;
    public int weight = 1;
}
[System.Serializable]
public class EnemyRarityWeight
{
    public EnemyRarity enemyRarity;
    public int weight = 1;
}


[CreateAssetMenu(fileName = "RoomData", menuName = "Scriptable Objects/RoomData")]
public abstract class RoomData : ScriptableObject
{
    public string roomName;
    public string ID;
    public int tier; // how strong the enemies are
    public bool isBossRoom = false;
    public RoomContents roomContents;
    public RoomEnvironment roomEnv;
    public GenerationAlgorithm generationAlgorithm;
    public TileContentWeights[] tileContentWeights;
    public EnemyRarityWeight[] enemyRarityWeights;
    public bool useRuleTile = true;
    public List<ResourceWeights> GetPotentialResources()
    {
        return roomContents.potentialResources;
    }
    public List<GameObject> GetPotentialObstacles()
    {
        return roomContents.potentialObstacles;
    }
    public List<GameObject> GetPotentialTraps()
    {
        return roomContents.potentialTraps;
    }
    public PotentialEnemiesHolder GetPotentialEnemies()
    {
        return roomContents.potentialEnemies;
    }
    public abstract void GenerateRoom();
}
