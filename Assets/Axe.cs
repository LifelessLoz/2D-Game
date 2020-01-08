using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MonoBehaviour
{
    public float axeThrowSpeed = 20f;
    public Rigidbody2D axeRigidBody;
    public Animator animator;
    //bool hitObject = false;

    // Start is called before the first frame update
    void Start()
    {
        axeRigidBody.velocity = transform.right * axeThrowSpeed;
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        Debug.Log(hitInfo.name);
        //hitObject = true;
        animator.SetBool("hitObject", true);
        axeRigidBody.velocity = transform.right * 0;

        BoarController enemy = hitInfo.GetComponent<BoarController>();
        if (enemy != null)
        {
            enemy.TakeDamage(30);
            Destroy(gameObject);
        }
    }
}
