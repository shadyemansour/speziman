
using System;
using UnityEngine;

public class PlayerRotator : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                other.transform.rotation = Quaternion.Euler(0, 0, -90);
            
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                other.transform.rotation = Quaternion.Euler(0, 0, 0);

            }
        }
    }
}
