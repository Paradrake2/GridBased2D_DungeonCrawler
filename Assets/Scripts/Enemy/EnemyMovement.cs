using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Manager.instance.playerMoving) // Enemy can only move when player is moving
        {
            // Implement enemy movement logic here
        }
    }
}
