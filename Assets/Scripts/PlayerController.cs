using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public void Die()
    {
        SoundManager.Instance.PlayRandomDieSound();
        GameManager.Instance.RespawnPlayer(gameObject);
    }

}
