using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostCharacter : MonoBehaviour
{

    [SerializeField] private float boostLength = 20f;	 



private void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            PlayerMovement player = collider.gameObject.GetComponent<PlayerMovement>();
            if (player != null)
            {
                SoundManager.Instance.PlaySound("boost");
                // Here you might want to do additional checks or apply effects
                player.Boost(boostLength);  // Call the die method
                Destroy(gameObject); 

            }
        }
    }
}
