using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauliCopterController : MonoBehaviour
{
    Vector3 targetPosition = new Vector3(13.5f, 1.75f, 0);


    public void MoveToTarget()
    {
        StartCoroutine(MoveToTargetCoroutine());
    }

    IEnumerator MoveToTargetCoroutine()
    {
        while (transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, 0.003f);
            yield return null;
        }
    }
}
