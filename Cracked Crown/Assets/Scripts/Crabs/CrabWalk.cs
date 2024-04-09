using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabWalk : MonoBehaviour
{

    public float speed = 15.0f;
    public float health = 1.0f;
    public Animator animator;
    public bool hasDied = false;
    public bool stop = false;
    public bool alreadyAtPos = false;
    public bool canMove = false;
    private Transform startPos;
    public bool hasBeenExecuted = false;
    public bool crown = false;
    PlayerBody player;
    GameObject[] ghost = null;
    PlayerBody PB;

    [SerializeField]
    private Transform finalPos;
    [SerializeField]
    private Vector3 movementVector;
    [SerializeField]
    SpriteRenderer spriteRenderer;
    [SerializeField]
    GameObject bigShadow;
    GameManager GM;
    [SerializeField]
    public AudioSource AS_S;

    private void Awake()
    {
        GM = GameManager.Instance;
        startPos = gameObject.transform.parent;
    }

    private void Start()
    {
        if (crown)
        {
            canMove = false;
            animator.SetBool("Moving", false);
        }
    }

    // Update is called once per frame
    void Update()
    {

        foreach(PlayerContainer pc in GM.Players) 
        {
            //while (i < player.Length)
            //{
            PB = pc.PB;

            if (PB != null)
            {
                if (gameObject.tag == "Mini Crab" || gameObject.tag == "MiniCrabExecutable")
                {
                    if (!alreadyAtPos)
                    {
                        canMove = true;
                        animator.SetBool("Moving", true);
                        gameObject.transform.GetChild(0).gameObject.SetActive(true);
                    }
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
        if (canMove && !GameManager.Instance.isLoading)
        {
            if (hasDied)
            {
                animator.SetBool("PermaDead", true);
                bigShadow.SetActive(false);
            }
            if (!hasDied && !stop)
            {
                if (Vector3.Distance(transform.position, finalPos.position) > 5.0f)
                {
                    transform.position = transform.position + (movementVector * speed * Time.deltaTime);
                }
                else
                {
                    if (!alreadyAtPos)
                    {
                        
                        if (crown)
                            StartCoroutine(FadeSprite(0, 1));
                        else
                            animator.SetBool("AtPosition", true);
                        alreadyAtPos = true;
                    }
                }

                

            }
            if (health <= 0 && !hasDied)
            {
                // play death animation
                animator.SetBool("Death", true);
                speed = 0;
                if (!AS_S.isPlaying)
                    AS_S.Play();
                hasDied = true;
                StartCoroutine(respawnCrab());
            }
        }

    }


    private IEnumerator FadeSprite(float to, float speed)
    {
        if (to < 0.5f)
        {
            while (spriteRenderer != null && spriteRenderer.color.a >= to)
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a - (speed * Time.deltaTime));
                yield return new WaitForEndOfFrame();
            }
            //dropShadow.SetActive(false);
        }
        else
        {
            while (spriteRenderer != null && spriteRenderer.color.a <= to)
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a + (speed * Time.deltaTime));
                yield return new WaitForEndOfFrame();
            }
            //dropShadow.SetActive(true);
        }
    }

    public IEnumerator respawnCrab()
    {
        if (gameObject.tag == "MiniCrabExecutable" && hasBeenExecuted)
        {
            hasBeenExecuted = false;
            transform.position = startPos.position;
        }

        yield return new WaitForSeconds(1.0f);
        transform.position = startPos.position;
        animator.SetBool("AtPosition", false);
        alreadyAtPos = false;

        yield return new WaitForSeconds(0.5f);
        hasDied = false;
        animator.SetBool("Death", false);
        animator.SetBool("PermaDead", false);
        speed = 15.0f;
        health = 1.0f;
        
        if (gameObject.tag == "MiniCrabExecutable")
        {
            animator.SetBool("BadgerExecute", false);
            animator.SetBool("BunnyExecute", false);
            animator.SetBool("DuckExecute", false);
            animator.SetBool("FrogExecute", false);
            animator.SetBool("LeaveSign", false);
        }
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }
}
