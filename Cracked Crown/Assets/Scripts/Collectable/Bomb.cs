using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public Vector3 direction;

    private float damage = 100f;
    private int count = 0;
    private float speed = 5;
    private bool playOnce = true;
    private Vector3 height;
    private int counter = 0;

    private List<EnemyAIController> enemiesInRange;
    private PlayerBody body;
    private Rigidbody rb;

    private void Awake()
    {
        height = new Vector3(0,1,0);
        rb = GetComponent<Rigidbody>();
        enemiesInRange = new List<EnemyAIController>();
    }

    int thing = 1;
    // Update is called once per frame
    void Update()
    {
        if (counter == 1)
        {
            speed = 0;
            thing = 0;
        }

        transform.position += (direction + height) * speed * Time.deltaTime;
        if (playOnce)
        {
            StartCoroutine(Explode());
            playOnce = false;
        }

        transform.position += new Vector3(0,-4,0) * thing * Time.deltaTime;
    }

    private IEnumerator Explode()
    {

        // play wub wub animation looking like its gonna explode

        yield return new WaitForSeconds(2f);

        // play explosion effect

        if (enemiesInRange != null)
        {
            foreach (EnemyAIController enemy in enemiesInRange)
            {
                enemy.DecHealth(damage);
            }
        }

        Debug.Log("bomb went boom");
        gameObject.SetActive(false);

    }

    public void setDirection(Vector3 dir)
    {
        direction = dir;
        Debug.Log("The direction of bomb is: " + direction);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            //Debug.Log("enemy in radius");
            //Debug.Log(other.gameObject.transform.parent.GetChild(0).GetComponent<EnemyAIController>());
            enemiesInRange.Add(other.transform.parent.GetChild(0).gameObject.GetComponent<EnemyAIController>());
            //Debug.Log("enemy in list is: " + enemiesInRange[count]);
            //count++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Medium" || other.tag == "Heavy" || other.tag == "Light")
        {
            //Debug.Log("enemy out of range");
            //count--;
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
