using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelmutBarbaraSceneController : MonoBehaviour
{

    private Animator animator;
    private bool isWaliking = false;
    private Vector3 currentTarget;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private CameraFollow cam;
    private System.Action onComplete;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
    }

    void Update()
    {
        if (isWaliking)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentTarget, 1.5f* Time.deltaTime);

            if (Vector3.Distance(transform.position, currentTarget) < 0.1f)
            {
                isWaliking = false; 
                animator.enabled = false;
                spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/standing");
                onComplete?.Invoke();
            }
        }
  
    }


    public void WalkTo(Vector3 target, System.Action onComplete)
    {
        animator.enabled = true;
        animator.Play("HelmutBarbaraWalk");
        currentTarget = target;
        isWaliking = true;
        this.onComplete = onComplete;
    
    }

    public void ThrowCollected()
    {

    }


}
