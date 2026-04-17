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
    private readonly List<Coroutine> activeDebuffCoroutines = new List<Coroutine>();
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
            activeDebuffCoroutines.Add(StartCoroutine(inflictor.debuff.TickEffect()));
            Debug.Log("Applied " + inflictor.debuff.debuffName + " to " + target.name + " from player.");
        }
        var firstEnemy = target.GetComponent<Enemy>();
        var firstDmgType = GetDamageType(firstEnemy, player.GetAttackAttributes());
        float firstDmg = CalculateDamageTaken(firstEnemy, player.GetDamage() * damageMult, player.GetAttackAttributes());
        firstEnemy.TakeDamage(firstDmg);
        SpawnDamageIndicators(target.transform.position, firstDmg, firstDmgType);
        targeted = firstEnemy;
        // get data values for spells
        // begin combat routine
        target.GetComponent<Enemy>().BeginCombat(player);
        StartCoroutine(CombatRoutine(attackSpeed, target));
    }

    private IEnumerator CombatRoutine(float attackSpeed, GameObject target)
    {
        Manager.instance.playerCanMove = false; // stop movement during battle
        float attackInterval = attackSpeed > 0f ? 1f / attackSpeed : 1f;
        while (target != null)
        {
            anim.SetTrigger("Attacking");
            yield return new WaitForSeconds(attackInterval * 0.4f); // wait for attack animation to reach hit frame

            // Enemy may have died during the wait (e.g. from a DoT/debuff tick)
            if (target == null)
            {
                anim.SetTrigger("Idle");
                break;
            }

            var combatEnemy = target.GetComponent<Enemy>();
            var dmgType = GetDamageType(combatEnemy, player.GetAttackAttributes());
            float dmg = CalculateDamageTaken(combatEnemy, player.GetDamage(), player.GetAttackAttributes());
            combatEnemy.TakeDamage(dmg);
            SpawnDamageIndicators(target.transform.position, dmg, dmgType);
            anim.SetTrigger("Idle");

            yield return new WaitForSeconds(attackInterval * 0.6f);
        }
        Manager.instance.playerCanMove = true;
    }
    private DamageNumberType GetDamageType(Enemy enemy, System.Collections.Generic.List<PlayerAttackAttributes> attackAttributes)
    {
        bool isSpell = player.spellManager.GetSpellBehaviour() != null;
        bool isWeakness = false;

        if (enemy.stats.esh.weakness != null)
        {
            if (attackAttributes != null)
            {
                foreach (var attr in attackAttributes)
                {
                    if (attr.attackAttribute == enemy.stats.esh.weakness.attribute && attr.attackAttributeValue > 0)
                    {
                        isWeakness = true;
                        break;
                    }
                }
            }

            if (!isWeakness && isSpell)
            {
                var spellAttrs = player.spellManager.GetTempPlayerAttributeSet().GetAttackAttributes();
                if (spellAttrs != null)
                {
                    foreach (var attr in spellAttrs)
                    {
                        if (attr.attackAttribute == enemy.stats.esh.weakness.attribute && attr.attackAttributeValue > 0)
                        {
                            isWeakness = true;
                            break;
                        }
                    }
                }
            }
        }

        if (isSpell && isWeakness) return DamageNumberType.SpellWeakness;
        if (isSpell) return DamageNumberType.Spell;
        if (isWeakness) return DamageNumberType.Weakness;
        return DamageNumberType.Normal;
    }

    private void SpawnDamageIndicators(Vector3 position, float damage, DamageNumberType type)
    {
        if (DamageNumberSpawner.Instance == null) return;
        DamageNumberSpawner.Instance.SpawnDamageNumber(position, damage, type);
        if (type == DamageNumberType.Spell || type == DamageNumberType.SpellWeakness)
            DamageNumberSpawner.Instance.SpawnSpellIndicator(position, "* MAGIC", new Color(0.75f, 0.25f, 1f));
    }

    float DamageAfterSpell(Enemy enemy)
    {
        float totalDamage = 0;
        if (player.spellManager.GetSpellBehaviour() == null) return totalDamage;
        StatCollection stats = player.spellManager.GetSpellStats();
        List<PlayerAttackAttributes> tempAttackAttributes = player.spellManager.GetTempPlayerAttributeSet().GetAttackAttributes();
        bool gotDamageFromStats = false;
        if (stats != null)
            gotDamageFromStats = stats.TryGetStat("Damage", out totalDamage);

        if (!gotDamageFromStats)
            totalDamage = player.spellManager.GetPendingSpellFlatDamage();

        // Apply enemy defense + attribute interactions, but DO NOT call CalculateDamageTaken()
        // (it calls DamageAfterSpell again, causing infinite recursion).
        float afterDefense = Mathf.Max(0f, totalDamage - enemy.stats.esh.defense);
        float attributeDamage = CalculateAttributeDamage(tempAttackAttributes, enemy);
        float finalSpellDamage = afterDefense + attributeDamage;

        player.spellManager.ClearSpellStats();
        player.spellManager.ClearTempAttributeAttackStats();
        player.spellManager.ClearPendingSpellFlatDamage();
        return finalSpellDamage;
    }
    float SpellDamageMult()
    {
        if (player == null) return 1f;
        var spellBehaviour = player.spellManager.GetSpellBehaviour();
        if (spellBehaviour == null) return 1f;

        float damageMult = spellBehaviour.GetDamageMult();
        return damageMult > 0f ? damageMult : 1f;
    }
    float CalculateDamageTaken(Enemy enemy, float damage, List<PlayerAttackAttributes> attackAttributes)
    {
        var activeSpell = player != null ? player.spellManager.GetSpellBehaviour() : null;

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
            player.spellManager.SetSpellBehaviour(null); // one-shot spell behaviour
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
        foreach (var c in activeDebuffCoroutines)
            if (c != null) StopCoroutine(c);
        activeDebuffCoroutines.Clear();

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
