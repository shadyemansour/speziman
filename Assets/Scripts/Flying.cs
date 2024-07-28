using UnityEngine;

public class Flying : MonoBehaviour
{
    private Transform player;
    [SerializeField] private float maintainDistanceX = 5f;
    [SerializeField] private float maintainDistanceY = 3f;

    private Vector3 targetPosition;

    private Vector3 velocity = Vector3.zero; // For smooth damp
    public bool isActive = false;
    private Weapon weapon;

    public bool isPaused = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        weapon = GetComponent<Weapon>();
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
                MoveTowardsPlayer();
                if (IsVisibleToCamera()) weapon.Attack(player);
            }
        }
        else
        {
            // If the player was lost (e.g., respawn), find again
            player = GameObject.FindWithTag("Player")?.transform;
        }
    }

    private bool IsVisibleToCamera()
    {
        var camera = Camera.main; // Make sure the main camera is set correctly in tag
        Vector3 viewportPosition = camera.WorldToViewportPoint(transform.position);
        bool isVisible = viewportPosition.z > 0 && viewportPosition.x > 0 && viewportPosition.x < 1 && viewportPosition.y > 0 && viewportPosition.y < 1;
        return isVisible;
    }

    private void MoveTowardsPlayer()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        float adaptiveDampTime = Mathf.Clamp(distance / 5, 0.3f, 1f);  // Adjust these values based on desired responsiveness

        float horizontalDirection = Mathf.Sign(player.position.x - transform.position.x);
        float verticalDirection = Mathf.Sign(player.position.y - transform.position.y);

        targetPosition = new Vector3(
            player.position.x - horizontalDirection * maintainDistanceX,
            player.position.y - verticalDirection * maintainDistanceY,
            transform.position.z
        );

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, adaptiveDampTime);
    }
}