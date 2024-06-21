using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class BoostCharacter : MonoBehaviour
{

    [SerializeField] private float boostLength = 20f;
    private GameObject uiImage;

    void Awake()
    {
        uiImage = GameObject.FindGameObjectWithTag("BoostUI");
        uiImage.GetComponent<CanvasGroup>().alpha = 0;
    }

    private void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            PlayerMovement player = collider.gameObject.GetComponent<PlayerMovement>();
            if (player != null)
            {
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(gameObject.transform.position);
                uiImage.transform.position = screenPosition;
                uiImage.GetComponent<CanvasGroup>().alpha = 1;
                var uiAnimator = uiImage.GetComponent<Animator>();
                uiAnimator.SetTrigger("Collect");
                SoundManager.Instance.PlaySound("boost");
                // Here you might want to do additional checks or apply effects
                player.Boost(boostLength, DisableImage);  // Call the die method
                Destroy(gameObject); 

            }
        }
    }
    



    public void DisableImage()
    {
        uiImage.GetComponent<CanvasGroup>().alpha = 0;
    }
}
