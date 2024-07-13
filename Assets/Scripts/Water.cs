
using System.Collections;
using UnityEngine;

public class Water : MonoBehaviour
{
    bool coolOff = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.tag);
        if (other.CompareTag("WaterCheck") && !coolOff)
        {
            CharacterController2D playercontroller = other.transform.parent.GetComponent<CharacterController2D>();
            PlayerMovement playerMovement = other.transform.parent.GetComponent<PlayerMovement>();

            if (playercontroller != null)
            {
                playerMovement.ReduceSpeed(false);
                playercontroller.SetInWater(true);
                coolOff = true;
                StartCoroutine(WaitXSeconds(0.2f));

            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {

        if (other.CompareTag("WaterCheck") && !coolOff)
        {
            CharacterController2D playercontroller = other.transform.parent.GetComponent<CharacterController2D>();
            PlayerMovement playerMovement = other.transform.parent.GetComponent<PlayerMovement>();

            if (playercontroller != null)
            {
                if (playerMovement.GetMovementState() != PlayerMovement.MovementState.Swim)
                {
                    playerMovement.ReduceSpeed(false);
                    playercontroller.SetInWater(true);
                    coolOff = true;
                    StartCoroutine(WaitXSeconds(0.2f));
                }

            }
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("WaterCheck") && !coolOff)
        {
            CharacterController2D playercontroller = other.transform.parent.GetComponent<CharacterController2D>();
            PlayerMovement playerMovement = other.transform.parent.GetComponent<PlayerMovement>();
            if (playercontroller != null)
            {
                playerMovement.ResetSpeed();
                playercontroller.SetInWater(false);
                coolOff = true;
                StartCoroutine(WaitXSeconds(0.2f));
            }
        }
    }
    private IEnumerator WaitXSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        coolOff = false;
    }
}
