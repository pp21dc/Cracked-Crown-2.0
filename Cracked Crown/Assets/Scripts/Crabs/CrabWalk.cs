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
    public bool canMove = false;

    GameObject[] player = new GameObject[4];
    GameObject[] ghost = null;
    PlayerBody PB;

    [SerializeField]
    private Transform finalPos;
    [SerializeField]
    private Vector3 movementVector;

    [SerializeField]
    GameObject bigShadow;
    GameManager GM;

    private void Awake()
    {
        GM = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < player.Length; i++)
        {
            //while (i < player.Length)
            //{

            player = GameObject.FindGameObjectsWithTag("Player");
            PB = player[i].gameObject.transform.GetComponent<PlayerBody>();

            if (player[i] != null)
                {
                    if (gameObject.tag == "Mini Crab" || gameObject.tag == "MiniCrabExecutable")
                    {
                        canMove = true;
                        gameObject.transform.GetChild(0).gameObject.SetActive(true);
                    }
                }
                if (gameObject.tag == "StruggleCrab")
                {
                    if (PB != null)
                    {
                        if (PB.Grabbed == true)
                        {
                            canMove = true;
                            gameObject.transform.GetChild(0).gameObject.SetActive(true);
                        }
                    }
                }
                if (gameObject.tag == "ReviveCrab")
                {
                    if (PB != null)
                    {
                        if (PB.alreadyDead == true)
                        {
                            canMove = true;
                            gameObject.transform.GetChild(0).gameObject.SetActive(true);
                        }
                    }
                }
            //}
        }
        if (canMove)
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

    }

    private IEnumerator deathTime()
    {
        yield return new WaitForSeconds(0.005f);
        hasDied = true;
    }
}
