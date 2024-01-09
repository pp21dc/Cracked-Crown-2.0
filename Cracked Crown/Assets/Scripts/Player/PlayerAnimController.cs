using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerAnimController : MonoBehaviour
{

    [SerializeField]
    Animator Animator;

    private bool moving;
    [HideInInspector]
    public bool Moving
    {
        get { return moving; }
        set { moving = value; }
    }

    private bool dashing;
    [HideInInspector]
    public bool Dashing
    {
        get { return Dashing; }
        set { Dashing = value; }
    }

    private bool dead;
    [HideInInspector]
    public bool Dead
    {
        get { return dead; }
        set { dead = value; }
    }

    private bool attacking;
    [HideInInspector]
    public bool Attacking
    {
        get { return attacking; }
        set { attacking = value; }
    }

    private void FixedUpdate()
    {
        Animator.SetBool("Moving", moving);
        Animator.SetBool("Dashing", dashing);
        Animator.SetBool("Attacking", attacking);
    }

 

}
