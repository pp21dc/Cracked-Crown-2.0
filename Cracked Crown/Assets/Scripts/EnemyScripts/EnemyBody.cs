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
        if (other.tag == "Player")
        {
            if (timer > timeToDamage)
            {
                timer = 0;
                //damaging = true;
                PlayerBody pb = other.gameObject.GetComponent<PlayerBody>();
                if (pb.canTakeDamage)
                {
                    pb.DecHealth(1);
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
