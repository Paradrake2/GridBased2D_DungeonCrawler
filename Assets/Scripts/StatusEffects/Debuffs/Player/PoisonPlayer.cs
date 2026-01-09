using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Poison", menuName = "Status Effects/Debuff/PoisonPlayer")]
public class PoisonPlayer : Debuff
{
    float poisonDamage;
    public override void ApplyDebuff(GameObject target, GameObject inflictor, float value = 0)
    {
        Debug.Log(target.name + " is being poisoned.");
        poisonDamage = Mathf.Max(1, inflictor.GetComponent<Player>().stats.GetStatValue("Poison")); // minimum of 1 damage
        this.target = target.gameObject;
        timeSinceLastTick = 0f;
        Enemy enemyComponent = target.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            if (enemyComponent.immunities.Exists(immunity => immunity.immunityName == "Poison"))
            {
                // Enemy is immune to poison
                return;
            }
            if (enemyComponent.HasResistance("Poison"))
            {
                poisonDamage = Mathf.Max(0, poisonDamage * (1 - enemyComponent.GetResistanceValue("Poison"))); // max of 0 in case resistance value is more than 1 which would result in healing
                Debug.Log("Poison damage reduced to " + poisonDamage + " due to resistance on " + target.name);
                return;
            }
            else
            {
                return;
            }
        }
    }
    
    public override IEnumerator TickEffect()
    {
        Debug.Log("Starting poison tick effect on " + target.name);
        while (duration != 0)
        {
            if (duration > 0)
            {
                duration -= tickInterval;
            }
            timeSinceLastTick += tickInterval;
            if (timeSinceLastTick >= tickInterval)
            {
                // Apply poison damage
                target.GetComponent<Enemy>().TakeDamage(poisonDamage, null);
                Debug.Log(target.name + " takes " + poisonDamage + " poison damage.");
                timeSinceLastTick = 0f;
            }
            yield return new WaitForSeconds(tickInterval);
        }
        yield return null;
    }
}
