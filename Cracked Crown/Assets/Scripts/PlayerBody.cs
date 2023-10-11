using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    [SerializeField]
    private PlayerController controller;
    [SerializeField]
    private float movementSpeed = 5f;
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private GameObject prefabForAttack;
    [SerializeField]
    private Transform primaryAttackPoint;

    private void Update()
    {
        Attack();
    }

    private void FixedUpdate()
    {
        float zInput = controller.ForwardMagnitude;
        float xInput = controller.HorizontalMagnitude;

        Vector3 movementVector = new Vector3(xInput, 0, zInput);
        if (movementVector.magnitude > 1)
        {
            movementVector.Normalize();
        }
        movementVector = rb.position + (movementVector * movementSpeed * Time.deltaTime);

        rb.MovePosition(movementVector);
    }

    private void Attack()
    {
        if (controller.ExecuteDown)
        {
            // havnt made yet
        }
        if (controller.PrimaryAttackDown)
        {
            GameObject attack = Instantiate(prefabForAttack, primaryAttackPoint.position, primaryAttackPoint.rotation);
            Debug.Log("primary attack happened");
        }
    }
}
