using UnityEngine;

public enum EnemyRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(fileName = "EnemyDefinition", menuName = "Scriptable Objects/EnemyDefinition")]
public class EnemyDefinition : ScriptableObject
{
    public string enemyName;
    public Sprite enemyIcon;
    public GameObject enemyPrefab;
    public EnemyRarity rarity;
    public int tier;
}
