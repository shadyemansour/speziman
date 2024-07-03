using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class Helmut : MonoBehaviour
{
    private SpriteRenderer imageRenderer;
    [SerializeField] private GameObject bulb;
    private bool done = false;
    // Start is called before the first frame update
    void Awake()
    {
        //find the child with the script flying
      imageRenderer = GetComponent<SpriteRenderer>();
        
    }
    private void OnCollisionEnter2D(Collision2D collider)
    {
        Debug.Log("Collision");
        if (collider.gameObject.CompareTag("Orange") && !done)
        {
            
            Sprite newSprite = Resources.Load<Sprite>("Sprites/Story 3");
            Debug.Log("Sprite loaded");
            if(newSprite != null)
            {
                imageRenderer.sprite = newSprite;
            }
            else
            {
                Debug.LogError("sprite is null");
            }
            done = true;
            
        }
    }  

    public void Drink()
    {
            Sprite newSprite = Resources.Load<Sprite>("Sprites/Story 2");
            Debug.Log("Sprite loaded");
            if(newSprite != null)
            {
                imageRenderer.sprite = newSprite;
            }
            else
            {
                Debug.LogError("ImageRenderer is null");
            }    
    }    

        public void Idea()
    {
            Sprite newSprite = Resources.Load<Sprite>("Sprites/Story 1");
            Debug.Log("Sprite loaded");
            if(newSprite != null)
            {
                imageRenderer.sprite = newSprite;
            }
            else
            {
                Debug.LogError("ImageRenderer is null");
            }  
            bulb.SetActive(true);  
    }    

      public void WearGlasses()
    {
            bulb.SetActive(false);
            Sprite newSprite = Resources.Load<Sprite>("Sprites/Story 4");
            Debug.Log("Sprite loaded");
            if(newSprite != null)
            {
                imageRenderer.sprite = newSprite;
            }
            else
            {
                Debug.LogError("ImageRenderer is null");
            }  
    }    
}
