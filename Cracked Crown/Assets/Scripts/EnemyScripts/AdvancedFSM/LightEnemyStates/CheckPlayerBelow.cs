using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlayerBelow : MonoBehaviour
{
    public bool seesPlayer = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            seesPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            seesPlayer = false;
        }
    }



    public bool IsPlayerBelow()
    {
        if(seesPlayer)
        {
            //seesPlayer = false;
            return true;
            
        }
        else
        { 
            return false;
        }
    }
}
