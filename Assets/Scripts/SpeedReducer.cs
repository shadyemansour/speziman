
using System;
using UnityEngine;

public class SpeedReducer : MonoBehaviour
{
    public float speed = 3f; 
    public float jump = 100f; 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                Debug.Log(gameObject.name.ToLower());
                if (gameObject.name.ToLower().Contains("water")){
                    other.transform.rotation = Quaternion.Euler(0, 0, -90);
                    playerMovement.inWater = true;
                } 
                playerMovement.ReduceSpeed(speed, jump);
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
                if (gameObject.name.ToLower().Contains("water")){
                    other.transform.rotation = Quaternion.Euler(0, 0, 0);
                    playerMovement.inWater = false;
                } 

                playerMovement.ResetSpeed();
            }
        }
    }
}
