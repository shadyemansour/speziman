using UnityEngine;

public class Collectable : MonoBehaviour
{
    public string itemType;  // "Cola" or "Orange"

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Triggered by: " + collision.name);


        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.CollectItem(itemType);
            Destroy(gameObject);  // Destroy the collectable after it's been collected
        }
    }
}
