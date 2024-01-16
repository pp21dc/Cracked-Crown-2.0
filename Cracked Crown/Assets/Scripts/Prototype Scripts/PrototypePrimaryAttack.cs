using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypePrimaryAttack : MonoBehaviour
{
    private EnemyAIController enemyController;
    private PlayerBody playerBody;

    private void Awake()
    {
        playerBody = gameObject.GetComponentInParent<PlayerBody>();
    }

    void Start()
    {
        StartCoroutine(destroyObject());
    }

    IEnumerator destroyObject()
    {
        yield return new WaitForSeconds(0.25f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        { 
            enemyController = other.GetComponent<EnemyAIController>();
            enemyController.DecHealth(playerBody.damage);
            Debug.Log("Enemy Health: " + enemyController.Health);
        }
    }
}
