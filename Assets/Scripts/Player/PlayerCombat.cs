using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerCombat : MonoBehaviour
{
    public Player player;
    public PlayerMovement movement;
    public Manager manager;
    private Vector2 startPos;
    void Start()
    {
        player = GetComponent<Player>();
        manager = FindAnyObjectByType<Manager>();
        movement = GetComponent<PlayerMovement>();
    }
    public void StartCombat(GameObject target, float attackSpeed, List<PlayerAttackAttributes> attackAttributes, float damageMult, List<PlayerDebuffInflictorHolder> debuffInflictors)
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
        target.GetComponent<Enemy>().TakeDamage(player.GetDamage() * damageMult, attackAttributes); // initial attack with damage multiplier
        StartCoroutine(ConmbatRoutine(attackSpeed, target, attackAttributes));
    }

    private IEnumerator ConmbatRoutine(float attackSpeed, GameObject target, List<PlayerAttackAttributes> attackAttributes)
    {
        Manager.instance.playerCanMove = false; // stop movement during battle
        float attackInterval = 1f / attackSpeed;
        while (target != null)
        {
            //if (target.GetComponent<Enemy>() == null) break; // break when enemy is dead
            Debug.LogWarning("Player attacks " + target.name);
            target.GetComponent<Enemy>().TakeDamage(player.GetDamage(), attackAttributes);
            yield return new WaitForSeconds(attackInterval);
        }
        Manager.instance.playerCanMove = true;
    }
    public void PositionForCombat(GameObject target)
    {
        Vector2 targetPos = target.transform.position;
        GetComponent<BoxCollider2D>().enabled = false; // disable detection hitbox
        movement.ForceStopMovement();
        transform.position = new Vector2(targetPos.x - 0.5f, targetPos.y); // position player to the left of the enemy
        target.transform.position = new Vector2(targetPos.x + 0.5f, targetPos.y); // position enemy to the right of the player
    }
    public void EndCombat()
    {
        transform.position = startPos;
        Debug.Log("Combat ended, returning to position " + startPos);
        Manager.instance.playerCanMove = true;
        GetComponent<BoxCollider2D>().enabled = true; // re-enable detection hitbox
        movement.anim.ResetTrigger("Attacking");
        movement.anim.SetTrigger("Idle");
        movement.anim.ResetTrigger("Idle");
    }


}
