using UnityEngine;
using System.Collections;

public class BottleKick : MonoBehaviour
{
    public float kickForceMagnitude = 500f; 
    public float upwardKickRatio = 0.2f;

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
        }

        IEnumerator DestroyAfterDelay()
        {
            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }
    }
}

}