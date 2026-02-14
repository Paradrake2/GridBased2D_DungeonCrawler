using System.Collections;
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
    [SerializeField] private Animator anim;
    public float attackAnimLength = 1f;
    public float attackAnimHitFrame = 0.6f; // time in seconds when the hit frame occurs in the attack animation
    public float idleAnimLength = 1f;
    public float idleAnimTriggerFrame = 0.5f; // time in seconds when the idle animation should be triggered after combat starts
    public void TakeDamage(float damage)
    {
        stats.currentHealth -= Mathf.Max(1, damage);
        if (stats.currentHealth <= 0)
        {
            Die();
        }
    }
    public void PositionForCombat()
    {
        //positionBeforeCombat = new Vector2(transform.position.x, transform.position.y);
        transform.position = new Vector2(transform.position.x + 0.5f, transform.position.y); // position enemy to the right of the player
    }
    public void BeginCombat(Player player)
    {
        anim.SetTrigger("CombatInit");
        StartCoroutine(CombatRoutine(player));
    }
    public void TakeTrueDamage(float damage)
    {
        stats.currentHealth -= damage;
        if (stats.currentHealth <= 0)
        {
            Die();
        }
    }
    private IEnumerator CombatRoutine(Player player)
    {
        float attackSpeed = stats.esh.stats.GetStat(StatDatabase.Instance.GetStat("AttackSpeed"));
        float attackInterval = 1f / attackSpeed;
        Debug.Log(attackInterval);
        while (stats.currentHealth > 0 && player.GetHealth() > 0)
        {
            anim.SetTrigger("AttackStart");
            yield return new WaitForSeconds(attackInterval*0.6f); // wait for attack animation to reach hit frame
            //anim.ResetTrigger("AttackFinished");
//            Debug.LogWarning(gameObject.name + " attacks Player");
            player.TakeDamage(stats.esh.damage, stats.esh.enemyAttributesList);
            //anim.ResetTrigger("AttackStart");
            anim.SetTrigger("AttackFinished");
            yield return new WaitForSeconds(attackInterval*0.4f);
        }
        yield return null;
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
        // Debug.Log("Base Damage: " + totalDamage);
        if (attackAttributes == null) return totalDamage;
        foreach (var attr in attackAttributes)
        {
            StatType defenseAttr = AttributeManager.instance.GetCorrespondingDefenseAttribute(attr.attackAttribute);
            // Debug.Log("Against " + defenseAttr.displayName);
            float enemyDefenseValue = stats.GetAttributeValue(defenseAttr);
            // Debug.Log("Enemy " + defenseAttr.displayName + ": " + enemyDefenseValue);
            float potentialDamage = Mathf.Max(0, attr.attackAttributeValue - enemyDefenseValue);
            
            if (stats.esh.weakness != null)
            {
                if (attr.attackAttribute == stats.esh.weakness && attr.attackAttributeValue > 0)
                {
                    potentialDamage *= stats.esh.weaknessMultiplier;
                    // Debug.Log("Weakness applied! New Damage: " + potentialDamage);
                    break;
                }
            }
            totalDamage += potentialDamage;
            // Debug.Log("After " + attr.attackAttribute + ": " + totalDamage);
        }
        // Debug.Log(totalDamage);
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

    void Update()
    {
        
    }
}
