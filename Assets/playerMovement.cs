using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour

{

    public CharacterController2D controller;
    public AxeWeapon axeController;
    public Animator animator;

    public float runSpeed = 40f;

    //private float dashTime;
    //public float startDashTime = 2.5f;

    float horizontalMove = 0f;

    float verticalMove = 0f;

    //float jumpTime = 0.2f;
    //float jumpTimeCounter;

    // Player Movement Booleans
    bool jump = false;
    bool crouch = false;
    bool sprint = false;
    bool dash = false;

    // Player Attack Booleans
    bool uppercut = false;
    bool downattack = false;
    bool meleeAttack = false;

    //Player Throw Animation
    bool throwAttack = false;

    //If the player has the axe in hand
    bool hasAxe = true;

    // Start is called before the first frame update
    void Start()
    {
        //  dashTime = startDashTime;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        verticalMove = controller.vSpeed();

        animator.SetFloat("vSpeed", verticalMove);

        //Throw Attack
        if (Input.GetButtonDown("ThrowAxe") && controller.timeStampThrowAttack <= Time.time) {
            throwAttack = true;
            animator.SetBool("Throw", true);
            hasAxe      = false;
        }

        // Uppercut Attack
        if (Input.GetButtonDown("Attack") && controller.timeStampAttack <= Time.time)
        {
            //Handle returning the axe if the player doesn't have it
            if (!hasAxe)
            {
                Destroy(GameObject.FindWithTag("Axe"));
                hasAxe = true;
            }
            if (Input.GetKey(KeyCode.W)) {
                uppercut = true;
            }
            else if (Input.GetKey(KeyCode.S)) {
                downattack = true;
            }
            else
            {
                meleeAttack = true;
                animator.SetBool("Attack", true);
            }
        }

        // Dealing with Dashing
        if (Input.GetButtonDown("Dash") && controller.timeStampDash <= Time.time)
        {
            animator.SetBool("Dash", true);
            dash = true;
        }

        // Dealing with Jumping
        if ((controller.m_Grounded && Input.GetButtonDown("Jump")) || !controller.m_HasJumped && Input.GetButtonDown("Jump"))
        {
            jump = true;
            animator.SetBool("JumpPressed", true);
        }

        // Dealing with crouching
        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
            animator.SetBool("Crouch", true);
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            crouch = false;
            animator.SetBool("Crouch", false);
        }

        // Dealing with Sprinting
        if (Input.GetButtonDown("Sprint"))
        {
            sprint = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            sprint = false;
        }
    }

    void FixedUpdate()
    {
        //Move out character
        controller.MovePlayer(  horizontalMove * Time.fixedDeltaTime, 
                                crouch, 
                                jump, 
                                sprint, 
                                dash, 
                                uppercut, 
                                downattack,
                                meleeAttack,
                                throwAttack);
        throwAttack = false;
        jump = false;
        uppercut = false;
        downattack = false;
        meleeAttack = false;
        dash = false;
        animator.SetBool("Attack", false);
        animator.SetBool("Dash", false);
        animator.SetBool("JumpPressed", false);
        animator.SetBool("Throw", false);
    }
}
