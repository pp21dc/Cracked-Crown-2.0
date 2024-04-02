using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheck : MonoBehaviour
{
    public EnemyAIController AI;
    public Collider checker;

    private void OnTriggerEnter(Collider other)
    {
        if (checker != null)
        {


            if (other.CompareTag("Wall"))
            {
                if (!AI.EAC.Stunned)
                {
                    AI.wallContact = true;
                }
            }
        }
    }
}
