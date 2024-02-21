using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypePrimaryAttack : MonoBehaviour
{
    private EnemyAIController enemyController;
    public PlayerBody playerBody;

    private void Awake()
    {
        //playerBody = gameObject.GetComponentInParent<PlayerBody>();
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
        if (other.tag == "Enemy" || other.tag == "Light" || other.tag == "Medium" || other.tag == "Heavy")
        {
            playerBody.hitEnemy = true;
            Rigidbody rb;
            enemyController = other.transform.parent.GetChild(0).GetComponent<EnemyAIController>();
            enemyController.DecHealth(playerBody.damage);
            rb = other.GetComponent<Rigidbody>();    
            if (!enemyController.lockKnock)
                enemyController.StartCoroutine(enemyController.KB(playerBody.GetMovementVector() * 15 * playerBody.forceMod));
            Debug.Log("Enemy Health: " + enemyController.Health);
        }
        if (other.tag == "Boss")
        {
            if (other.gameObject.GetComponent<BossPhases>().cantakedmg)
            {
                other.gameObject.GetComponent<BossPhases>().decHealth(playerBody.damage);
            }
        }
    }
}
