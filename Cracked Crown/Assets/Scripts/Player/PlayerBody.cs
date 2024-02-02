using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
    [SerializeField]
    private GameObject deathBody; 

    private float attackKnockback = 100;
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
    [SerializeField]
    private EnemyAIController enemyAIController;

    public GameObject CharacterFolder;
    public bool canMove = true;
    public PlayAnim SwordSlash;

    [HideInInspector]
    public bool canAttack = true;
    [HideInInspector]
    public bool hitEnemy = false;
    [HideInInspector]
    public bool canUseItem = true;
    [HideInInspector]
    public bool canExecute = true;
    [HideInInspector]
    public float ghostCoins = 0;
    [HideInInspector]
    public bool alreadyDead = false;
    [HideInInspector]
    public bool hasBomb = false;
    [HideInInspector]
    public bool hasPotion = false;
    [HideInInspector]
    public bool canCollectBomb = false;
    [HideInInspector]
    public bool canCollectPotion = false;
    [HideInInspector]
    public Collect collectable;

    private bool dashOnCD = false;
    private bool canTakeDamage = true;
    private float executeHeal = 5f;
    private float executeMoveSpeed = 150f;
    private GameObject executeTarget;
    private bool canMovePlayerForexecute = false;
    private bool ifHopper = false;
    private Vector3 respawnPoint;
    private GameObject corpse;
    private float maxHealth;
    private GameManager gameManager;

    private void Update()
    {
        collect();
        onNoDamage();

        if ((health <= 0 || Input.GetKey(KeyCode.O)) && alreadyDead == false)
        {
            Debug.Log("You Died");

            GhostMode();
            animController.Moving = false;
            animController.dashing = false;
            animController.Attacking = false;
            animController.Dead = true;
            alreadyDead = true;
        }
        if (ghostCoins >= 5)
        {
            gameObject.tag = "Player";

            // move player back to corpse
            transform.position = respawnPoint;

            // change back to normal
            animController.Moving = true;
            animController.Dead = false;
            alreadyDead = false;
            ghostCoins = 0;
            health = maxHealth;

            canTakeDamage = true;
            resetPlayer();

            // delete corpse
            Destroy(corpse);
        }
        //Move();
        Attack();
        Dash();
        UseItem();
    }

    public Transform sprite;

    private void Awake()
    {

        gameManager = GameManager.Instance;

        if (CharacterType != null)
        {
            SetCharacterData();
        }
        maxHealth = health;
    }
    bool dontForward;
    private void FixedUpdate()
    {
        
        //rb.AddForce(new Vector3(0, -12000, 0) * Time.fixedDeltaTime);
        if (canMove && !dashing && sprite != null)
        {
            
            float scale = Mathf.Abs(sprite.localScale.x);
            if (controller.HorizontalMagnitude > 0) { sprite.localScale = new Vector3(-scale, sprite.localScale.y, 1); }
            else if (controller.HorizontalMagnitude < 0) { CharacterFolder.transform.GetChild(0).localScale = new Vector3(scale, sprite.localScale.y, 1); }
        }
        if (hitEnemy)
        {
            Debug.Log(GetMovementVector());
            rb.AddForce((-GetMovementVector()) * attackKnockback * 2400 * Time.fixedDeltaTime,ForceMode.Impulse);
            hitEnemy = false;
            dontForward = true;
        }
        Move();
    }

    public Vector3 GetMovementVector()
    {
        float zInput = controller.ForwardMagnitude;
        float xInput = controller.HorizontalMagnitude;
        Vector3 mV= new Vector3(xInput, 0, zInput);
        return mV;
    }

    public Vector3 movementVector;
    private void Move()
    {
        if (true || !ifHopper)
        {
            float zInput = controller.ForwardMagnitude;
            float xInput = controller.HorizontalMagnitude;
            movementVector = new Vector3(xInput, 0, zInput);
            if (canMove /*&& !GameManager.Instance.Pause/*/)
            {
                
                primaryAttackSpawnPoint.localPosition = (movementVector) * 10;
                primaryAttackSpawnPoint.localRotation = primaryAttackPoint.localRotation;
                primaryAttackPoint.LookAt(primaryAttackSpawnPoint);
                primaryAttackPoint.eulerAngles = new Vector3(0, primaryAttackPoint.eulerAngles.y, 0);

                if (movementVector.magnitude > 1)
                {
                    movementVector.Normalize();
                    
                }
                movementVector.z = movementVector.z * 1.5f;
                movementVector = (movementVector * movementSpeed);

                //rb.MovePosition(rb.position + (movementVector * Time.fixedDeltaTime));
                rb.AddForce(movementVector * 650 * Time.fixedDeltaTime);
                //Debug.Log("canMove");
                //transform.position += (movementVector/2) * Time.deltaTime;

                if ((rb.velocity.magnitude > 30f || movementVector.magnitude > 1) & Mathf.Abs(movementVector.magnitude) > 0)
                {
                    //Debug.Log(movementVector.magnitude);
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
        attackKnockback = CharacterType.attackKnockback;
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
            
            animController.Attacking = true;
            GameObject attack = Instantiate(prefabForAttack, primaryAttackSpawnPoint);
            SwordSlash.sword.Play();

            if (!hitEnemy && !dontForward)
            {
                rb.AddForce(GetMovementVector() * attackKnockback * 4800 * Time.fixedDeltaTime, ForceMode.Impulse);
            }
            else
            {
                dontForward = false;
            }
        }
        

    }

    public void Execute(GameObject enemy)
    {
        StartCoroutine(InExecute(enemy));
    }

    bool lockDash;
    public bool dashing;
    Vector3 dashDirection;
    private void Dash()
    {
        if (controller.DashDown & dashOnCD == false && !lockDash)
        {
            float scale = Mathf.Abs(sprite.localScale.x);
            if (controller.HorizontalMagnitude > 0) { sprite.localScale = new Vector3(-scale, sprite.localScale.y, 1); }
            else if (controller.HorizontalMagnitude < 0) { CharacterFolder.transform.GetChild(0).localScale = new Vector3(scale, sprite.localScale.y, 1); }

            lockDash = true;
            animController.dashing = true;
            animController.Moving = false;

            float zInput = controller.ForwardMagnitude;
            float xInput = controller.HorizontalMagnitude;
            dashDirection = new Vector3(xInput, 0, zInput);
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
        dashing = true;
        
        float startTime = Time.time; // need to remember this to know how long to dash

        
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
            rb.AddForce((dashDirection * dashSpeed * Time.deltaTime)*1200);
            yield return null;
        }

        canAttack = true;
        canTakeDamage = true;
        canMove = true;
        animController.dashing = false;
        lockDash = false;
        dashing = false;
        //Debug.Log("Dash Finish");
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
            controller.sprite.localScale = new Vector3(-scale, controller.sprite.localScale.y, 1); // to swap so its always facing enemy

            canTakeDamage = false;
            canMove = false;
            enemyAIController.canMove = false;
            canMovePlayerForexecute = true;

            yield return new WaitForSeconds(0.75f);
            Destroy(toExecute.transform.parent.gameObject);

            executeCollideScript.enemiesInRange.Remove(toExecute); // remove enemy from list
            health = health + executeHeal;
            canMove = true;
            canTakeDamage = true;
        }
    }

    private void onNoDamage()
    {
        if (controller.NoDamageDown)
        {
            enemyAIController.canMove = !enemyAIController.canMove;
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

    private void GhostMode()
    {
        // instantiate dead sprite at player position
        respawnPoint = transform.position;
        corpse = Instantiate(deathBody, transform.position, Quaternion.identity);
        canTakeDamage = false;

        // turn player sprite to ghost sprite

        gameObject.tag = "Ghost";

        // turn off attacking, dash, and item use,
        resetPlayer();

    }

    private void resetPlayer()
    {
        canAttack = !canAttack;
        canTakeDamage = !canTakeDamage;
        canExecute = !canExecute;
        canUseItem = !canUseItem;
        lockDash = !lockDash;

        Debug.Log("canMove = " + canMove);
        Debug.Log("canAttack = " + canAttack);
        Debug.Log("canTakeDamage = " + canTakeDamage);
        Debug.Log("canExecute = " + canExecute);
        Debug.Log("canUseItem = " + canUseItem);
        Debug.Log("LockDash = " + lockDash);
    }

    private void collect()
    {
        if (canCollectBomb)
        {
            if (controller.InteractDown)
            {
                Debug.Log("pressed interact");

                if (gameManager.eyeCount >= 5 && hasPotion == false && hasBomb == false)
                {
                    gameManager.eyeCount -= 5;
                    hasBomb = true;
                    collectable.gameObject.SetActive(false);

                    Debug.Log("Player has a bomb: " + hasBomb);
                }
            }
        }
        if (canCollectPotion)
        {
            if (controller.InteractDown)
            {
                Debug.Log("pressed interact");

                if (gameManager.eyeCount >= 5 && hasPotion == false && hasBomb == false)
                {
                    gameManager.eyeCount -= 5;
                    hasPotion = true;
                    collectable.gameObject.SetActive(false);

                    Debug.Log("Player has a bomb: " + hasPotion);
                }
            }
        }
    }

    private void UseItem()
    {
        if (controller.ItemDown)
        {
            if (hasBomb)
            {
                // set isThrow to true
            }
            if (hasPotion)
            {
                float healAmount = maxHealth * 0.33f;
                AddHealth(healAmount);
            }
        }
    }
}
