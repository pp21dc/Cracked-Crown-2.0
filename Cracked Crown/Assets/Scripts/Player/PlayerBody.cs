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
    private float finisherRadius = 20f;
    [SerializeField]
    private float health = 10f;
    public float Health { get { return health; } }

    [SerializeField]
    public CharacterType CharacterType;

    [Header("Do Not Touch")]
    [SerializeField]
    private PlayerController controller;
    [SerializeField]
    private PlayerAnimController animController;
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private GameObject prefabForAttack;
    [SerializeField]
    private Transform primaryAttackPoint;
    [SerializeField]
    private Transform primaryAttackSpawnPoint;
    [SerializeField]
    private GameObject finisherColliderGO;

    public GameObject CharacterFolder;

    public PlayAnim SwordSlash;

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
        movementVector = (movementVector * movementSpeed * Time.deltaTime);

        rb.AddForce(movementVector*400);
        
        if (rb.velocity.magnitude > 30f)
        {
            animController.Moving = true;
        }
        else
        {
            animController.Moving = false;
        }

    }

    public void SetCharacterData()
    {
        movementSpeed = CharacterType.moveSpeed;
        dashSpeed = CharacterType.dashSpeed;
        dashTime = CharacterType.dashTime;
        finisherRadius = CharacterType.finisherRadius;
        finisherColliderGO.GetComponent<CapsuleCollider>().radius = finisherRadius;
    }
    float x = 0;
    private void Attack()
    {
        if (controller.PrimaryAttackDown & canAttack)
        {
            animController.Attacking = true;
            GameObject attack = Instantiate(prefabForAttack, primaryAttackSpawnPoint);
            SwordSlash.sword.Play();
            Debug.Log("primary attack happened");
        }

        if (x > 1)
        {
            x = 0;
            animController.Attacking = false;
        }
        else
        {
            x += Time.deltaTime;
        }

    }

    public void Execute(GameObject enemy)
    {

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
        animController.Dashing = true;

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
            rb.AddForce((dashDirection * dashSpeed * Time.deltaTime)*400);
            yield return null;
        }

        canAttack = true;
        canTakeDamage = true;
        animController.Dashing = false;
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(1f);
        dashOnCD = false;
    }
}
