using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private bool isFinalCheckpoint = false;
    private bool isreached = false;

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && !isreached) {
            isreached = true; 
            string name = gameObject.name.Split(' ')[0].Trim();
            Sprite sprite = Resources.Load<Sprite>("Sprites/" + name);
            SoundManager.Instance.PlaySound("checkpoint");
            GetComponent<SpriteRenderer>().sprite = sprite;
            GameManager.Instance.UpdateLastCheckpoint(transform.position);

            if (isFinalCheckpoint)
            {
                GameManager.Instance.TriggerLevelComplete();
                
            }
        }
    }
}