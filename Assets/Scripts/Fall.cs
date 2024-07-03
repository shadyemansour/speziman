using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;


public class Fall : MonoBehaviour
{
    public float shakeAmount = 0.01f; // Amplitude of the shake
    public float shakeDuration = 1f; // Duration of the initial shake in seconds
    public float delayBeforeFall = 2f; // Delay before starting to fall

    private bool isShaking = false;
    private Vector3 originalPosition;
    private int counter = 0;

  void Start()
    {
        originalPosition = transform.position;
    }

    void FixedUpdate()
    {

        if (isShaking )
        {
            if (counter % 5 == 0)
            {
                Shake();
            }
            counter++;}
    }
    public void StartFall()
    {
        StartCoroutine(StartFallCo());
        

    }

    
    IEnumerator StartFallCo()
    {
        
        isShaking = true;
        yield return new WaitForSeconds(shakeDuration);

        // Stop shaking and prepare for fall
        isShaking = false;
        transform.position = originalPosition; // Reset position to prevent drift
        // yield return new WaitForSeconds(delayBeforeFall - shakeDuration);

        // Start falling
        GetComponent<Rigidbody2D>().gravityScale = 0.7f;
    }

    private void Shake()
    {
        float x = Random.Range(-1f, 1f) * shakeAmount;
        float y = Random.Range(-1f, 1f) * shakeAmount;
        transform.position = originalPosition + new Vector3(x, y, 0);
    }
}
