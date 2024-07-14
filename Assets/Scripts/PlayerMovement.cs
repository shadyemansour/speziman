// Adaapted from https://github.com/Brackeys/2D-Movement/tree/master/2D%20Movement/Assets

using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public enum MovementState { Normal, Mud, Swim }
    public enum BoostState { None, Boosted }

    [Header("Speed Settings")]
    [SerializeField] private float defaultSpeed = 30f;
    [SerializeField] private float mudSpeed = 3f;
    [SerializeField] private float swimSpeed = 10f;
    [SerializeField] private float boostMultiplier = 1.5f;

    [Header("Jump Settings")]
    [SerializeField] private float defaultJumpForce = 400f;
    [SerializeField] private float mudJumpForce = 100f;
    [SerializeField] private float waterJumpForce = 220f;
    [SerializeField] private float boostJumpMultiplier = 1.5f;

    [Header("Animation Speeds")]
    [SerializeField] private float boostAnimationSpeed = 1.5f;
    [SerializeField] private float reducedAnimationSpeed = .5f;

    private CharacterController2D controller;
    private Animator anim;
    private Action disableBoostCallback;
    private MovementState currentMovementState = MovementState.Normal;
    private BoostState currentBoostState = BoostState.None;
    private float speedFactor = 1f;
    private bool isStopped = false;
    private float horizontalMove = 0f;
    private bool jump = false;

    void Start()
    {
        controller = GetComponent<CharacterController2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isStopped)
        {
            horizontalMove = 0;
            return;
        }

        float speed = GetSpeed();
        horizontalMove = Input.GetAxisRaw("Horizontal") * speed;
        anim.SetFloat("speed", Mathf.Abs(horizontalMove));
        float jf = GetJumpForce();
        if (controller.GetJumpForce() != jf) controller.SetJumpForce(jf);

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            anim.SetBool("isJumping", true);
        }

    }

    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, jump);
        jump = false;
    }

    private float GetSpeed()
    {
        float baseSpeed = defaultSpeed;
        switch (currentMovementState)
        {
            case MovementState.Mud:
                baseSpeed = mudSpeed;
                break;
            case MovementState.Swim:
                baseSpeed = swimSpeed;
                break;
        }
        return baseSpeed * (currentBoostState == BoostState.Boosted ? boostMultiplier : 1f);
    }

    private float GetJumpForce()
    {
        float baseJumpForce = defaultJumpForce;
        switch (currentMovementState)
        {
            case MovementState.Mud:
                baseJumpForce = mudJumpForce;
                break;
            case MovementState.Swim:
                baseJumpForce = waterJumpForce;
                break;
        }
        return baseJumpForce * (currentBoostState == BoostState.Boosted ? boostJumpMultiplier : 1f);
    }

    public void SetMovementState(MovementState state)
    {
        currentMovementState = state;
        controller.SetJumpForce(GetJumpForce());
    }

    public void Boost(float length, Action onComplete)
    {
        SetBoostState(BoostState.Boosted, length);
        disableBoostCallback = onComplete;
    }

    public void SetBoostState(BoostState state, float duration)
    {
        currentBoostState = state;
        if (state == BoostState.Boosted)
        {
            SetAnimationSpeed(boostAnimationSpeed);
            CancelInvoke(nameof(ClearBoost));
            Invoke(nameof(ClearBoost), duration);
        }
    }

    public MovementState GetMovementState()
    {
        return currentMovementState;
    }

    private void ClearBoost()
    {
        SetBoostState(BoostState.None, 0);
        SetAnimationSpeed(1);
        Debug.Log("Boost cleared");
        if (disableBoostCallback != null)
        {
            disableBoostCallback.Invoke();
            disableBoostCallback = null;
        }
    }

    public void onLanding()
    {
        anim.SetBool("isJumping", false);
    }

    public void SetAnimationSpeed(float speed)
    {
        anim.speed = speed;
    }

    public void ReduceSpeed(bool isMud)
    {
        SetMovementState(isMud ? MovementState.Mud : MovementState.Swim);
        anim.speed = reducedAnimationSpeed;
    }

    public void ResetSpeed(bool isDead = false)
    {
        SetMovementState(MovementState.Normal);
        float animSpeed = BoostState.Boosted == currentBoostState ? boostAnimationSpeed : 1;
        SetAnimationSpeed(animSpeed);
        if (isDead && disableBoostCallback != null)
        {
            ClearBoost();
        }
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

    public void SetIsStopped(bool value)
    {
        isStopped = value;
        controller.SetIsStopped(value);
    }



}