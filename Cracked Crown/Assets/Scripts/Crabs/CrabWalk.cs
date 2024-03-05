using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabWalk : MonoBehaviour
{

    private float speed = 15.0f;
    public float health = 1.0f;
    public Animator animator;

    [SerializeField]
    private Transform finalPos;
    [SerializeField]
    private Vector3 movementVector;

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, finalPos.position) > 5.0f)
        {
            transform.position = transform.position  + (movementVector * speed * Time.deltaTime);
        }
        else
        {
            animator.SetTrigger("AtPosition");
        }

        if(health <= 0)
        {
            // play death animation
            animator.SetTrigger("Death");
            speed = 0;
        }
    }
}
