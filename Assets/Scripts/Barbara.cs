using UnityEngine;

public class Barbara : MonoBehaviour
{
    private bool isreached = false;
    Animator animator;
    public bool reachedAfterCheckpoint = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && !isreached)
        {
            bool canDeliver = GameManager.Instance.SendCollectables(other.gameObject, gameObject.transform.position);
            if (canDeliver)
            {
                animator.enabled = true;
                GameManager.Instance.IncrementDeliveries();
                reachedAfterCheckpoint = true;
                isreached = true;
                SoundManager.Instance.PlaySound("checkpoint");
                transform.rotation = Quaternion.Euler(0, 0, 0);
                animator.Play("BarbaraWinken");
            }
        }
    }

    void Update()
    {
        if (isreached)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Quaternion targetRotation;
            if (player.transform.position.x < transform.position.x)
            {
                targetRotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                targetRotation = Quaternion.Euler(0, 0, 0);
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5);
        }
    }

    public void Reset()
    {
        animator.enabled = false;
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Barbara_Grau");
        isreached = false;
        reachedAfterCheckpoint = false;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        GameManager.Instance.DecrementDeliveries();
    }
}
