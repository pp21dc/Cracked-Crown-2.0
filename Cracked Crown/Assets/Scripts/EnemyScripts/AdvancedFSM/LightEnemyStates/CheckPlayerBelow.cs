using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlayerBelow : MonoBehaviour
{
    private bool seesPlayer = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            seesPlayer = true;
        }
    }

    

    public bool IsPlayerBelow()
    {
        if(seesPlayer)
        {
            seesPlayer = false;
            return true;
            
        }
        else
        { return false; }
    }
}
