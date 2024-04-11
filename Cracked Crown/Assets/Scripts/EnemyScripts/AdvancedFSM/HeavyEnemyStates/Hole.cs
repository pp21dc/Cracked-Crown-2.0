using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    PlayerBody pb;
    public GameObject papa;
    public Animator animator;
    

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            pb = other.GetComponent<PlayerBody>();
            pb.DecHealth(2f);
            StartCoroutine(pb.gotHitKnockback(-pb.GetMovementVector()));
        }
    }

    private void Start()
    {
        StartCoroutine(SelfD());
    }

    IEnumerator SelfD()
    {

        yield return new WaitForSeconds(7);

        animator.SetTrigger("Delete"); 

        yield return new WaitForSeconds(1);

        Destroy(papa);

        yield return null;
    }
}
