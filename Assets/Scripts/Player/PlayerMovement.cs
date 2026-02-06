using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float cellSize = 1.0f;

    [Header("Grid Snapping")]
    [SerializeField] private Vector2 gridOrigin = new Vector2(0.5f, 0.5f);
    [SerializeField] private bool snapWhenIdle = true;

    [Header("Collision")]
    public LayerMask obstacleLayer;
    public LayerMask environementalLootLayer;

    private bool isMoving = false;
    [SerializeField] private float moveDuration = 0.05f;

    [Header("Movement Tracking")]
    private Vector2 currentDirection = Vector2.zero;
    private float distanceTraveled = 0f;
    private Vector2 lastPosition;
    public bool diagonalMovementAllowed = false;
    private Vector2 lastPressedDirection = Vector2.zero;
   // [SerializeField] public Animator anim;
    private Collider2D playerCollider;
    private float collisionInset = 0.05f;
    [SerializeField] private bool canMove = true;

    private Vector2 facingDirection = Vector2.down;

    // Animation state tracking (prevents trigger spam / conflicts)
    private Vector2 lastAnimDirection = Vector2.down;
    private bool wasAnimatingWalk = false;
    private bool idleTriggered = false;
    [SerializeField] private PlayerAnimator anim;
    [SerializeField] private Player player;
    void Start()
    {
        playerCollider = GetComponent<Collider2D>();
        player = GetComponent<Player>();
        //anim = GetComponent<Animator>();
        transform.position = SnapToGrid(transform.position);

        TriggerIdleFromFacing();
        idleTriggered = true;
    }

    void Update()
    {
        if (!isMoving && snapWhenIdle && !player.isInCombat)
            transform.position = SnapToGrid(transform.position);

        UpdateLastPressedDirection();

        if (!isMoving && canMove)
        {
            Vector2 direction = diagonalMovementAllowed ? GetDiagonalDirection() : GetCardinalDirection();

            // ---- Animation selection (single source of truth) ----
            if (direction != Vector2.zero)
            {
                SetFacingDirection(direction);
                PlayWalkIfChanged(direction);
                idleTriggered = false;

                if (currentDirection != direction)
                {
                    currentDirection = direction;
                    distanceTraveled = 0f;
                }

                if (!IsTargetBlocked(direction))
                    StartCoroutine(Move(direction));

                if (!IsObstacleOrLootAtPosition((Vector2)transform.position + direction * cellSize))
                    GetLootAtPosition((Vector2)transform.position + direction * cellSize);
            }
            else
            {
                // No input this frame (while not moving)
                if (!idleTriggered)
                {
                    TriggerIdleFromFacing();
                    idleTriggered = true;
                }
            }
        }
        else
        {
            // If we finished a move and no keys are held, ensure we idle once.
            if (!isMoving && !AnyMoveKeyHeld() && !idleTriggered)
            {
                TriggerIdleFromFacing();
                idleTriggered = true;
            }
        }
    }

    private bool AnyMoveKeyHeld()
    {
        return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) ||
               Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) ||
               Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ||
               Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
    }

    private Vector2 GetDiagonalDirection()
    {
        Vector2 dir = Vector2.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) dir.y = 1;
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) dir.y = -1;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) dir.x = -1;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) dir.x = 1;

        return dir;
    }

    // Cardinal-only direction selection that behaves well with multiple keys:
    // 1) Prefer lastPressedDirection IF it is still held
    // 2) Otherwise pick any currently held direction in a consistent order
    private Vector2 GetCardinalDirection()
    {
        if (IsHeld(lastPressedDirection)) return lastPressedDirection;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) return Vector2.up;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) return Vector2.down;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) return Vector2.left;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) return Vector2.right;

        return Vector2.zero;
    }

    private void PlayWalkIfChanged(Vector2 direction)
    {
        // Only fire a walking trigger when player actually changed direction
        if (wasAnimatingWalk && lastAnimDirection == direction) return;

        ResetIdleTriggers();
        ResetMovementTriggers();

        if (direction == Vector2.up) anim.SetTrigger("WalkingUp");
        else if (direction == Vector2.down) anim.SetTrigger("WalkingDown");
        else if (direction == Vector2.left) anim.SetTrigger("WalkingLeft");
        else if (direction == Vector2.right) anim.SetTrigger("WalkingRight");

        wasAnimatingWalk = true;
        lastAnimDirection = direction;
    }

    private void UpdateLastPressedDirection()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) lastPressedDirection = Vector2.up;
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) lastPressedDirection = Vector2.down;
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) lastPressedDirection = Vector2.left;
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) lastPressedDirection = Vector2.right;
    }

    // IMPORTANT: IsHeld should NOT set triggers.
    private bool IsHeld(Vector2 dir)
    {
        if (dir == Vector2.up) return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        if (dir == Vector2.down) return Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        if (dir == Vector2.left) return Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        if (dir == Vector2.right) return Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        return false;
    }

    void ResetMovementTriggers()
    {
        anim.ResetTrigger("WalkingUp");
        anim.ResetTrigger("WalkingDown");
        anim.ResetTrigger("WalkingLeft");
        anim.ResetTrigger("WalkingRight");
    }

    void ResetIdleTriggers()
    {
        anim.ResetTrigger("IdleUp");
        anim.ResetTrigger("IdleDown");
        anim.ResetTrigger("IdleLeft");
        anim.ResetTrigger("IdleRight");
    }

    public void ForceStopMovement()
    {
        StopCoroutine(Move(currentDirection));
        anim.SetTrigger("Attacking");
        isMoving = false;

        // After forced stop, go idle based on facing once
        wasAnimatingWalk = false;
        idleTriggered = false;
    }

    private IEnumerator Move(Vector2 direction)
    {
        if (Manager.instance.playerCanMove == false)
            yield break;

        isMoving = true;

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

        transform.position = targetPosition;

        distanceTraveled += cellSize;
        lastPosition = transform.position;

        isMoving = false;

        // If player isn't holding a direction after the step, idle once.
        if (!AnyMoveKeyHeld())
        {
            wasAnimatingWalk = false;
            idleTriggered = false;
        }
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

        Vector2 centerNow = snappedPos + playerCollider.offset;
        Vector2 centerTarget = centerNow + delta;

        Vector2 size = playerCollider.bounds.size;
        size.x = Mathf.Max(0.001f, size.x - collisionInset);
        size.y = Mathf.Max(0.001f, size.y - collisionInset);

        LayerMask blockingMask = obstacleLayer | environementalLootLayer;
        Collider2D hit = Physics2D.OverlapBox(centerTarget, size, 0f, blockingMask);
        return hit != null;
    }

    private bool IsObstacleOrLootAtPosition(Vector2 position)
    {
        Vector2 snappedPos = SnapToGrid(position);
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

    void TriggerIdleFromFacing()
    {
        ResetIdleTriggers();
        ResetMovementTriggers();

        if (facingDirection == Vector2.up) anim.SetTrigger("IdleUp");
        else if (facingDirection == Vector2.down) anim.SetTrigger("IdleDown");
        else if (facingDirection == Vector2.left) anim.SetTrigger("IdleLeft");
        else if (facingDirection == Vector2.right) anim.SetTrigger("IdleRight");

        wasAnimatingWalk = false;
        lastAnimDirection = facingDirection;
    }

    void SetFacingDirection(Vector2 direction)
    {
        facingDirection = direction;
    }

    public float GetDistanceTraveled() => distanceTraveled;
    public bool GetCanMove() => canMove;
    public void SetCanMove(bool value) => canMove = value;
}
