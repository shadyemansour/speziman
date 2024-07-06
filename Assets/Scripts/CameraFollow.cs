using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    private Camera cam;
    private float[] zoomSpeeds = { 0.45f, 0.6f };
    private float[] moveSpeeds = { 0.8f, 1.5f };
    public bool isFollowingPlayer;

    private Vector3[] stops = { new Vector3(2.7f, -0.55f, -10f), new Vector3(7.69f, -1.44f, -10f) };
    private float[] zooms = { 3.5f, 2.0f };
    public int currentStop = 0;
    void Start()
    {
        cam = Camera.main;
        isFollowingPlayer = false;

    }

    void LateUpdate()
    {
        if (isFollowingPlayer) FollowPlayer(currentStop);

    }

    void FollowPlayer(int stopNumber)
    {
        if (player == null) return;
        // Calculate the offset to keep the player at the bottom right of the screen
        cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, zooms[stopNumber], zoomSpeeds[stopNumber] * Time.deltaTime);
        cam.transform.position = Vector3.MoveTowards(cam.transform.position, stops[stopNumber], moveSpeeds[stopNumber] * Time.deltaTime);

    }
}
