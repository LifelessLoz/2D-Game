using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamShake : MonoBehaviour
{
    public Animator camAnim;

    public void CameraShake()
    {
        camAnim.SetTrigger("Shake");
    }
}
