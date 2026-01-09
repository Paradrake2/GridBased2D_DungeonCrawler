using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float cellSize = 1.0f;
    [Header("Collision")]
    public LayerMask obstacleLayer; // Layer for obstacles
    private bool isMoving = false;
    [SerializeField] private float moveDuration = 0.05f;

    [Header("Movement Tracking")]
    private Vector2 currentDirection = Vector2.zero;
    private float distanceTraveled = 0f;
    private Vector2 lastPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving)
        {
            Vector2 direction = Vector2.zero;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                direction.y = 1;
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                direction.y = -1;
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                direction.x = -1;
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                direction.x = 1;
            }
            if (direction != Vector2.zero)
            {
                if (currentDirection != direction)
                {
                    currentDirection = direction;
                    distanceTraveled = 0f;
                }
                StartCoroutine(Move(direction));
            }
        }
    }

    private IEnumerator Move(Vector2 direction)
    {
        if (Manager.instance.playerCanMove == false)
        {
            yield break;
        }
        isMoving = true;
        Vector2 startPosition = transform.position;
        Vector2 targetPosition = startPosition + direction * cellSize;

        float elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float percent = elapsedTime / moveDuration;
            transform.position = Vector2.Lerp(startPosition, targetPosition, percent);
            yield return null;
        }

        transform.position = targetPosition;

        distanceTraveled += cellSize;
        lastPosition = transform.position;

        isMoving = false;
    }
    public float GetDistanceTraveled()
    {
        return distanceTraveled;
    }
}
