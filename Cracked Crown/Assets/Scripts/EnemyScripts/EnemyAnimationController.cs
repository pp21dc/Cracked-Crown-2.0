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
            timer += Time.deltaTime;

            if (timer < 1f)
            {
                this.GetComponent<SpriteRenderer>().color = Color.red;
            }
            else if (timer < 2)
            {
                this.GetComponent<SpriteRenderer>().color = Color.white;
            }

            if (timer > 2)
            {
                timer = 0f;
            }
        }
    }
}
