using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public void Die()
    {
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        playerMovement.ResetSpeed(true);
        SoundManager.Instance.PlayRandomDieSound();
        GameManager.Instance.RespawnPlayer(gameObject);
    }

}
