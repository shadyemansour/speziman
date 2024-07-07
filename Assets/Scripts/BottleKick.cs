using UnityEngine;
using System.Collections;

public class BottleKick : MonoBehaviour
{
    public float kickForceMagnitude = 500f;
    public float upwardKickRatio = 0.2f;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Awake()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 kickDirection = (transform.position - collision.transform.position).normalized;
            kickDirection += Vector2.up * upwardKickRatio;
            kickDirection.Normalize();


            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(kickDirection * kickForceMagnitude);
                StartCoroutine(DestroyAfterDelay());
                Collectable collectable = GetComponent<Collectable>();
                if (collectable != null)
                {
                    collectable.Collect(collision.collider);
                }
            }

            IEnumerator DestroyAfterDelay()
            {
                yield return new WaitForSeconds(1f);
                gameObject.SetActive(false);
                transform.position = originalPosition;
                transform.rotation = originalRotation;
            }
        }
    }

}