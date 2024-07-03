using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Flying : MonoBehaviour
{
    private Transform player; 
    [SerializeField] private float maintainDistanceX = 5f;
    [SerializeField] private float maintainDistanceY = 3f;
    [SerializeField] private float destroyAfterSeconds = 10f; // Time after starting the attack to destroy the enemy

    private Vector3 targetPosition;

    private Vector3 attackTargetPosition;
    private Vector3 velocity = Vector3.zero; // For smooth damp
    public bool isActive = false;

void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform; // Find the player by tag
    }

    void Update()
    {
        if(player != null){
            if (isActive)
            {
                    Vector3 direction = (player.position - transform.position).normalized;
                    targetPosition = player.position - direction * Mathf.Min(maintainDistanceX, maintainDistanceY);
                    targetPosition = new Vector3(
                        player.position.x + Mathf.Sign(transform.position.x - player.position.x) * maintainDistanceX,
                        player.position.y + Mathf.Sign(transform.position.y - player.position.y) * maintainDistanceY,
                        transform.position.z
                    );

                    transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 0.3f);        
                    GetComponent<Weapon>().Attack(player);  // Ensure Weapon script is attached to the same object

            }
        }else{
            player = GameObject.FindWithTag("Player")?.transform;
        }
    }


   
}