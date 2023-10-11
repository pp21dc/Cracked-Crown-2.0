using System;
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

    [SerializeField]
    private float dashSpeed = 15f;
    [SerializeField]
    private float dashTime = 0.5f;

    private bool canAttack = true;
    private bool dashOnCD = false;
    private bool canTakeDamage = true;


    private void Start()
    {
        controller = GameObject.FindGameObjectWithTag("PlayerController");
    }
    private void Update()
    {
        Attack();
        Dash();
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
        if (controller.PrimaryAttackDown & canAttack)
        {
            GameObject attack = Instantiate(prefabForAttack, primaryAttackPoint.position, primaryAttackPoint.rotation);
            Debug.Log("primary attack happened");
        }
    }

    private void Dash()
    {
        if (controller.DashDown & dashOnCD == false)
        {
            StartCoroutine(DashCoroutine());
            dashOnCD = true;
            StartCoroutine(Cooldown());
        }
    }

    private IEnumerator DashCoroutine()
    {
        canTakeDamage = false;
        canAttack = false;

        float zInput = controller.ForwardMagnitude;
        float xInput = controller.HorizontalMagnitude;
        float startTime = Time.time; // need to remember this to know how long to dash

        Vector3 dashDirection = new Vector3(xInput, 0, zInput);
        if (dashDirection.magnitude > 1)
        {
            dashDirection.Normalize();
        }

        while (Time.time < startTime + dashTime)
        {
            transform.Translate(dashDirection * dashSpeed * Time.deltaTime);
            yield return null;
        }

        canAttack = true;
        canTakeDamage = true;
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(1f);
        dashOnCD = false;
    }
}
