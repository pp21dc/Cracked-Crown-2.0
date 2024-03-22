using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Bomb : MonoBehaviour
{
    public Vector3 direction;

    private float damage = 1000f;
    private int count = 0;
    private bool playOnce = true;
    private int counter = 0;
    private bool enteredTrigger = false;
    private bool exitedTrigger = false;
    EnemyAIController currentEnemy;

    [Header("Do Not Touch")]
    [SerializeField]
    private Vector3 height;

    [Header("Can Touch")]
    [Header("Hover for more information")]
    [SerializeField]
    [Tooltip("make it a bigger negative number to have bomb not go as far")]
    private float gravity = -160;
    [SerializeField]
    [Tooltip("make bigger to increase speed of bomb travel")]
    private float speed = 17;
    private int colourCount = 0;

    [SerializeField]
    private List<EnemyAIController> enemiesInRange;
    private Rigidbody rb;
    private PlayerController controller;
    private SpriteRenderer sprite;
    private Animator animator;

    private void Awake()
    {
        //Debug.Log("controller is: " + controller);
        rb = GetComponent<Rigidbody>();
        enemiesInRange = new List<EnemyAIController>();
        sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        animator = transform.GetChild(1).GetComponent<Animator>();
    }

    int thing = 1;
    // Update is called once per frame
    bool flashing;
    void Update()
    {

        if (colourCount <= 180 && !flashing)
        {
            flashing = true;
            StartCoroutine(ColourFlash());
        }

        if (counter == 1)
        {
            speed = 0;
            thing = 0;
            rb.velocity = Vector3.zero;
        }

        if (playOnce)
        {
            height.x = height.x * controller.HorizontalMagnitude;
            height.z = direction.z * 45;

            rb.AddForce((direction + height) * speed, ForceMode.Impulse);
            //Debug.Log("Force: " + (direction + height) * speed);
            StartCoroutine(Explode());
            playOnce = false;
        }
        rb.velocity += new Vector3(0, gravity, 0) * thing * Time.deltaTime;

        if (enteredTrigger)
        {
            if (!enemiesInRange.Contains(currentEnemy))
            {
                enemiesInRange.Add(currentEnemy);
            }
            enteredTrigger = false;
        }
        if (exitedTrigger)
        {
            if (enemiesInRange.Contains(currentEnemy))
            {
                enemiesInRange.Remove(currentEnemy);
            }
            exitedTrigger = false;
        }
    }

    private IEnumerator ColourFlash()
    {
        sprite.color = Color.white;
        yield return new WaitForSeconds(0.2f);
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        colourCount++;
        flashing = false;
    }

    private IEnumerator Explode()
    {

        yield return new WaitForSeconds(2f);

        transform.GetChild(0).gameObject.SetActive(false);
        animator.SetBool("Blow", true);

        if (enemiesInRange != null)
        {
            foreach (EnemyAIController enemy in enemiesInRange)
            {
                if (Vector3.Distance(transform.position, enemy.transform.position) < 10.0f)
                {
                    enemy.DecHealth(damage);
                }
            }
        }

        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);

    }

    public void setDirection(Vector3 dir)
    {
        direction = dir;
    }

    public void SetController(PlayerController playerControls)
    {
        controller = playerControls;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "Light" || other.tag == "Medium" || other.tag == "Heavy")
    //    {
    //        currentEnemy = other.transform.parent.GetChild(0).gameObject.GetComponent<EnemyAIController>();
    //        enteredTrigger = true;
    //    }
    //}

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Light" || other.tag == "Medium" || other.tag == "Heavy")
        {
            currentEnemy = other.transform.parent.GetChild(0).gameObject.GetComponent<EnemyAIController>();
            enteredTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Medium" || other.tag == "Heavy" || other.tag == "Light")
        {
            currentEnemy = other.transform.parent.GetChild(0).gameObject.GetComponent<EnemyAIController>();
            exitedTrigger = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Player" || collision.gameObject.tag == "Medium" || collision.gameObject.tag == "Heavy" || collision.gameObject.tag == "Light")
        {
            counter++;
        }
    }
}
