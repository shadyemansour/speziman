using UnityEngine;

public class FaxField : MonoBehaviour
{
    private Flying flying;
    void Start()
    {
        //find the child with the script flying
        flying = GetComponentInChildren<Flying>();
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

    public void SetIsPaused(bool isPaused)
    {
        flying.isPaused = isPaused;
    }
}
