using UnityEngine;
using System.Collections;

public class Shot : MonoBehaviour
{
    public Transform target;
    public float speed = 5f;
    public bool isPaused = false;


    public void Init(Transform targetTransform)
    {
        target = targetTransform;
        StartCoroutine(AttackPlayer());
        StartCoroutine(DestroyAfterAttack());
    }

    IEnumerator AttackPlayer()
    {
        float attackSpeed = speed; // Use the same speed for attack
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
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, target.position.y, transform.position.z), attackSpeed * Time.deltaTime);
                if (Mathf.Approximately(transform.position.y, target.position.y))
                {
                    moveAlongY = false;
                }
            }
            else
            {
                // Move along the x-axis towards the player's position
                transform.Rotate(Vector3.forward * 10);
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(-100, transform.position.y, transform.position.z), attackSpeed * Time.deltaTime);

            }
            yield return null;
        }
    }

    IEnumerator DestroyAfterAttack()
    {

        float elapsed = 0f;

        while (elapsed < 5f)
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

    private void OnCollisionEnter2D(Collision2D other)
    {

        Destroy(gameObject);

    }
}