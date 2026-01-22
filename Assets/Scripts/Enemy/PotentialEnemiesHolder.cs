using UnityEngine;

[CreateAssetMenu(fileName = "PotentialEnemiesHolder", menuName = "Management/PotentialEnemiesHolder")]
public class PotentialEnemiesHolder : ScriptableObject
{
    public EnemyDefinition[] potentialEnemies;
    public EnemyDefinition[] GetPotentialEnemies()
    {
        return potentialEnemies;
    }
}
