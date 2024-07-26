
using UnityEngine;

public class Water : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("WaterCheck"))
        {
            CharacterController2D playercontroller = other.transform.parent.GetComponent<CharacterController2D>();
            PlayerMovement playerMovement = other.transform.parent.GetComponent<PlayerMovement>();

            if (playercontroller != null)
            {
                playerMovement.ReduceSpeed(false);
                playercontroller.SetInWater(true);

            }
        }
    }



    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("WaterCheck"))
        {
            CharacterController2D playercontroller = other.transform.parent.GetComponent<CharacterController2D>();
            PlayerMovement playerMovement = other.transform.parent.GetComponent<PlayerMovement>();
            if (playercontroller != null)
            {
                playerMovement.ResetSpeed();
                playercontroller.SetInWater(false);

            }
        }
    }

}
