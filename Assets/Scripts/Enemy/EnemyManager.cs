using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<Enemy> currentEnemies = new List<Enemy>();
    public static EnemyManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void RemoveEnemy(Enemy enemy)
    {
        if (currentEnemies.Contains(enemy))
        {
            currentEnemies.Remove(enemy);
        }
    }
    public void AddEnemy(Enemy enemy)
    {
        if (!currentEnemies.Contains(enemy))
        {
            currentEnemies.Add(enemy);
        }
    }
    public void InCombatWith(Enemy enemy)
    {
        foreach (Enemy e in currentEnemies)
        {
            if (e != enemy)
            {
                e.isActive = false;
            }
            else
            {
                e.isActive = true;
            }
        }
    }
    public void ExitCombat()
    {
        foreach (Enemy e in currentEnemies)
        {
            e.isActive = true;
        }
    }
    public void ClearEnemies()
    {
        currentEnemies.Clear();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
