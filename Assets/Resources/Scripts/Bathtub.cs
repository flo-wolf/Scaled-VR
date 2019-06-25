using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bathtub : MonoBehaviour
{
    public Animator animator;


    void Start()
    {
        //animator.SetTrigger("fade-in");
    }

    public void FadeOut()
    {
        animator.SetTrigger("fade-out");
        //animator.Play("bathtub_fade-out");
    }

    public void Kill()
    {
        Destroy(gameObject);
    }
}
