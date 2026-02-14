using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerCombat : MonoBehaviour
{
    public Player player;
    public PlayerMovement movement;
    public PlayerAnimator anim;
    public Manager manager;
    private Vector2 startPos;
    void Start()
    {
        player = GetComponent<Player>();
        manager = FindAnyObjectByType<Manager>();
        movement = GetComponent<PlayerMovement>();
        anim = GetComponent<PlayerAnimator>();
    }
    public void StartCombat(GameObject target, float attackSpeed, float damageMult, List<PlayerDebuffInflictorHolder> debuffInflictors)
    {
        startPos = transform.position;
        // move into position, remember to disable the detection hitbox so it doesnt start a second fight
        PositionForCombat(target);
        foreach (var inflictor in debuffInflictors)
        {
            inflictor.debuff.ApplyDebuff(target, gameObject, inflictor.value);
            StartCoroutine(inflictor.debuff.TickEffect());
            Debug.Log("Applied " + inflictor.debuff.debuffName + " to " + target.name + " from player.");
        }
        target.GetComponent<Enemy>().TakeDamage(CalculateDamageTaken(target.GetComponent<Enemy>(), player.GetDamage() * damageMult, player.GetAttackAttributes())); // initial attack with damage multiplier
        target.GetComponent<Enemy>().BeginCombat(player);
        StartCoroutine(ConmbatRoutine(attackSpeed, target));
    }

    private IEnumerator ConmbatRoutine(float attackSpeed, GameObject target)
    {
        Manager.instance.playerCanMove = false; // stop movement during battle
        float attackInterval = 1f / attackSpeed;
        while (target != null)
        {
            anim.SetTrigger("Attacking");
            //if (target.GetComponent<Enemy>() == null) break; // break when enemy is dead
            Debug.LogWarning("Player attacks " + target.name);
            yield return new WaitForSeconds(attackInterval*0.4f); // wait for attack animation to reach hit frame
            target.GetComponent<Enemy>().TakeDamage(CalculateDamageTaken(target.GetComponent<Enemy>(), player.GetDamage(), player.GetAttackAttributes()));
            anim.SetTrigger("Idle");
            yield return new WaitForSeconds(attackInterval*0.6f);
        }
        Manager.instance.playerCanMove = true;
    }
    float DamageAfterSpell(Enemy enemy)
    {
        float totalDamage = 0;
        if (player.GetSpellBehaviour() == null) return totalDamage;
        StatCollection stats = player.GetSpellStats();
        List<PlayerAttackAttributes> tempAttackAttributes = player.GetTempPlayerAttributeSet().GetAttackAttributes();
        if (stats != null)
        {
            if (!stats.TryGetStat("Damage", out totalDamage)) return 0f; // if no damage stat, return 0
        }
        totalDamage = CalculateDamageTaken(enemy, totalDamage, tempAttackAttributes); // temp attributes
        player.ClearSpellStats();
        player.ClearTempAttributeAttackStats();
        return totalDamage;
    }
    float SpellDamageMult()
    {
        if (player == null) return 1f;
        var spellBehaviour = player.GetSpellBehaviour();
        if (spellBehaviour == null) return 1f;

        float damageMult = spellBehaviour.GetDamageMult();
        return damageMult > 0f ? damageMult : 1f;
    }
    float CalculateDamageTaken(Enemy enemy, float damage, List<PlayerAttackAttributes> attackAttributes)
    {
        float totalDamage = Mathf.Max(1, damage - enemy.stats.esh.defense);
        // Debug.Log("Base Damage: " + totalDamage);
        if (attackAttributes == null) return totalDamage;
        totalDamage = totalDamage + CalculateAttributeDamage(attackAttributes, enemy); // attribute damage
        totalDamage = totalDamage + DamageAfterSpell(enemy); // spell damage
        totalDamage *= SpellDamageMult(); // spell damage multiplier
        Debug.Log(totalDamage);
        return totalDamage;
    }
    float CalculateAttributeDamage(List<PlayerAttackAttributes> attackAttributes, Enemy enemy)
    {
        float totalDamage = 0;
        if (attackAttributes == null) return totalDamage;
        foreach (var attr in attackAttributes)
        {
            StatType defenseAttr = AttributeManager.instance.GetCorrespondingDefenseAttribute(attr.attackAttribute);
            // Debug.Log("Against " + defenseAttr.displayName);
            float enemyDefenseValue = enemy.stats.GetAttributeValue(defenseAttr);
            // Debug.Log("Enemy " + defenseAttr.displayName + ": " + enemyDefenseValue);
            float potentialDamage = Mathf.Max(0, attr.attackAttributeValue - enemyDefenseValue);
            
            if (enemy.stats.esh.weakness != null)
            {
                if (attr.attackAttribute == enemy.stats.esh.weakness && attr.attackAttributeValue > 0)
                {
                    potentialDamage *= enemy.stats.esh.weaknessMultiplier;
                    // Debug.Log("Weakness applied! New Damage: " + potentialDamage);
                    break;
                }
            }
            totalDamage += potentialDamage;
            // Debug.Log("After " + attr.attackAttribute + ": " + totalDamage);
        }
        return totalDamage;
    }
    public void PositionForCombat(GameObject target)
    {
        movement.ForceStopMovement();
        GetComponent<BoxCollider2D>().enabled = false; // disable detection hitbox
        Vector2 targetPos = target.transform.position;
        player.isInCombat = true;
        transform.position = new Vector2(targetPos.x - 0.5f, targetPos.y); // position player to the left of the enemy
        target.GetComponent<Enemy>().PositionForCombat();
    }
    public void EndCombat()
    {
        transform.position = startPos;
        player.isInCombat = false;
        Debug.Log("Combat ended, returning to position " + startPos);
        Manager.instance.playerCanMove = true;
        GetComponent<BoxCollider2D>().enabled = true; // re-enable detection hitbox
        anim.ResetTrigger("Attacking");
        anim.SetTrigger("Idle");
        anim.ResetTrigger("Idle");
    }


}
