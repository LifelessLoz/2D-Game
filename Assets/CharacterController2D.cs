using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
    [Range(1, 2)] [SerializeField] private float m_SprintSpeed = 1.2f;          // Amount of maxSpeed applied to the sprinting movement.
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
    [SerializeField] private float m_DashSpeed = 1.5f;
    [SerializeField] private float m_UpperCutSpeedStart = 1.5f;
    [SerializeField] private float m_UpperCutSpeedEnd = 1.5f;
    //[SerializeField] private float m_DownAttackSpeedStart = 1.5f;
    [SerializeField] private float m_DownAttackSpeedEnd = 1.5f;
    [SerializeField] private float m_fallMulti = 2.5f;
    [SerializeField] private float m_DashCooldown = 2.5f;                       // The cooldown between when a player can dash
    [SerializeField] private float m_AttackCooldown = 10.0f;                    // The cooldown between player attacks
    [SerializeField] private float m_UpAttackCooldown = 1.5f;                   // The cooldown between player Up attack
    [SerializeField] private float m_ThrowAttackCooldown = 1.5f;                // The cooldown between throw attacks
    [SerializeField] private int m_Attack_Damage = 10;                          // The amount of damage dealt by the player per attack
    [SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;				// A collider that will be disabled when crouching

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	public bool m_Grounded;            // Whether or not the player is grounded
    public bool m_HasJumped;           // Whether the player has jumped to leave the group or not
    public bool m_HasAirAttacked;      // Whether the player has performed an in air attack
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
    private Vector3 tempPos;

    public Animator animator;

    /* Axe throw settings */
    public Transform throwPoint;
    public GameObject axePrefab;
    public float Axe_Throw_Delay = 0.3f;

    /* Variables for cooldowns of Player Character */
    public float timeStampDash;
    public float timeStampAttack;
    public float timeStampUpAttack;
    public float timeStampDownAttack;
    public float timeStampThrowAttack;

    private bool downSmash = false;

    public Transform attackPosition;
    public LayerMask whatIsEnemies;
    public float attackRange;

    [Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

	private void FixedUpdate()
	{

        if (m_Rigidbody2D.velocity.y < 0) {
            m_Rigidbody2D.velocity += Vector2.up * Physics2D.gravity.y * (m_fallMulti) * Time.deltaTime;
        }

        bool wasGrounded = m_Grounded;
		m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
                m_HasJumped = false;
                m_HasAirAttacked = false;
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
	}

    // Function for moving the player character
	public void MovePlayer( float move, 
                            bool crouch, 
                            bool jump, 
                            bool sprint, 
                            bool dash, 
                            bool uppercut, 
                            bool downattack, 
                            bool meleeAttack,
                            bool throwAxe)
	{
		// If crouching, check to see if the character can stand up
		if (!crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}
		}

        // Handling Down Attack
        if (downattack && !m_Grounded)
        {
            m_Rigidbody2D.AddForce(new Vector2(0f, -m_DownAttackSpeedEnd));
            timeStampDownAttack = Time.time + 0.1f;
            downSmash = true;
        }
        if (downSmash && timeStampDownAttack <= Time.time) {
            m_Rigidbody2D.velocity = Vector2.up;
            downSmash = false;
        }

        //only control the player is grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
		{

			// If crouching
			if (crouch)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			} else
			{
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

            // If Sprinting
            if (sprint && !crouch)
            {
                move *= m_SprintSpeed;
            }

            // If Dashing
            if (dash && !crouch && timeStampDash <= Time.time)
            {
                timeStampDash = Time.time + m_DashCooldown;
                if (!m_FacingRight)
                    m_Rigidbody2D.AddForce(new Vector2(-m_DashSpeed, 0f));
                if (m_FacingRight)
                    m_Rigidbody2D.AddForce(new Vector2(m_DashSpeed, 0f));
            }

            /* Handling the different Attack functionalities */
            
            // Handling normal attack
            if(meleeAttack && timeStampAttack <= Time.time)
            {
                timeStampAttack = Time.time + m_AttackCooldown;
                Debug.Log("Attacked!");
                Invoke("Attack", 0.25f);
                animator.SetBool("hasAxe", true);
            }

            // Handling Uppercut Attack
            if (uppercut && !m_HasAirAttacked && timeStampUpAttack <= Time.time)
            {
                timeStampUpAttack = Time.time + m_UpAttackCooldown;
                tempPos = transform.position;
                tempPos.y += m_UpperCutSpeedStart;
                transform.position = tempPos;
                m_Rigidbody2D.velocity = Vector2.up * m_UpperCutSpeedEnd;
                m_HasAirAttacked = true;
            }

            //Handling Throw Attack
            if (throwAxe && timeStampThrowAttack <= Time.time)
            {
                timeStampThrowAttack = Time.time + m_ThrowAttackCooldown;
                Destroy(GameObject.FindGameObjectWithTag("Axe"));
                Invoke("Shoot", Axe_Throw_Delay);
                animator.SetBool("hasAxe", false);
            }

            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if (jump && !m_HasJumped)
		{
            // Teleport player fraction upwards so not to trigger m_HasJumped on fixed update
            tempPos = transform.position;
            tempPos.y += 0.5f;
            transform.position = tempPos;

            // Add a vertical force to the player.
            m_Grounded = false;
            m_HasJumped = true;

            // Move the character by finding the target velocity
            m_Rigidbody2D.velocity = new Vector2(move * 10f, m_JumpForce);

        }
    }

    // Function for getting the current speed of a players vertical velocity
    public float vSpeed()
    {
        return m_Rigidbody2D.velocity.y;
    }

	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

        transform.Rotate(0f, 180f, 0f);
	}

    /* Display Gizmos for testing */
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition.position, attackRange);
    }

    /* Axe throw */
    void Shoot()
    {
        Instantiate(axePrefab, throwPoint.position, throwPoint.rotation);
    }

    /* Normal attack */
    void Attack()
    {
        Collider2D[] attackCollider = Physics2D.OverlapCircleAll(attackPosition.position, attackRange, whatIsEnemies);
        for (int i = 0; i < attackCollider.Length; i++)
        {
            attackCollider[i].GetComponent<BoarController>().TakeDamage(m_Attack_Damage);
        }
    }
}
