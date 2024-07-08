using UnityEngine;

public class TruckEnemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float moveDistance = 5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private string playerTag = "Player";

    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.9f, 0.1f);
    [SerializeField] private float obstacleCheckDistance = 0.5f;
    [SerializeField] private Vector2 obstacleCheckSize = new Vector2(0.1f, 0.9f);
    private Rigidbody2D rb;


    private Vector3 startPosition;
    private bool movingRight = true;
    private SpriteRenderer spriteRenderer;

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

    private void Update()
    {
        Move();
        CheckDirection();
    }

    private void Move()
    {
        if (!IsGrounded())
        {
            ChangeDirection();
        }
        else if (IsObstacleAhead())
        {
            ChangeDirection();
        }

        float distanceMoved = transform.position.x - startPosition.x;

        if (distanceMoved >= moveDistance && movingRight)
        {
            ChangeDirection();
        }
        else if ((-1)*distanceMoved >= moveDistance && !movingRight)
        {
            ChangeDirection();
        }

        float movement = movingRight ? moveSpeed : -moveSpeed;
        rb.velocity = new Vector2(movement, rb.velocity.y);

    }
    private void CheckDirection()
    {
        if (spriteRenderer != null)
        {
            // Flip the sprite based on the movement direction
            bool newDirection = !movingRight;
            spriteRenderer.flipX = newDirection;
        }
    }
    private void ChangeDirection()
    {
        movingRight = !movingRight;
        Vector3 newPosition = transform.position;
        newPosition.x = Mathf.Clamp(newPosition.x, startPosition.x - moveDistance, startPosition.x + moveDistance);
        transform.position = newPosition;

        // Immediately update the sprite direction
        CheckDirection();
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

    private bool IsGrounded()
    {
        Vector2 boxCastOrigin = (Vector2)transform.position + Vector2.down * (GetComponent<Collider2D>().bounds.extents.y - groundCheckDistance / 2);
        RaycastHit2D hit = Physics2D.BoxCast(boxCastOrigin, groundCheckSize, 0f, Vector2.down, groundCheckDistance, groundLayer);
        return hit.collider != null;
    }



    private bool IsObstacleAhead()
    {
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;
        Vector2 boxCastOrigin = (Vector2)transform.position + direction * (GetComponent<Collider2D>().bounds.extents.x - obstacleCheckDistance / 2);
        RaycastHit2D hit = Physics2D.BoxCast(boxCastOrigin, obstacleCheckSize, 0f, direction, obstacleCheckDistance, obstacleLayer);
        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector2 boxCastOrigin = (Vector2)transform.position + Vector2.down * (GetComponent<Collider2D>().bounds.extents.y - groundCheckDistance / 2);
        Gizmos.DrawWireCube(boxCastOrigin, groundCheckSize);

        Gizmos.color = Color.red;
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;
        Vector2 obstacleBoxCastOrigin = (Vector2)transform.position + direction * (GetComponent<Collider2D>().bounds.extents.x - obstacleCheckDistance / 2);
        Gizmos.DrawWireCube(obstacleBoxCastOrigin, obstacleCheckSize);
    }
}