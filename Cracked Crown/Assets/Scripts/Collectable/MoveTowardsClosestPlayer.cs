using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsClosestPlayer : MonoBehaviour
{

    [SerializeField]
    private float moveSpeed = 10f;
    [SerializeField]
    private GameObject parent;

    private bool playerInRange = false;
    private Transform playerTarget;

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && playerTarget != null)
        {
            parent.transform.position = Vector3.MoveTowards(gameObject.transform.position, playerTarget.position, moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInRange = true;
            playerTarget = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInRange = false;
            playerTarget = null;
        }
    }
}
