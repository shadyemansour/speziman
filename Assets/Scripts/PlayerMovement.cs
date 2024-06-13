using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public CharacterController2D controller;

	[SerializeField] private float runSpeed;
    [SerializeField] private float boostSpeed = 40f;
	[SerializeField] private float defaultSpeed = 30f;
    [SerializeField] private float defaultJumpForce = 400f;
    [SerializeField] private float boostJumpForce = 500f;
    [SerializeField] private float boostAnimationSpeed = 1.5f;



	float horizontalMove = 0f;
	bool jump = false;
    public Animator anim;    
    void Start() {
        runSpeed = defaultSpeed;    
    }
	
	// Update is called once per frame
	void Update () {

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


    public void Boost(float length) {
        StartCoroutine(BoostDuration(length));
    }

     private IEnumerator BoostDuration(float length) {
        // Set boosted parameters
        runSpeed = boostSpeed;
        controller.SetJumpForce(boostJumpForce);
        SetAnimationSpeed(boostAnimationSpeed);


        // Wait for 'length' seconds
        yield return new WaitForSeconds(length);

        // Reset parameters to normal
        runSpeed = defaultSpeed;
        SetAnimationSpeed(1);
        controller.SetJumpForce(defaultJumpForce);
    }


}