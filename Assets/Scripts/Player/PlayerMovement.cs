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
    public bool diagonalMovementAllowed = false;
    private Vector2 lastPressedDirection = Vector2.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLastPressedDirection();
        if (!isMoving)
        {
            if (diagonalMovementAllowed)
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
                    if (!isMoving) StartCoroutine(Move(direction));
                }
            } else
            {
                Vector2 direction = Vector2.zero;
                // Use last pressed if still held, otherwise fall back to any held key
                if (IsHeld(lastPressedDirection)) direction = lastPressedDirection;
                else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) direction = Vector2.up;
                else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) direction = Vector2.down;
                else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) direction = Vector2.left;
                else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) direction = Vector2.right;
                if (direction != Vector2.zero)
                {
                    if (currentDirection != direction)
                    {
                        currentDirection = direction;
                        distanceTraveled = 0f;
                    }
                    if (!isMoving) StartCoroutine(Move(direction));
                }
            }
        }
    }
    private void UpdateLastPressedDirection()
    {

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) lastPressedDirection = Vector2.up;
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) lastPressedDirection = Vector2.down;
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) lastPressedDirection = Vector2.left;
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) lastPressedDirection = Vector2.right;
    }
    private bool IsHeld(Vector2 dir)
    {
        if (dir == Vector2.up) return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        if (dir == Vector2.down) return Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        if (dir == Vector2.left) return Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        if (dir == Vector2.right) return Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        return false;
    }
    public void ForceStopMovement()
    {
        StopCoroutine(Move(currentDirection));
        isMoving = false;
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
            if (Manager.instance.playerCanMove == false)
            {
                isMoving = false;
                yield break;
            }
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
