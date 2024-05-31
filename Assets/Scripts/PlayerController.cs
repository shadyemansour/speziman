using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask platformsLayerMask;
    private Rigidbody2D rb;
    private BoxCollider2D bc;
    [SerializeField] private float jumpVelocity = 2f;
    private float moveSpeed= 5f;
    private Animator anim;
    private bool grounded = true;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

        if (grounded && Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }

        else if (horizontalInput > 0.01f && CanMoveRight())
        {
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
            transform.localScale =  Vector3.one;
        }
        else if (horizontalInput < -0.01f && CanMoveLeft()){
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
            transform.localScale = new Vector3(-1, 1, 1); 
        }

        anim.SetBool("grounded", grounded); 
        anim.SetBool("run", horizontalInput != 0); 

    
    }

    private bool CanMoveRight()
    {
        RaycastHit2D raycastHit2D = Physics2D.Raycast(bc.bounds.center, Vector2.right , 0.53f, platformsLayerMask);
        return (raycastHit2D.collider == null);
    }

    private bool CanMoveLeft()
    {
        RaycastHit2D raycastHit2D = Physics2D.Raycast(bc.bounds.center, Vector2.left , 0.53f, platformsLayerMask);
        return (raycastHit2D.collider == null); 
    }

    private void Jump()
    {
        // SoundManager.PlayJumpSound();
        rb.velocity = Vector2.up * jumpVelocity;
        grounded = false;
        anim.SetTrigger("jump");
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            grounded = true;
        }
    }
}