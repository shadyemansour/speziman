using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauliCopterController : MonoBehaviour
{
    Vector3 targetPosition = new Vector3(13.5f, 1.75f, 0);
    private bool moveToTarget = false;


    void FixedUpdate()
    {

        if (moveToTarget)
        {

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, 0.1f);
            if (transform.position == targetPosition)
            {
                this.moveToTarget = false;
            }
        }

    }

    public void MoveToTarget()
    {
        moveToTarget = true;
    }


}
