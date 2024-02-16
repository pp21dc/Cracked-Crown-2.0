using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlamAttack : MonoBehaviour
{
    [SerializeField]
    EnemyAIController enemyAIController;
    public bool hasHit;
    public bool HitGround;
    public PlayerBody hitPlayer;
    public GameObject hitPlayerBody;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            PlayerBody player = other.GetComponent<PlayerBody>();
            
            player.DecHealth(0.5f);
            hasHit = true;
            hitPlayer = player;
            hitPlayerBody = other.gameObject;
        }

        if(other.CompareTag("Ground"))
        {
            HitGround = true;
        }

        
    }
}
