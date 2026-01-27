using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float cellSize = 1.0f;

    [Header("Grid Snapping")]
    [SerializeField] private Vector2 gridOrigin = new Vector2(0.5f, 0.5f); 
    [SerializeField] private bool snapWhenIdle = true;

    [Header("Collision")]
    public LayerMask obstacleLayer; // Layer for obstacles
    public LayerMask environementalLootLayer;

    private bool isMoving = false;
    [SerializeField] private float moveDuration = 0.05f;

    [Header("Movement Tracking")]
    private Vector2 currentDirection = Vector2.zero;
    private float distanceTraveled = 0f;
    private Vector2 lastPosition;
    public bool diagonalMovementAllowed = false;
    private Vector2 lastPressedDirection = Vector2.zero;

    private Collider2D playerCollider;
    private float collisionInset = 0.05f; // Small inset to avoid edge collisions
    [SerializeField] private bool canMove = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCollider = GetComponent<Collider2D>();

        // Ensure we start perfectly aligned
        transform.position = SnapToGrid(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving && snapWhenIdle)
            transform.position = SnapToGrid(transform.position);

        UpdateLastPressedDirection();
        if (!isMoving && canMove)
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
                    if (!isMoving && !IsTargetBlocked(direction)) StartCoroutine(Move(direction));
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
                    if (!isMoving && !IsTargetBlocked(direction)) StartCoroutine(Move(direction));
                    if (!IsObstacleOrLootAtPosition((Vector2)transform.position + direction * cellSize))
                    {
                        // Loot detected, open it
                        GetLootAtPosition((Vector2)transform.position + direction * cellSize);
                    }
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
            yield break;

        isMoving = true;

        // Always move from an exact grid position
        Vector2 startPosition = SnapToGrid(transform.position);
        Vector2 targetPosition = SnapToGrid(startPosition + direction.normalized * cellSize);

        float elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            if (Manager.instance.playerCanMove == false)
            {
                isMoving = false;
                yield break;
            }

            elapsedTime += Time.deltaTime;
            float percent = Mathf.Clamp01(elapsedTime / moveDuration);
            transform.position = Vector2.Lerp(startPosition, targetPosition, percent);
            yield return null;
        }

        // Hard snap at the end to kill drift
        transform.position = targetPosition;

        distanceTraveled += cellSize;
        lastPosition = transform.position;

        isMoving = false;
    }
    private Vector2 SnapToGrid(Vector2 worldPos)
    {
        float x = Mathf.Round((worldPos.x - gridOrigin.x) / cellSize) * cellSize + gridOrigin.x;
        float y = Mathf.Round((worldPos.y - gridOrigin.y) / cellSize) * cellSize + gridOrigin.y;
        return new Vector2(x, y);
    }
    private bool IsTargetBlocked(Vector2 direction)
    {
        Vector2 snappedPos = SnapToGrid(transform.position);

        Vector2 delta = direction.normalized * cellSize;

        // Use snapped position + collider offset to get a stable check center
        Vector2 centerNow = snappedPos + playerCollider.offset;
        Vector2 centerTarget = centerNow + delta;

        Vector2 size = playerCollider.bounds.size;
        size.x = Mathf.Max(0.001f, size.x - collisionInset);
        size.y = Mathf.Max(0.001f, size.y - collisionInset);

        LayerMask blockingMask = obstacleLayer | environementalLootLayer;
        Collider2D hit = Physics2D.OverlapBox(centerTarget, size, 0f, blockingMask);
        return hit != null;
    }
    // if false, loot detected
    private bool IsObstacleOrLootAtPosition(Vector2 position)
    {
        Vector2 snappedPos = SnapToGrid(position);

        // Use snapped position + collider offset to get a stable check center
        Vector2 center = snappedPos + playerCollider.offset;

        Vector2 size = playerCollider.bounds.size;
        size.x = Mathf.Max(0.001f, size.x - collisionInset);
        size.y = Mathf.Max(0.001f, size.y - collisionInset);

        Collider2D hitObstacle = Physics2D.OverlapBox(center, size, 0f, obstacleLayer);
        if (hitObstacle != null) return true;

        Collider2D hitLoot = Physics2D.OverlapBox(center, size, 0f, environementalLootLayer);
        if (hitLoot != null) return false;

        return true;
    }
    private void GetLootAtPosition(Vector2 position)
    {
        Vector2 snappedPos = SnapToGrid(position);

        // Use snapped position + collider offset to get a stable check center
        Vector2 center = snappedPos + playerCollider.offset;

        Vector2 size = playerCollider.bounds.size;
        size.x = Mathf.Max(0.001f, size.x - collisionInset);
        size.y = Mathf.Max(0.001f, size.y - collisionInset);
        
        Collider2D hitLoot = Physics2D.OverlapBox(center, size, 0f, environementalLootLayer);
        if (hitLoot != null)
        {
            hitLoot.GetComponent<Chest>().OpenChest();
        }
    }
    public float GetDistanceTraveled()
    {
        return distanceTraveled;
    }
    public bool GetCanMove()
    {
        return canMove;
    }
    public void SetCanMove(bool value)
    {
        canMove = value;
    }
}
