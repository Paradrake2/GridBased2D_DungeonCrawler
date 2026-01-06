using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Player player;
    void Start()
    {
        player = GetComponent<Player>();
    }
    public void StartCombat(GameObject target, float attackSpeed)
    {
        StartCoroutine(ConmbatRoutine(attackSpeed, target));
    }

    private IEnumerator ConmbatRoutine(float attackSpeed, GameObject target)
    {
        float attackInterval = 1f / attackSpeed;
        while (target != null)
        {
            target.GetComponent<Enemy>().TakeDamage(player.damage);
            
            yield return new WaitForSeconds(attackInterval);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
