using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeperateCheck : MonoBehaviour
{
    public EnemyAIController AI;
    public Collider checker;

    private void OnTriggerEnter(Collider other)
    {
        if (checker != null)
        {
            

            if(other.CompareTag("Seperate"))
            {
                Debug.Log(other.tag);
                AI.InContact = true;
                AI.otherAI = other.gameObject;
            }
        }
    }


}
