using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }


    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            // get gameobject name
            string name = gameObject.name.Split(' ')[0].Trim();
            Sprite sprite = Resources.Load<Sprite>("Sprites/" + name);
            SoundManager.Instance.PlaySound("checkpoint");
            GetComponent<SpriteRenderer>().sprite = sprite;
            GameManager.Instance.UpdateLastCheckpoint(transform.position);
        }
    }
}
