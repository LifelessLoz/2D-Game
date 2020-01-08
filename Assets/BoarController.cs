using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarController : MonoBehaviour
{
    public int health;
    public float speed;
    public GameObject boarBloodSplash;

    private Rigidbody2D boar_rigidbody2D;
    public Animator animator;

    private Animator newAnimation;

    public float timeStampBoarMovement = 0f;

    public float boar_direction_switch = 3f;
    public bool boar_right = true;

    private bool m_FacingRight = true;  // For determining which way the boar is currently facing.

    // Start is called before the first frame update
    void Start()
    {
        boar_rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        MoveBoar(speed);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(damage + " damage taken");
        Instantiate(boarBloodSplash, transform.position, Quaternion.identity);
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void MoveBoar(float move) {

        if (timeStampBoarMovement <= Time.time)
        {
            timeStampBoarMovement = Time.time + boar_direction_switch;
            boar_right ^= true;
        }
        if (boar_right)
        {
            boar_rigidbody2D.velocity = Vector2.left * move;
            //boar_rigidbody2D.AddForce(new Vector2(move, 0f));
        }
        else
        {
            boar_rigidbody2D.velocity = Vector2.right * move;
            //boar_rigidbody2D.AddForce(new Vector2(-move, 0f));
        }

        // Set to animate boar movement
        animator.SetFloat("Speed", Mathf.Abs(boar_rigidbody2D.velocity.x));

        // If the input is moving the boar right and the boar is facing left...
        if (boar_rigidbody2D.velocity.x > 0.01 && !m_FacingRight)
        {
            // ... flip the player.
            Flip();
        }
        // Otherwise if the input is moving the boar left and the boar is facing right...
        else if (boar_rigidbody2D.velocity.x < -0.01 && m_FacingRight)
        {
            // ... flip the player.
            Flip();
        }
    }

    private void Flip()
    {
        // Switch the way the boar is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the boar's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
