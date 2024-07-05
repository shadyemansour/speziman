using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public void Die()
    {
        SoundManager.Instance.PlayRandomDieSound();
        GameManager.Instance.RespawnPlayer(gameObject);
    }

}
