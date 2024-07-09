using System;
using UnityEngine;

public class HelicopterController : MonoBehaviour
{
    [SerializeField] private float riseSpeed = 2f;
    [SerializeField] private float rotationSpeed = 40f;
    [SerializeField] private float riseToY = 0;
    private Vector3 finishLine;
    private Animator animator;
    private float moveSpeed = 5f;
    private bool readyToMove = false;
    private bool readyToRotate = false;
    private bool readyToDescend = false; float timeToArrive = 0f;
    private float originalY;
    public bool isPaused = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        GameObject finishLineObject = GameObject.FindGameObjectWithTag("FinishLine");
        if (finishLineObject != null)
        {
            finishLine = new Vector3(finishLineObject.transform.position.x + 1.5f, riseToY, finishLineObject.transform.position.z);
            originalY = transform.position.y;

        }
        else
        {
            Debug.LogError("Finish line object not found!");
            return;
        }
    }

    void Update()
    {
        if (!isPaused)
        {
            if (!readyToMove)
            {
                RiseToInitialPosition();
            }
            else if (!readyToRotate)
            {
                MoveTowardsFinishLine();
            }
            else if (!readyToDescend)
            {
                RotateHelicopter();
            }
            else
            {
                DescendToOriginalY();
            }
        }
    }
    void RiseToInitialPosition()
    {
        if (!Mathf.Approximately(transform.position.y, riseToY))
        {
            float newY = Mathf.MoveTowards(transform.position.y, riseToY, riseSpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
        else if (!Mathf.Approximately(NormalizeAngle(transform.eulerAngles.z), 350))
        {
            float targetZ = 350;
            float newZ = Mathf.MoveTowardsAngle(NormalizeAngle(transform.eulerAngles.z), targetZ, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, newZ);
        }
        else
        {
            CalculateMoveSpeed();
            readyToMove = true;
        }
    }

    void MoveTowardsFinishLine()
    {
        Vector3 direction = (new Vector3(finishLine.x, transform.position.y, finishLine.z) - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        if (Vector3.Distance(transform.position, finishLine) < 0.1f)
        {
            readyToRotate = true;
            readyToMove = false;
        }
    }

    void RotateHelicopter()
    {
        if (!Mathf.Approximately(NormalizeAngle(transform.eulerAngles.z), 0))
        {
            float targetZ = 0;
            float newZ = Mathf.MoveTowardsAngle(NormalizeAngle(transform.eulerAngles.z), targetZ, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, newZ);
        }
        else
        {
            float targetY = 180;
            float newY = Mathf.MoveTowardsAngle(NormalizeAngle(transform.eulerAngles.y), targetY, rotationSpeed * 3 * Time.deltaTime);
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, newY, 0);

            if (Mathf.Approximately(NormalizeAngle(transform.eulerAngles.y), targetY))
            {
                readyToDescend = true;
            }
        }
    }


    void DescendToOriginalY()
    {
        if (transform.position.y > originalY)
        {
            float newY = Mathf.MoveTowards(transform.position.y, originalY, riseSpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
        else
        {
            animator.enabled = false;
            Debug.Log("Mission Completed: Landed and Ready.");
            this.enabled = false;
        }
    }

    float NormalizeAngle(float angle)
    {
        angle = angle % 360;
        if (angle < 0)
            angle += 360;
        return angle;
    }

    private void CalculateMoveSpeed()
    {
        timeToArrive = Math.Max(GameManager.Instance.GetTimeLeft() - 15f, 20f);
        float distance = Mathf.Abs(finishLine.x - transform.position.x);
        moveSpeed = distance / timeToArrive;
    }
}
