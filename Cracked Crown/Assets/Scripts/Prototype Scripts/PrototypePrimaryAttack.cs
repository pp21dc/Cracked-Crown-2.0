using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypePrimaryAttack : MonoBehaviour
{
    private BossPhases bossController;
    private EnemyAIController enemyController;
    public PlayerBody playerBody;
    private CrabWalk miniCrab;

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
        yield return new WaitForSeconds(0.15f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            enemyController = other.transform.parent.GetChild(0).GetComponent<EnemyAIController>();
            if (!enemyController.EAC.Dead)
            {
                playerBody.hitEnemy = true;
                playerBody.eac_cur = enemyController;
                Rigidbody rb;

                enemyController.DecHealth(playerBody.damage, playerBody);
                rb = other.GetComponent<Rigidbody>();
                if (!enemyController.EAC.HitReact)
                    enemyController.EAC.HitReact = true;
                if (!enemyController.lockKnock)
                {
                    enemyController.hitBy = playerBody.transform;
                    enemyController.lockKnock = true;
                    enemyController.StartCoroutine(enemyController.KB(playerBody.AttackVector * 20 * playerBody.attackKnockback));
                }
            }
        }
        if (other.tag == "BossHit")
        {
            bossController = other.transform.parent.parent.GetComponent<BossPhases>();

            bossController.decHealth(playerBody.damage);
        }
        if (other.tag == "Mini Crab" || other.tag == "MiniCrabExecutable" || other.tag == "StruggleCrab" || other.tag == "ReviveCrab")
        {
            miniCrab = other.GetComponent<CrabWalk>();
            if (!miniCrab.crown)
                miniCrab.health = 0;
        }
        if (other.tag == "Player")
        {
            PlayerBody otherPlayer = other.gameObject.GetComponent<PlayerBody>();

            if (playerBody.playerID != otherPlayer.playerID && !otherPlayer.gotHit && !otherPlayer.alreadyDead)
            {
                otherPlayer.DecHealth(playerBody.damage*0.8f);
                if (otherPlayer.health <= 0 && playerBody != null)
                    playerBody.scoreboard.PlayersKilled++;
                otherPlayer.pbOther = playerBody;
                StartCoroutine(otherPlayer.gotHitKnockback(playerBody.GetMovementVector()));
            }
        }
    }
}
