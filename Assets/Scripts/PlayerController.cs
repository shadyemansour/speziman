using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
   public void Die()
    {
        SoundManager.Instance.PlaySound("die");
        GameManager.Instance.RespawnPlayer(gameObject);
    }
}
