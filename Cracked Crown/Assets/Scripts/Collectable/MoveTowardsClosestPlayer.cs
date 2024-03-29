using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MoveTowardsClosestPlayer : MonoBehaviour
{

    [SerializeField]
    private float moveSpeed = 10f;
    [SerializeField]
    private GameObject parent;

    [SerializeField]
    Rigidbody rb;

    private bool playerInRange = false;
    private Transform playerTarget;
    private bool canSpeedUp = true;

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && playerTarget != null)
        {
            rb.AddForce((playerTarget.position - gameObject.transform.position) * moveSpeed);
            if (canSpeedUp)
            {
                StartCoroutine(SpeedUp());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" || other.tag == "Ghost")
        {
            playerInRange = true;
            playerTarget = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" || other.tag == "Ghost")
        {
            playerInRange = false;
            playerTarget = null;
        }
    }

    private IEnumerator SpeedUp()
    {
        float halfOfSpeed = moveSpeed/2;

        yield return new WaitForSeconds(0.5f);
        moveSpeed = moveSpeed + halfOfSpeed;
        canSpeedUp = false;
    }
}
