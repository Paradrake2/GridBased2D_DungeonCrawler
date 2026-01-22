using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "DefenseDownPlayer", menuName = "Status Effects/Debuff/DefenseDownPlayer")]
public class DefenseDownPlayer : Debuff
{
    float defenseReduction;
    public override void ApplyDebuff(GameObject target, GameObject inflictor, float value)
    {
        Debug.Log(target.name + " has defense down " + defenseReduction + ".");
        defenseReduction = value;
        this.target = target.gameObject;
        Enemy enemyComponent = target.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            if (enemyComponent.immunities.Exists(immunity => immunity.immunityName == "DefenseDown"))
            {
                // Enemy is immune to defense down
                return;
            }
            if (enemyComponent.HasResistance("DefenseDown"))
            {
                defenseReduction = Mathf.Max(0, defenseReduction * (1 - enemyComponent.GetResistanceValue("DefenseDown"))); // max of 0 in case resistance value is more than 1 which would result in healing
                Debug.Log("Defense reduction adjusted to " + defenseReduction + " due to resistance on " + target.name);
            }
            enemyComponent.stats.ModifyDefenseMultiplier(1 - defenseReduction);
        }
    }
    public override IEnumerator TickEffect()
    {
        yield break;
    }
}
