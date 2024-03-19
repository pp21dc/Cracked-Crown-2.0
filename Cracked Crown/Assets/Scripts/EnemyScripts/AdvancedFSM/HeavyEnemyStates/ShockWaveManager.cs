using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWaveManager : MonoBehaviour
{


    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            PlayerBody pb = other.GetComponent<PlayerBody>();
            StartCoroutine(pb.gotHitKnockback(-pb.GetMovementVector()));
            pb.DecHealth(3f);
        }
    }
}
