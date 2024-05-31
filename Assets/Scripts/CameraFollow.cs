using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float smoothSpeed = 0.125f;
    public float verticalOffset = 0;

    private Transform player;
    private float minX;
    private float maxX;

    void Start()
    {
        SetupCameraLimits();
    }

    void SetupCameraLimits()
    {
        GameObject ground = GameObject.Find("Ground");
        if (ground)
        {
            BoxCollider2D groundCollider = ground.GetComponent<BoxCollider2D>();
            if (groundCollider)
            {
                float groundWidth = groundCollider.bounds.size.x;
                float leftEdge = groundCollider.bounds.min.x;
                float rightEdge = groundCollider.bounds.max.x;

                float cameraHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
                minX = leftEdge + cameraHalfWidth;
                maxX = rightEdge - cameraHalfWidth;
            }
        }
    }

    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        if (player != null)
        {
            Vector3 desiredPosition = new Vector3(player.position.x, transform.position.y + verticalOffset, transform.position.z);
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
         Debug.Log("Camera moved to: " + smoothedPosition);
        }
    }
}
