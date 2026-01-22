using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyResistance
{
    public Resistance resistance;
    public float resistanceValue;
}


public class Enemy : MonoBehaviour
{
    public bool isActive = true; // set to false when player in combat with different enemy
    public LootTable dropTable;
    public EnemyStats stats;
    public List<EnemyResistance> resistances = new List<EnemyResistance>();
    public List<Immunity> immunities = new List<Immunity>();
    private Vector2 positionBeforeCombat;
    public Player player;
    [SerializeField] private int dropItemNum = 1;
    public void TakeDamage(float damage, List<PlayerAttackAttributes> attackAttributes)
    {
        stats.currentHealth -= Mathf.Max(1, CalculateDamageTaken(damage, attackAttributes));
        if (stats.currentHealth <= 0)
        {
            Die();
        }
    }
    public void TakeTrueDamage(float damage)
    {
        stats.currentHealth -= damage;
        if (stats.currentHealth <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        Debug.Log(gameObject.name + " has died.");
        EnemyManager.instance.RemoveEnemy(this);
        float dropChance = player.GetDropChance();
        if (Random.value*100 < dropChance)
        {
            for (int i = 0; i < dropItemNum; i++)
            {
                Item dropItem = dropTable.GetDroppedItem();
                if (dropItem != null)
                {
                    player.AddItemToInventory(dropItem);
                    // UI notification can be added here
                    //Instantiate(dropItem.itemPrefab, transform.position, Quaternion.identity); // might be replaced with just directly adding to player inventory later
                }
            }
        }
        Destroy(gameObject);
        player.combat.EndCombat();
    }
    float CalculateDamageTaken(float damage, List<PlayerAttackAttributes> attackAttributes)
    {
        float totalDamage = Mathf.Max(1, damage - stats.esh.defense);
        Debug.Log("Base Damage: " + totalDamage);
        if (attackAttributes == null) return totalDamage;
        foreach (var attr in attackAttributes)
        {
            StatType defenseAttr = AttributeManager.instance.GetCorrespondingDefenseAttribute(attr.attackAttribute);
            Debug.Log("Against " + defenseAttr.displayName);
            float enemyDefenseValue = stats.GetAttributeValue(defenseAttr);
            Debug.Log("Enemy " + defenseAttr.displayName + ": " + enemyDefenseValue);
            totalDamage += Mathf.Max(0, attr.attackAttributeValue - enemyDefenseValue);
            Debug.Log("After " + attr.attackAttribute + ": " + totalDamage);
        }
        Debug.Log(totalDamage);
        return totalDamage;
    }
    public bool HasResistance(string resistanceName)
    {
        foreach (var res in resistances)
        {
            if (res.resistance.resistanceName == resistanceName)
            {
                return true;
            }
        }
        return false;
    }
    public Vector2 GetPositionBeforeCombat()
    {
        return positionBeforeCombat;
    }
    public float GetResistanceValue(string resistanceName)
    {
        foreach (var res in resistances)
        {
            if (res.resistance.resistanceName == resistanceName)
            {
                return res.resistanceValue;
            }
        }
        return 0f;
    }
    void Start()
    {
        stats = GetComponent<EnemyStats>();
        player = FindAnyObjectByType<Player>();
        EnemyManager.instance.AddEnemy(this);
        positionBeforeCombat = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
