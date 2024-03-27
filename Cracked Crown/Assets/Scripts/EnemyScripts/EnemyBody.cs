using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBody : MonoBehaviour
{
    [SerializeField]
    EnemyAIController EAIC;


    //bool damaging;
    float timer;
    public float timeToDamage = 2f;
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && transform.parent.tag != "Light" && !EAIC.EAC.Dead)
        {
            if (timer > timeToDamage)
            {
                timer = 0;
                //damaging = true;
                PlayerBody pb = other.gameObject.GetComponent<PlayerBody>();
                if (pb.canTakeDamage && !pb.alreadyDead)
                {
                    pb.DecHealth(11f);
                    EAIC.EAC.Attacking = true;
                }

            }
            else
            {
                timer += Time.deltaTime;
                
            }

        }
    }
}
