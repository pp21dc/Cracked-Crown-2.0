using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Bomb : MonoBehaviour
{
    public Vector3 direction;

    public float damage = 50f;
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
    [SerializeField]
    private List<PlayerBody> playersInRange;
    private List<BossPhases> clawsInRange;
    private Rigidbody rb;
    private PlayerController controller;
    private SpriteRenderer sprite;
    private Animator animator;
    private CameraShake css;
    [SerializeField]
    AudioSource AS;

    private void Awake()
    {
        //Debug.Log("controller is: " + controller);
        rb = GetComponent<Rigidbody>();
        enemiesInRange = new List<EnemyAIController>();
        playersInRange = new List<PlayerBody>();
        clawsInRange = new List<BossPhases>();
        sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        animator = transform.GetChild(1).GetComponent<Animator>();
    }

    private void Start()
    {
        
    }


    int thing = 1;
    // Update is called once per frame
    bool flashing;
    void Update()
    {
        css = GameManager.Instance.css;
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

        yield return new WaitForSeconds(1.7f);
        AS.Play();
        yield return new WaitForSeconds(0.3f);

        transform.GetChild(0).gameObject.SetActive(false);
        animator.SetBool("Blow", true);
        
        if (css != null)
            StartCoroutine(css.Shake(1.5f, 2f));

        if (true)
        {
            foreach (EnemyAIController enemy in enemiesInRange)
            {
                enemy.DecHealth(damage);
            }
            foreach (PlayerBody pb in playersInRange)
            {
                pb.DecHealth(damage);
            }
            foreach (BossPhases bp in clawsInRange)
            {
                bp.decHealth(20.0f);
            }

        }
    

        yield return new WaitForSeconds(2.5f);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerBody pb = other.transform.GetComponent<PlayerBody>();
            if (!playersInRange.Contains(pb))
            {
                playersInRange.Add(pb);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.tag == "Enemy")
        {
            currentEnemy = other.transform.parent.GetChild(0).gameObject.GetComponent<EnemyAIController>();
            if (!enemiesInRange.Contains(currentEnemy))
                enemiesInRange.Add(currentEnemy);
        }
        if (other.tag == "Player")
        {
            PlayerBody pb = other.transform.GetComponent<PlayerBody>();
            if (!playersInRange.Contains(pb))
            {
                playersInRange.Add(pb);
            }
        }
        if (other.tag == "Boss")
        {
            BossPhases bp = other.transform.GetComponent<BossPhases>();
            if (!clawsInRange.Contains(bp))
                clawsInRange.Add(bp);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            currentEnemy = other.transform.parent.GetChild(0).gameObject.GetComponent<EnemyAIController>();
            
            enemiesInRange.Remove(currentEnemy);
        }
        if (other.tag == "Player")
        {
            PlayerBody pb = other.transform.GetComponent<PlayerBody>();
            playersInRange.Remove(pb);
        }
        if (other.tag == "Boss")
        {
            BossPhases bp = other.transform.GetComponent<BossPhases>();
            clawsInRange.Remove(bp);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            counter++;
        }
    }
}
