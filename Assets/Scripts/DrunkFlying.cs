using System.Collections;
using UnityEngine;

public class DrunkFlying : MonoBehaviour
{
    private Transform player;
    [SerializeField] private float maintainDistanceX = 5f;
    [SerializeField] private float maintainDistanceY = 3f;
    [SerializeField] private float drunkFlyDuration = 3f; // Duration to fly in a "drunk" way
    [SerializeField] private float drunkFlySpeed = 4f; // Speed of flying erratically
    [SerializeField] private float attackInterval = 5f; // Time before flying towards the player to attack
    [SerializeField] private float destroyAfterSeconds = 10f; // Time after starting the attack to destroy the enemy

    private bool isFlyingErratically = false;
    private Vector3 targetPosition;
    public bool isActive = false;

    private Vector3 attackTargetPosition;
    private bool hasAttacked = false;
    private Vector3 velocity = Vector3.zero;
    public bool isPaused = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
    }

    void Update()
    {
        if (isPaused)
        {
            return;
        }

        if (player != null)
        {
            if (isActive)
            {
                StartCoroutine(FlyRoutine());

                if (!hasAttacked)
                {
                    if (!isFlyingErratically)
                    {
                        // Calculate the target position to maintain distance in front of the player
                        Vector3 direction = (player.position - transform.position).normalized;
                        targetPosition = player.position - direction * Mathf.Min(maintainDistanceX, maintainDistanceY);
                        targetPosition = new Vector3(
                            player.position.x + Mathf.Sign(transform.position.x - player.position.x) * maintainDistanceX,
                            player.position.y + Mathf.Sign(transform.position.y - player.position.y) * maintainDistanceY,
                            transform.position.z
                        );

                        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 0.3f);
                    }
                    else
                    {
                        // Fly erratically
                        transform.position += new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized * drunkFlySpeed * Time.deltaTime;
                    }
                }
            }
        }
        else
        {
            player = GameObject.FindWithTag("Player").transform;
        }
    }

    IEnumerator FlyRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval);
            while (isPaused)
            {
                yield return null;
            }

            isFlyingErratically = true;
            yield return new WaitForSeconds(drunkFlyDuration);
            while (isPaused)
            {
                yield return null;
            }

            isFlyingErratically = false;

            if (!hasAttacked)
            {
                // Get the player's position right before attacking
                attackTargetPosition = player.position;
                hasAttacked = true;
                StartCoroutine(AttackPlayer());
                StartCoroutine(DestroyAfterAttack());
            }
        }
    }

    IEnumerator AttackPlayer()
    {
        float attackSpeed = drunkFlySpeed; // Use the same speed for attack
        bool moveAlongY = true;

        while (true)
        {
            while (isPaused)
            {
                yield return null;
            }

            if (moveAlongY)
            {

                // Move along the y-axis towards the player's position
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, attackTargetPosition.y, transform.position.z), attackSpeed * Time.deltaTime);
                if (Mathf.Approximately(transform.position.y, attackTargetPosition.y))
                {
                    moveAlongY = false;
                }
            }
            else
            {
                // Move along the x-axis towards the player's position
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(-100, transform.position.y, transform.position.z), attackSpeed * Time.deltaTime);
            }
            yield return null;
        }
    }

    IEnumerator DestroyAfterAttack()
    {
        float elapsed = 0f;

        while (elapsed < destroyAfterSeconds)
        {
            while (isPaused)
            {
                yield return null;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}