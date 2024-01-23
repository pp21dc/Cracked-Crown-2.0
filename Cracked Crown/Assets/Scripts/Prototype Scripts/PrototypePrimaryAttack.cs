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
        transform.position = transform.position + new Vector3(1, 0, 0);
        StartCoroutine(destroyObject());
    }

    IEnumerator destroyObject()
    {
        yield return new WaitForSeconds(0.25f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.tag == "Enemy")
        {
            playerBody.hitEnemy = true;
            enemyController = other.transform.parent.GetChild(0).GetComponent<EnemyAIController>();
            enemyController.DecHealth(playerBody.damage);
            Debug.Log("Enemy Health: " + enemyController.Health);
        }
        if (other.tag == "Medium")
        {
            playerBody.hitEnemy = true;
            enemyController = other.GetComponent<EnemyAIController>();
            enemyController.DecHealth(playerBody.damage);
            Debug.Log("Enemy Health: " + enemyController.Health);
        }
        if (other.tag == "Light")
        {
            playerBody.hitEnemy = true;
            enemyController = other.GetComponent<EnemyAIController>();
            enemyController.DecHealth(playerBody.damage);
            Debug.Log("Enemy Health: " + enemyController.Health);
        }
        if (other.tag == "Heavy")
        {
            playerBody.hitEnemy = true;
            enemyController = other.GetComponent<EnemyAIController>();
            enemyController.DecHealth(playerBody.damage);
            Debug.Log("Enemy Health: " + enemyController.Health);
        }
    }
}
