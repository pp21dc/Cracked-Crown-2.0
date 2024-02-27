using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Bomb : MonoBehaviour
{
    public Vector3 direction;

    private float damage = 100f;
    private int count = 0;
    private bool playOnce = true;
    private int counter = 0;

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

    private List<EnemyAIController> enemiesInRange;
    private Rigidbody rb;
    private PlayerController controller;
    private SpriteRenderer sprite;

    private void Awake()
    {
        Debug.Log("controller is: " + controller);
        rb = GetComponent<Rigidbody>();
        enemiesInRange = new List<EnemyAIController>();
        sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    int thing = 1;
    // Update is called once per frame
    void Update()
    {

        if (colourCount <= 30)
        {
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
            Debug.Log("Force: " + (direction + height) * speed);
            StartCoroutine(Explode());
            playOnce = false;
        }
        rb.velocity += new Vector3(0, gravity, 0) * thing * Time.deltaTime;
    }

    private IEnumerator ColourFlash()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        sprite.color = Color.white;

        colourCount++;
    }

    private IEnumerator Explode()
    {

        // play wub wub animation looking like its gonna explode

        yield return new WaitForSeconds(3f);

        // play explosion effect

        if (enemiesInRange != null)
        {
            foreach (EnemyAIController enemy in enemiesInRange)
            {
                enemy.DecHealth(damage);
            }
        }

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
        if (other.tag == "Enemy")
        {
            enemiesInRange.Add(other.transform.parent.GetChild(0).gameObject.GetComponent<EnemyAIController>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Medium" || other.tag == "Heavy" || other.tag == "Light")
        {
            enemiesInRange.Remove(other.transform.parent.GetChild(0).gameObject.GetComponent<EnemyAIController>()); // dont know how to know which one is leaving and how to delete that one from list
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
