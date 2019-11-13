using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarController : MonoBehaviour
{
    public int health;
    public float speed;
    public GameObject boarBloodSplash;

    private Animator animation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0)
        {
            Destroy(gameObject);
        }


    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(damage + " damage taken");
        Instantiate(boarBloodSplash, transform.position, Quaternion.identity);
    }
}
