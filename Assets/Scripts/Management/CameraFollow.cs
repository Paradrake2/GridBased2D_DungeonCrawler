using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player;
    [SerializeField] private Player playerScript;
    public float smoothSpeed = 0.125f;
    void Start()
    {
        playerScript = player.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return; // safety check
        if (playerScript.GetHealth() <= 0) return; // stop following if player is dead
        if (playerScript.isInCombat) return; // stop following during combat to prevent jitter
        Vector3 desiredPosition = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
