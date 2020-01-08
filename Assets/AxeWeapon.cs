using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeWeapon : MonoBehaviour
{
    public Transform throwPoint;
    public GameObject axePrefab;
    public float Axe_Throw_Delay = 0.3f;

    [SerializeField] private float m_ThrowAttackCooldown = 1.5f;                // The cooldown between throw attacks

    public float timeStampThrowAttack;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("ThrowAxe") && timeStampThrowAttack <= Time.time)
        {
            timeStampThrowAttack = Time.time + m_ThrowAttackCooldown;
            Destroy(GameObject.FindGameObjectWithTag("Axe"));
            Invoke("Shoot", Axe_Throw_Delay);
        }
    }

    void Shoot()
    {
        Instantiate(axePrefab, throwPoint.position, throwPoint.rotation);
    }
}
