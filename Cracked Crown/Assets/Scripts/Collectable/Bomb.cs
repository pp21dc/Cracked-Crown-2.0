using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public Vector3 direction;

    private float damage = 100f;
    private int count = 0;
    private float speed = 25;

    private EnemyAIController[] enemiesInRange;
    private PlayerBody body;

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
        StartCoroutine(Explode());
    }

    private IEnumerator Explode()
    {

        // play wub wub animation looking like its gonna explode

        yield return new WaitForSeconds(5f);

        // play explosion effect

        if (enemiesInRange != null)
        {
            foreach (EnemyAIController enemy in enemiesInRange)
            {
                enemiesInRange[count].DecHealth(damage);
                count--;
            }
        }

        Debug.Log("bomb went boom");
        gameObject.SetActive(false);

    }

    public void setDirection(Vector3 dir)
    {
        direction = dir;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Medium" || other.tag == "Heavy" || other.tag == "Light")
        {
            Debug.Log("enemy in radius");
            enemiesInRange[count] = other.GetComponent<EnemyAIController>();
            count++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Medium" || other.tag == "Heavy" || other.tag == "Light")
        {
            Debug.Log("enemy out of range");
            count--;
            enemiesInRange[count] = null; // dont know how to know which one is leaving and how to delete that one from list
        }
    }
}
