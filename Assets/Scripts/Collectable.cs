using UnityEngine;

public class Collectable : MonoBehaviour
{
    public string itemType;  // "Cola" or "Orange"
    public bool collectedAfterCheckpoint;  // True if collected after the last checkpoint
    private Vector3 startPosition;
    public int scoreValue = 10;  // Default score value, can be adjusted in inspector

    private void Start()
    {
        startPosition = transform.position;  // Save the initial position
        collectedAfterCheckpoint = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SoundManager.Instance.PlaySound("collect" + itemType);
            GameManager.Instance.CollectItem(this);
            collectedAfterCheckpoint = true;
            
            PlayerMovement player = collision.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.IncrementScore(scoreValue);
            }
            
            gameObject.SetActive(false);  // Instead of destroying, just deactivate
        }
    }

    public void ResetCollectable()
    {
        transform.position = startPosition;  // Reset position if needed
        collectedAfterCheckpoint = false;
        gameObject.SetActive(true);
    }
}