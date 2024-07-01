using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .5f;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings

	const float k_GroundedRadius = 0.05f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	private bool m_InWater = false;            // Whether or not the player is in water.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	[SerializeField] private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
	private float jumpCooldown = 0.1f; // Cooldown duration in seconds
    private float jumpCooldownTimer;
	private bool m_IsStopped = false;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }


	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();
	}

	private void FixedUpdate()
	{
		if (!m_IsStopped)
		{
			bool wasGrounded = m_Grounded;
			m_Grounded = false;

			if (jumpCooldownTimer > 0)
			{
				jumpCooldownTimer -= Time.fixedDeltaTime;
			}
			else
			{
			// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
			// This can be done using layers instead but Sample Assets will not overwrite your project settings.
			Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
			for (int i = 0; i < colliders.Length; i++)
			{
				if (colliders[i].gameObject != gameObject)
				{
					m_Grounded = true;
					if (!wasGrounded)
						OnLandEvent.Invoke();
				}
			}
			}
		}
	}


	public void Move(float move, bool jump)
	{

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			 if (move > 0 && !m_FacingRight || move < 0 && m_FacingRight)
            {
                Flip();
            }
		}
		// If the player should jump...
		if (m_Grounded && jump)
		{
			// Add a vertical force to the player.
			m_Grounded = false;
			jumpCooldownTimer = jumpCooldown; // Start the cooldown
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
		}
		if (m_InWater && jump)
		{
			jumpCooldownTimer = jumpCooldown; // Start the cooldown
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce / 1.2f));
		}
	}


	private void Flip()
	{
		m_FacingRight = !m_FacingRight;
		if(m_InWater)
		{
			transform.Rotate(0, 180, 180);
		}
		else
		{
			transform.Rotate(0, 180, 0);
		}
	}

	public void SetJumpForce(float jumpForce) {
		m_JumpForce = jumpForce;
	}
	public void SetInWater(bool inWater) {
		m_InWater = inWater;
		if (inWater)
		{
			float yRotation = m_FacingRight ? 0 : 180;
			transform.rotation = Quaternion.Euler(0, yRotation, -90);
		}
		else
		{
			float yRotation = m_FacingRight ? 0 : -180;
			transform.rotation = Quaternion.Euler(0, yRotation, 0);
		}
	}
	public void SetIsStopped(bool isStopped) {
		m_IsStopped = isStopped;
	}

	public Rigidbody2D getRigidbody2D() {
		return m_Rigidbody2D;
	}

}