using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    PlayerBody pb;

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            pb = other.GetComponent<PlayerBody>();
            pb.DecHealth(2f);
            StartCoroutine(pb.gotHitKnockback(-pb.GetMovementVector()));
        }
    }
}
