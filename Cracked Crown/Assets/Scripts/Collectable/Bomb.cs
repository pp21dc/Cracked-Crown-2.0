using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{

    [HideInInspector]
    public bool isThrown = false;

    private float damage = 25f;

    private EnemyAIController[] enemiesInRange;

    // Update is called once per frame
    void Update()
    {
        if (isThrown)
        {
            gameObject.SetActive(true);

            Vector3 movementVector = new Vector3(3,0,0);

            transform.position += movementVector * Time.deltaTime;
            StartCoroutine(Explode());
        }
    }

    private IEnumerator Explode()
    {

        yield return new WaitForSeconds(2f);
        // play explosion effect
        gameObject.SetActive(false);
        for (int i = 0; i <= enemiesInRange.Length; )
        {
            enemiesInRange[i].DecHealth(damage);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i <= enemiesInRange.Length; i++)
        {
            if (gameObject.tag == "Radius")
            {
                enemiesInRange[i] = other.GetComponent<EnemyAIController>();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        for (int i = 0; i <= enemiesInRange.Length; i++)
        {
            if (gameObject.tag == "Radius")
            {
                enemiesInRange[i] = null;
            }
        }
    }
}
