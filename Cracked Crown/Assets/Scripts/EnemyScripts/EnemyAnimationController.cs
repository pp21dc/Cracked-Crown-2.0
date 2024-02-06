using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    public SpriteRenderer SR;

    public bool Dead;
    public bool Moving;
    public bool Attacking;
    public bool Stunned;

    float timer;

    [Header("Medium Enemy")]
    public bool Dashing;

    private void FixedUpdate()
    {
        animator.SetBool("Dead", Dead);
        animator.SetBool("Moving", Moving);
        animator.SetBool("Attacking", Attacking);
        animator.SetBool("Dashing", Dashing);
        animator.SetBool("Stunned", Stunned);

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
