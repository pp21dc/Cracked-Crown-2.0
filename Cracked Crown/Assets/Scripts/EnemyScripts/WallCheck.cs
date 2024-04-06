using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheck : MonoBehaviour
{
    public EnemyAIController AI;
    public Collider checker;

    private void OnTriggerStay(Collider other)
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

    private void OnTriggerExit(Collider other)
    {
        if (checker != null)
        {


            if (other.CompareTag("Wall"))
            {
                if (!AI.EAC.Stunned)
                {
                    AI.wallContact = false;
                }
            }
        }
    }
}
