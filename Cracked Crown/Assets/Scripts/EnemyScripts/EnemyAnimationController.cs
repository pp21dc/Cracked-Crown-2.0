using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private RuntimeAnimatorController[] AC;
    [SerializeField]
    public SpriteRenderer SR;
    [SerializeField]
    private EnemyAIController EAIC;

    public bool Dead;
    public bool Moving;
    public bool Attacking;
    public bool Stunned;
    public bool Bunny_Grabbed;
    public bool Frog_Grabbed;
    public bool Badger_Grabbed;
    public bool Duck_Grabbed;

    float timer;

    [Header("Medium Enemy")]
    public bool Dashing;

    /*
     * INDEX:: 0 = Light, 1 = Med, 2= Heavy
     */
    public void SetAnimController(int index)
    {
        animator.runtimeAnimatorController = AC[index];
    }

    private void FixedUpdate()
    {
        animator.SetBool("Dead", Dead);
        animator.SetBool("Moving", Moving);
        animator.SetBool("Attacking", Attacking);
        animator.SetBool("Dashing", Dashing);
        animator.SetBool("Stunned", Stunned);
        if (transform.parent.parent.CompareTag("Light"))
        {
            animator.SetBool("Grab_B", Bunny_Grabbed);
            animator.SetBool("Grab_D", Duck_Grabbed);
            animator.SetBool("Grab_F", Frog_Grabbed);
            animator.SetBool("Grab_Ba", Badger_Grabbed);
        }
        

        if (Stunned)
        {
            EAIC.stunObj.SetActive(true);
        }
        else
        {
            EAIC.stunObj.SetActive(false);
        }
    }


    public void EndAttack()
    {
        EAIC.EAC.Attacking = false;
    }
}
