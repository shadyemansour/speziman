using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{

    public int damageAmount = 1;

private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            KillPlayer(other.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("OnTriggerEnter2D");
        if (collider.CompareTag("Player"))
        {
           KillPlayer(collider.gameObject);
        }
    }

    private void KillPlayer(GameObject go)
    {
        // Here you might want to do additional checks or apply effects
        PlayerController player = go.GetComponent<PlayerController>();
        if (player != null)
        {
            player.Die();  // Call the die method
        }
    }
}
