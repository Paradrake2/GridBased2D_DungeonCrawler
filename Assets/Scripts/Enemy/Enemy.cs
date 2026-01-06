using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool isActive = true; // set to false when player in combat with different enemy
    public LootTable dropTable;
    EnemyStats stats;
    public void TakeDamage(float damage)
    {
        stats.currentHealth -= Mathf.Max(1, damage - stats.defense);
        if (stats.currentHealth <= 0)
        {
            Die();
        }
        Debug.Log(gameObject.name + " took " + damage + " damage.");
    }
    public void Die()
    {
        Debug.Log(gameObject.name + " has died.");
        EnemyManager.instance.RemoveEnemy(this);
        Item dropItem = dropTable.GetDroppedItem();
        if (dropItem != null)
        {
            Instantiate(dropItem.itemPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
    void Start()
    {
        stats = GetComponent<EnemyStats>();
        EnemyManager.instance.AddEnemy(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
