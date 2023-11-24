using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    [Header("Controlled By Scriptable Object CharacterType")]
    [SerializeField]
    private float movementSpeed = 5f;
    [SerializeField]
    private float dashSpeed = 15f;
    [SerializeField]
    private float dashTime = 0.5f;
    [SerializeField]
    public CharacterType CharacterType;

    [Header("Do Not Touch")]
    [SerializeField]
    private PlayerController controller;
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private GameObject prefabForAttack;
    [SerializeField]
    private Transform primaryAttackPoint;
    [SerializeField]
    private Transform primaryAttackSpawnPoint;

    public GameObject CharacterFolder;

    private bool canAttack = true;
    private bool dashOnCD = false;
    private bool canTakeDamage = true;

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
        primaryAttackSpawnPoint.localPosition = (movementVector) * 10;
        primaryAttackSpawnPoint.localRotation = primaryAttackPoint.localRotation;
        primaryAttackPoint.LookAt(primaryAttackSpawnPoint);
        primaryAttackPoint.eulerAngles = new Vector3(0,primaryAttackPoint.eulerAngles.y,0);

        if (movementVector.magnitude > 1)
        {
            movementVector.Normalize();
        }
        movementVector = rb.position + (movementVector * movementSpeed * Time.deltaTime);

        rb.MovePosition(movementVector);
    }

    public void SetCharacterData()
    {
        movementSpeed = CharacterType.moveSpeed;
        dashSpeed = CharacterType.dashSpeed;
        dashTime = CharacterType.dashTime;
    }

    private void Attack()
    {
        if (controller.ExecuteDown)
        {
            // havnt made yet
        }
        if (controller.PrimaryAttackDown & canAttack)
        {
            GameObject attack = Instantiate(prefabForAttack, primaryAttackSpawnPoint);
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
