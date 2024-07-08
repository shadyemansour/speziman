using UnityEngine;

public class AloisiusField : MonoBehaviour
{
    private DrunkFlying flying;
    void Start()
    {
        //find the child with the script flying
        flying = GetComponentInChildren<DrunkFlying>();
        flying.isActive = false;

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            flying.isActive = true;

        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            flying.isActive = false;
        }

    }
}
