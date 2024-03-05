using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabWalk : MonoBehaviour
{

    private float speed = 15.0f;
    public float health = 1.0f;
    public Animator animator;
    private bool hasDied = false;

    [SerializeField]
    private Transform finalPos;
    [SerializeField]
    private Vector3 movementVector;

    // Update is called once per frame
    void Update()
    {
        if (hasDied)
        {
            animator.SetBool("PermaDead", true);
        }
        if (!hasDied)
        {
            if (Vector3.Distance(transform.position, finalPos.position) > 5.0f)
            {
                transform.position = transform.position + (movementVector * speed * Time.deltaTime);
            }
            else
            {
                animator.SetTrigger("AtPosition");
            }

            if (health <= 0)
            {
                // play death animation
                animator.SetTrigger("Death");
                speed = 0;
                StartCoroutine(deathTime());
            }
        }
    }

    private IEnumerator deathTime()
    {
        yield return new WaitForSeconds(0.05f);
        hasDied = true;
    }
}
