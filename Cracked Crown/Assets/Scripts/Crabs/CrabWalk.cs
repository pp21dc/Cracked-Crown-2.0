using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabWalk : MonoBehaviour
{

    private float speed = 15.0f;
    public float health = 1.0f;
    public Animator animator;
    public bool hasDied = false;
    bool alreadyAtPos = false;

    [SerializeField]
    private Transform finalPos;
    [SerializeField]
    private Vector3 movementVector;

    [SerializeField]
    GameObject bigShadow;

    // Update is called once per frame
    void Update()
    {
        if (hasDied)
        {
            animator.SetBool("PermaDead", true);
            bigShadow.SetActive(false);
        }
        if (!hasDied)
        {
            if (Vector3.Distance(transform.position, finalPos.position) > 5.0f)
            {
                transform.position = transform.position + (movementVector * speed * Time.deltaTime);
            }
            else
            {
                if (!alreadyAtPos)
                {
                    animator.SetBool("AtPosition", true);
                    alreadyAtPos = true;
                }
            }

            if (health <= 0)
            {
                // play death animation
                animator.SetBool("Death", true);
                speed = 0;
                StartCoroutine(deathTime());
            }
        }
    }

    private IEnumerator deathTime()
    {
        yield return new WaitForSeconds(0.005f);
        hasDied = true;
    }
}
