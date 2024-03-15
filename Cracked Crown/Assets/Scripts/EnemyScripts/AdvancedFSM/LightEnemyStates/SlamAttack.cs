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

    private void Start()
    {
        Physics.IgnoreLayerCollision(0, 8);
    }

    public void OnEnable()
    {
        hasHit = false;
        hitPlayer = null;
        hitPlayerBody = null;
        HitGround = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player") && enemyAIController.canPickup && enemyAIController.CompareTag("Light"))
        {
            PlayerBody player = other.GetComponent<PlayerBody>();
            if (!player.alreadyDead && !HitGround && !player.Grabbed && enemyAIController.canPickup)
            {
                Debug.Log("SLAM HIT");
                player.Grabbed = true;
                enemyAIController.StartCoroutine(enemyAIController.ResetPlayerGrab(player));
                player.DecHealth(0.5f);
                hasHit = true;
                hitPlayer = player;
                hitPlayerBody = other.gameObject;
                enemyAIController.canPickup = false;
            }
        }

        if(other.CompareTag("Ground"))
        {
            HitGround = true;
        }

        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hasHit = false;
        }
        if (other.CompareTag("Ground"))
        {
            HitGround = false;
        }
    }
}
