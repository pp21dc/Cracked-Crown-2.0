using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlamAttack : MonoBehaviour
{
    [SerializeField]
    EnemyAIController enemyAIController;
    public bool hasHit;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("player"))
        {
            PlayerBody player = other.GetComponent<PlayerBody>();
            player.DecHealth(0.5f);
            hasHit = true;
        }

        

        
    }
}
