using UnityEngine;

public class TruckEnemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private string playerTag = "Player";

    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.9f, 0.1f);
    [SerializeField] private float obstacleCheckDistance = 0.2f;
    [SerializeField] private Vector2 obstacleCheckSize = new Vector2(0.1f, 0.9f);
    private Rigidbody2D rb;


    private Vector3 startPosition;
    private bool movingRight = true;
    private SpriteRenderer spriteRenderer;
    public bool isPaused = false;


    private void Start()
    {
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found on TruckEnemy!");
        }
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on TruckEnemy!");
        }
    }


    void FixedUpdate()
    {
        if (!isPaused)
        {
            Move();
            CheckDirection();
        }
    }

    private void Move()
    {

        if (!IsGrounded() || IsObstacleAhead())
        {
            ChangeDirection();
        }

        float movement = movingRight ? moveSpeed : -moveSpeed;
        rb.velocity = new Vector2(movement, rb.velocity.y);
        if (gameObject.name == "TruckEnemy")
        {
            Debug.Log("IsGrounded: " + IsGrounded() +
                             ", IsObstacleAhead: " + IsObstacleAhead() +
                             ", Movement: " + movement +
                             ", Velocity: " + rb.velocity +
                             ", Position: " + rb.position +
                             ", Collider Bounds: " + GetComponent<Collider2D>().bounds);
        }
    }

    private void CheckDirection()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !movingRight;
        }
    }

    private void ChangeDirection()
    {
        movingRight = !movingRight;
        CheckDirection();
    }

    private bool IsGrounded()
    {
        float frontOffset = movingRight ? GetComponent<Collider2D>().bounds.extents.x : -GetComponent<Collider2D>().bounds.extents.x;
        Vector2 boxCastOrigin = (Vector2)transform.position + new Vector2(frontOffset, 0) + Vector2.down * (GetComponent<Collider2D>().bounds.extents.y - groundCheckDistance / 2);

        RaycastHit2D hit = Physics2D.BoxCast(boxCastOrigin, groundCheckSize, 0f, Vector2.down, groundCheckDistance, groundLayer);
        return hit.collider != null;
    }

    private bool IsObstacleAhead()
    {
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;
        Vector2 boxCastOrigin = (Vector2)transform.position + direction * (GetComponent<Collider2D>().bounds.extents.x + obstacleCheckDistance);
        RaycastHit2D hit = Physics2D.BoxCast(boxCastOrigin, obstacleCheckSize, 0f, direction, obstacleCheckDistance, obstacleLayer);
        return hit.collider != null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.Die();
            }
            else
            {
                Debug.LogWarning("Player object does not have a PlayerController component!");
            }
        }
    }

    private void OnDrawGizmos()
    {
        DrawGroundCheckGizmo();
        DrawObstacleCheckGizmo();
    }

    private void DrawGroundCheckGizmo()
    {
        float frontOffset = movingRight ? GetComponent<Collider2D>().bounds.extents.x : -GetComponent<Collider2D>().bounds.extents.x;
        Vector2 boxCastOrigin = (Vector2)transform.position + new Vector2(frontOffset, 0) + Vector2.down * (GetComponent<Collider2D>().bounds.extents.y - groundCheckDistance / 2);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(boxCastOrigin, new Vector3(groundCheckSize.x, groundCheckSize.y, 1f));
    }

    private void DrawObstacleCheckGizmo()
    {
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;
        Vector2 boxCastOrigin = (Vector2)transform.position + direction * (GetComponent<Collider2D>().bounds.extents.x + obstacleCheckDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCastOrigin, new Vector3(obstacleCheckSize.x, obstacleCheckSize.y, 1f));
    }

    public void SetIsPaused(bool isPaused)
    {
        this.isPaused = isPaused;
        if (isPaused)
        {
            rb.velocity = Vector2.zero;
        }
    }

}