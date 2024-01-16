using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

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
    private float health = 100f;
    public float Health { get { return health; } }
    public float damage = 3f;

    [SerializeField]
    public CharacterType CharacterType;

    [Header("Do Not Touch")]
    [SerializeField]
    private PlayerController controller;
    public PlayerContainer playerContainer;
    public FinisherCollider executeCollideScript;
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
    [SerializeField]
    private Vector3 forExecutePosition = new Vector3(15, 0, 0);

    public GameObject CharacterFolder;
    public bool canMove = true;
    public PlayAnim SwordSlash;

    private bool canAttack = true;
    private bool dashOnCD = false;
    private bool canTakeDamage = true;
    private float executeHeal = 5f;
    private float executeMoveSpeed = 75f;
    private GameObject executeTarget;
    private bool canMovePlayerForexecute = false;
    private bool ifHopper = false;

    private void Update()
    {
        Attack();
        Dash();

        if (health <= 0)
        {
            Debug.Log("You Died");
            canMove = false;
            // have to do ghost stuff
        }
        
    }

    private void FixedUpdate()
    {
        if (true || !ifHopper)
        {
            if (canMove)
            {
                float zInput = controller.ForwardMagnitude;
                float xInput = controller.HorizontalMagnitude;


                Vector3 movementVector = new Vector3(xInput, 0, zInput);
                primaryAttackSpawnPoint.localPosition = (movementVector) * 10;
                primaryAttackSpawnPoint.localRotation = primaryAttackPoint.localRotation;
                primaryAttackPoint.LookAt(primaryAttackSpawnPoint);
                primaryAttackPoint.eulerAngles = new Vector3(0, primaryAttackPoint.eulerAngles.y, 0);

                if (movementVector.magnitude > 1)
                {
                    movementVector.Normalize();
                }
                movementVector = (movementVector * movementSpeed * Time.deltaTime);

                rb.AddForce(movementVector * 400);

                if (rb.velocity.magnitude > 30f)
                {
                    animController.Moving = true;
                }
                else
                {
                    animController.Moving = false;
                }
            }
            if (canMovePlayerForexecute && executeTarget != null)
            {
                transform.position = Vector3.MoveTowards(gameObject.transform.position, executeTarget.transform.position + forExecutePosition, executeMoveSpeed * Time.deltaTime);
            }
        }
    }

    public void DecHealth(float amount) 
    { 
        health = Mathf.Max(0, health - amount); //allows us to decrease the health of an enemy
    }
    public void AddHealth(float amount) 
    {
        health = Mathf.Min(100, health + amount); //allows us to add health to the enemy
    }

    public void SetCharacterData()
    {
        movementSpeed = CharacterType.moveSpeed;
        dashSpeed = CharacterType.dashSpeed;
        dashTime = CharacterType.dashTime;
        finisherRadius = CharacterType.finisherRadius;
        finisherColliderGO.GetComponent<CapsuleCollider>().radius = finisherRadius;
        forExecutePosition = CharacterType.executePosition;
        damage = CharacterType.attack;
        ifHopper = CharacterType.hop;
    }
    float x = 0;
    float attackTimer = 0;
    private void Attack()
    {
        if (controller.PrimaryAttackDown & canAttack)
        {
            canMove = false;
            canAttack = false;
            rb.velocity = Vector3.zero;
            //add force in opp dir from input
            
            animController.Attacking = true;
            GameObject attack = Instantiate(prefabForAttack, primaryAttackSpawnPoint);
            SwordSlash.sword.Play();
        }

        if (x > 0.1f)
        {
            x = 0;
            //animController.Attacking = false;
            canMove = true;
        }
        else
        {
            x += Time.deltaTime;
        }

        if (attackTimer > 0.2f)
        {
            attackTimer = 0;
            canAttack = true;
            animController.Attacking = false;
        }
        else
        {
            attackTimer += Time.deltaTime;
        }

    }

    public void Execute(GameObject enemy)
    {
        StartCoroutine(InExecute(enemy));
    }
    bool lockDash;
    private void Dash()
    {
        if (controller.DashDown & dashOnCD == false && !lockDash)
        {
            lockDash = true;
            animController.dashing = true;
            animController.Moving = false;
            StartCoroutine(DashCoroutine());
            
            dashOnCD = true;
            StartCoroutine(Cooldown());
        }
    }

    private IEnumerator DashCoroutine()
    {
        canTakeDamage = false;
        canAttack = false;
        canMove = false;

        float zInput = controller.ForwardMagnitude;
        float xInput = controller.HorizontalMagnitude;
        float startTime = Time.time; // need to remember this to know how long to dash

        Vector3 dashDirection = new Vector3(xInput, 0, zInput);
        if (dashDirection.magnitude > 0f)
        {
            dashDirection.Normalize();
        }
        else
        {
            dashDirection = new Vector3(1, 0, 0);
        }

        while (Time.time < startTime + dashTime)
        {
            rb.AddForce((dashDirection * dashSpeed * Time.deltaTime)*1400);
            yield return null;
        }

        canAttack = true;
        canTakeDamage = true;
        canMove = true;
        animController.dashing = false;
        lockDash = false;
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(1f);
        dashOnCD = false;
    }

    private IEnumerator InExecute(GameObject toExecute)
    {
        if (toExecute != null)
        {
            executeTarget = toExecute;
            float xInput = controller.HorizontalMagnitude;

            controller.sprite = CharacterFolder.transform.GetChild(0);
            float scale = Mathf.Abs(controller.sprite.localScale.x);
            controller.sprite.localScale = new Vector3(scale, controller.sprite.localScale.y, 1); // to swap so its always facing enemy

            canTakeDamage = false;
            canMove = false;
            canMovePlayerForexecute = true;

            yield return new WaitForSeconds(0.75f);
            Destroy(toExecute);

            executeCollideScript.enemiesInRange.Remove(toExecute); // remove enemy from list
            health = health + executeHeal;
            canMove = true;
            canTakeDamage = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            health = health - 1;
            Debug.Log(health);
        }
    }
}
