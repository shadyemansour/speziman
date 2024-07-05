using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarbaraController : MonoBehaviour
{
    private Animator animator;
    private System.Action OnAnimationFinished;
    public bool sayingBye = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        
    }

  void Update() {
        if (sayingBye) {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
           Quaternion targetRotation;
            if(player.transform.position.x < transform.position.x) {
                targetRotation = Quaternion.Euler(0, 180, 0);
            } else {
                targetRotation = Quaternion.Euler(0, 0, 0);
            }

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5); 
        }
    }   

    public void Wave(){
        sayingBye = true;
        animator.SetTrigger("Wave");
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

}
