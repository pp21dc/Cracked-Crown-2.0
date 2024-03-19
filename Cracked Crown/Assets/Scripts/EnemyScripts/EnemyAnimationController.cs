using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private RuntimeAnimatorController[] Light_AC;
    [SerializeField]
    private RuntimeAnimatorController[] Medium_AC;
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
    public bool Grabbing;
    public bool HitReact;
    float timer;

    [Header("Medium Enemy")]
    public bool Dashing;

    /*
     * INDEX:: 0 = Light, 1 = Med, 2= Heavy
     */
    public void SetAnimController(int index, int type)
    {
        if (type == 0)
            animator.runtimeAnimatorController = Light_AC[index-1];
        else if (type == 1)
            animator.runtimeAnimatorController = Medium_AC[index-1];
        if (index-1 == 0)
        {
            EAIC.colour = "Green";
        }
        else if (index-1 == 1)
        {
            EAIC.colour = "Purple";
        }
        else if (index-1 == 2)
        {
            EAIC.colour = "Red";
        }
    }

    private void Update()
    {
        animator.SetBool("Dead", Dead);
        animator.SetBool("Moving", Moving);
        animator.SetBool("Attacking", Attacking);
        animator.SetBool("Dashing", Dashing);
        animator.SetBool("Stunned", Stunned);
        if (HitReact)
        {
            HitReact = false;
            if (EAIC.hitBy.position.x + 1 > EAIC.enemyPosition.transform.position.x)
            {
                SR.flipX = false;
            }
            else
            {
                SR.flipX = true;
            }
            animator.SetTrigger("HitReact");
        }

        if (transform.parent.parent.CompareTag("Light"))
        {
            animator.SetBool("Grab_B", Bunny_Grabbed);
            animator.SetBool("Grab_D", Duck_Grabbed);
            animator.SetBool("Grab_F", Frog_Grabbed);
            animator.SetBool("Grab_Ba", Badger_Grabbed);
            animator.SetBool("Grabbing", Grabbing);
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

    public void StartGrab()
    {
        Grabbing = true;
    }

    public void EndGrab()
    {
        Grabbing = false;
    }

}
