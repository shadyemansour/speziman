using UnityEngine;
using System.Collections;
using UnityEditor.Timeline;

public class Shot : MonoBehaviour
{
     public Transform target;
    public float speed = 5f;

    public void Init(Transform targetTransform)
    {
        target = targetTransform;
        StartCoroutine(AttackPlayer());
    }

    IEnumerator AttackPlayer()
    {
        float attackSpeed = speed; // Use the same speed for attack
        bool moveAlongY = true;

        while (true)
        {
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
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
}