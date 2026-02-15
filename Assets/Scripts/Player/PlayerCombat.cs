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
    public Enemy targeted;
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
        targeted = target.GetComponent<Enemy>();
        // get data values for spells
        // begin combat routine
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
           // Debug.LogWarning("Player attacks " + target.name);
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
        bool gotDamageFromStats = false;
        if (stats != null)
            gotDamageFromStats = stats.TryGetStat("Damage", out totalDamage);

        if (!gotDamageFromStats)
            totalDamage = player.GetPendingSpellFlatDamage();

        // Apply enemy defense + attribute interactions, but DO NOT call CalculateDamageTaken()
        // (it calls DamageAfterSpell again, causing infinite recursion).
        float afterDefense = Mathf.Max(0f, totalDamage - enemy.stats.esh.defense);
        float attributeDamage = CalculateAttributeDamage(tempAttackAttributes, enemy);
        float finalSpellDamage = afterDefense + attributeDamage;

        player.ClearSpellStats();
        player.ClearTempAttributeAttackStats();
        player.ClearPendingSpellFlatDamage();
        return finalSpellDamage;
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
        var activeSpell = player != null ? player.GetSpellBehaviour() : null;

        float totalDamage = Mathf.Max(1, damage - enemy.stats.esh.defense);
        //Debug.Log("Base Damage: " + totalDamage);
        if (attackAttributes == null) return totalDamage;
        totalDamage = totalDamage + CalculateAttributeDamage(attackAttributes, enemy); // attribute damage
        //Debug.Log("Damage after attributes: " + totalDamage);
        totalDamage = totalDamage + DamageAfterSpell(enemy); // spell damage
        Debug.Log("Damage after spell and attributes: " + totalDamage);
        if (activeSpell != null)
        {
            float mult = activeSpell.GetDamageMult();
            Debug.Log("Damage multiplier from active spell: " + mult);
            totalDamage *= mult > 0f ? mult : 1f;
            //Debug.Log("Damage after applying multiplier: " + totalDamage);
            player.SetSpellBehaviour(null); // one-shot spell behaviour
        }
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
            //Debug.Log("Potential Damage: " + potentialDamage);
            if (enemy.stats.esh.weakness != null)
            {
                if (attr.attackAttribute == enemy.stats.esh.weakness.attribute && attr.attackAttributeValue > 0)
                {
                    potentialDamage *= enemy.stats.esh.weakness.multiplier;
                    Debug.Log("Weakness applied! New Damage: " + potentialDamage);
                    break;
                }
            }
            totalDamage += potentialDamage;
            //Debug.Log("After " + attr.attackAttribute + ": " + totalDamage);
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
