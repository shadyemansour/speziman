using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public CharacterController2D controller;

	[SerializeField] private float runSpeed;
    [SerializeField] private float boostSpeed = 40f;
    [SerializeField] private float swimSpeed = 10f;
    [SerializeField] private float mudSpeed = 3f;
	[SerializeField] private float defaultSpeed = 30f;
    [SerializeField] private float defaultJumpForce = 400f;
    [SerializeField] private float mudJumpForce = 100f;
    [SerializeField] private float waterJumpForce = 250f;
    [SerializeField] private float boostJumpForce = 500f;
    [SerializeField] private float boostAnimationSpeed = 1.5f;
    [SerializeField] private float reducedAnimationSpeed = .5f;
    private Action disableBoostCallback;
    private Rigidbody2D rb;
    public bool isStopped = false;



	float horizontalMove = 0f;
	bool jump = false;
    public Animator anim;    
    void Start() {
        runSpeed = defaultSpeed;   
        rb = GetComponent<Rigidbody2D>(); 
    }
	
	// Update is called once per frame
	void Update () {
        if (isStopped){
            horizontalMove = 0;
            return;
        }

		horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        anim.SetFloat("speed", Mathf.Abs(horizontalMove));

		if (Input.GetButtonDown("Jump"))
		{
			jump = true;
            anim.SetBool("isJumping", true);
        }

	}

    void FixedUpdate ()
	{
		// Move our character
		controller.Move(horizontalMove * Time.fixedDeltaTime, jump);
		jump = false;
	}


    public void onLanding() {
        anim.SetBool("isJumping", false);
    }

    public void SetAnimationSpeed(float speed)
    {
        anim.speed = speed;
    }


    public void Boost(float length, Action onComplete) {
        StartCoroutine(BoostDuration(length, onComplete));
    }

     private IEnumerator BoostDuration(float length, Action onComplete) {
        // Set boosted parameters
        runSpeed = boostSpeed;
        controller.SetJumpForce(boostJumpForce);
        SetAnimationSpeed(boostAnimationSpeed);
        disableBoostCallback = onComplete;


        // Wait for 'length' seconds
        yield return new WaitForSeconds(length);
        onComplete?.Invoke();
        disableBoostCallback = null;
        ResetSpeed();
    }

    public void ReduceSpeed(bool isMud) {
        if (isMud) {
            runSpeed = mudSpeed;
            controller.SetJumpForce(mudJumpForce);
        } else {
            runSpeed = swimSpeed;
            controller.SetJumpForce(waterJumpForce);
        }
        anim.speed = reducedAnimationSpeed;

    }

    public void ResetSpeed() {
        if (disableBoostCallback!=null) disableBoostCallback?.Invoke();
        runSpeed = defaultSpeed;
        SetAnimationSpeed(1);
        controller.SetJumpForce(defaultJumpForce);
    }

  
    // todo to be called when needed
    public void IncrementScore(int amount)
    {
        GameManager.Instance.score += amount;
    }

    public void IncrementDeliveries()
    {
        GameManager.Instance.UpdateDeliveries();
    }

    public void SetIsStopped(bool value) {
        isStopped = value;
        controller.SetIsStopped(value);
    }   

}