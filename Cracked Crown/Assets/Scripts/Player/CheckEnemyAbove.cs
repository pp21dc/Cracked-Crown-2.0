using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckEnemyAbove : MonoBehaviour
{
    public Collider checkEnemy;

    [SerializeField]
    private PlayerCarried carryCheck;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Light"))
        {
            carryCheck.timeToCarry = true;
            carryCheck.enemyAIController = other.gameObject.GetComponent<EnemyAIController>();
        }
    }
}
