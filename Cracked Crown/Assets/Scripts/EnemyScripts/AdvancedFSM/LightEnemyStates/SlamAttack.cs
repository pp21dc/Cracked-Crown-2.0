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

    public void OnEnable()
    {
        hasHit = false;
        hitPlayer = null;
        hitPlayerBody = null;
        HitGround = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && enemyAIController.canPickup && enemyAIController.CompareTag("Light"))
        {
            PlayerBody player = other.GetComponent<PlayerBody>();
            Debug.Log("SLAM HIT");
            player.DecHealth(0.5f);
            hasHit = true;
            hitPlayer = player;
            hitPlayerBody = other.gameObject;
            enemyAIController.canPickup = false;
        }

        if(other.CompareTag("Ground"))
        {
            HitGround = true;
        }

        
    }
}
