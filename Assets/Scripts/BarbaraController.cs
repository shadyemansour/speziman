using System.Collections;
using UnityEngine;

public class BarbaraController : MonoBehaviour
{
    private Animator animator;
    private System.Action OnAnimationFinished;
    public bool sayingBye = false;
    private Sprite initialSprite;

    void Start()
    {
        animator = GetComponent<Animator>();
        initialSprite = GetComponent<SpriteRenderer>().sprite;

    }

    void Update()
    {
        if (sayingBye)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Quaternion targetRotation;
            if (player.transform.position.x < transform.position.x)
            {
                targetRotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                targetRotation = Quaternion.Euler(0, 0, 0);
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5);
        }
    }

    public void Wave()
    {
        sayingBye = true;
        animator.SetTrigger("Wave");
    }

    public void WaveNoPos()
    {
        sayingBye = true;
        animator.SetTrigger("WaveNoPos");
    }

    public void StopWaveNoPos()
    {
        sayingBye = false;
        animator.enabled = false;
        GetComponent<SpriteRenderer>().sprite = initialSprite;

    }

    public void WaveAndMove(System.Action callback)
    {
        Debug.Log("WaveAndMove called");
        OnAnimationFinished = callback;
        animator.SetTrigger("Move");

        float animationDuration = GetAnimationDuration("BarbaraWaveMove");
        StartCoroutine(WaitForAnimation(animationDuration, true));
    }

    IEnumerator WaitForAnimation(float time, bool isWave = false)
    {
        Debug.Log($"Waiting for {time} seconds for the animation to complete.");
        yield return new WaitForSeconds(time);
        Debug.Log("Animation wait time completed. Executing callback.");
        OnAnimationFinished?.Invoke();
    }

    private float GetAnimationDuration(string animationName)
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++)
        {
            if (ac.animationClips[i].name == animationName)
            {
                return ac.animationClips[i].length;
            }
        }
        Debug.LogWarning("Animation name not found in the animator clips.");
        return 0f;
    }

    public void StartDrinking(System.Action callback)
    {
        OnAnimationFinished = callback;
        animator.SetTrigger("Drink");
        float animationDuration = GetAnimationDuration("BarbaraDrink");
        StartCoroutine(WaitForAnimation(animationDuration));

    }
    public void MoveAway()
    {
        StartCoroutine(MoveToTargetCoroutine());

    }

    IEnumerator MoveToTargetCoroutine()
    {

        Vector3 targetPosition = transform.position.x > 0 ? new Vector3(10f, transform.position.y, 0) : new Vector3(-7f, transform.position.y, 0);
        while (transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, 0.005f);
            yield return null;
        }
    }

}
