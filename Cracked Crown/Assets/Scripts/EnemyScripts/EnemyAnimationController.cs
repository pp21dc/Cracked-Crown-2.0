using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    public bool Dead;
    public bool Moving;
    public bool Attacking;
    public bool Stunned;

    [Header("Medium Enemy")]
    public bool Dashing;

    private void FixedUpdate()
    {
        animator.SetBool("Dead", Dead);
        animator.SetBool("Moving", Moving);
        animator.SetBool("Attacking", Attacking);
        animator.SetBool("Dashing", Dashing);
        animator.SetBool("Stunned", Stunned);
    }



}
