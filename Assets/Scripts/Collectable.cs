using UnityEngine;

public class Collectable : MonoBehaviour
{
    public string itemType;  // "Cola" or "Orange"
    public bool collectedAfterCheckpoint;  // True if collected after the last checkpoint
    private Vector3 startPosition;
    public int scoreValue = 10;

    private void Start()
    {
        startPosition = transform.position;
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

            gameObject.SetActive(false);
        }
    }

    public void ResetCollectable()
    {
        transform.position = startPosition;
        collectedAfterCheckpoint = false;
        gameObject.SetActive(true);
    }
}