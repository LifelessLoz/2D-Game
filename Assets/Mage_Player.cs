using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage_Player : MonoBehaviour
{

    private Rigidbody2D mage_rigidbody2D;
    public Animator animator;
    private Animator animation;

    // Start is called before the first frame update
    void Start()
    {
        mage_rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
