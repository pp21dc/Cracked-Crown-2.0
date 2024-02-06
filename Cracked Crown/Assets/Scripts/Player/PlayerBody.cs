using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine.SceneManagement;
public class PlayerBody : MonoBehaviour
{
    [Header("SET BEFORE USE")]
    public bool IS_IN_TEST_SCENE = true;
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
    [SerializeField]
    private GameObject throwableBombPrefab;

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
    public bool canTakeDamage = true;
    public float executeHeal = 5f;
    private float executeMoveSpeed = 150f;
    private GameObject executeTarget;
    public bool canMovePlayerForexecute = false;
    private bool ifHopper = false;
    private Vector3 respawnPoint;
    private GameObject corpse;
    private float maxHealth;
    private GameManager gameManager;

    //hey Ian dont know where you will want this bool
    private bool canRelease = false;

    private void Update()
    {
        collect();
        onNoDamage();

        if ((health <= 0 || Input.GetKey(KeyCode.O)) && alreadyDead == false)
        {
            Debug.Log("You Died");
            rb.velocity = Vector3.zero;
            //GhostMode();
            animController.Moving = false;
            animController.dashing = false;
            animController.Attacking = false;
            animController.Dead = true;
            alreadyDead = true;
            canMove = false;
            canAttack = false;
            lockDash = true;
        }
        if (ghostCoins >= 5)
        {
            
            gameObject.tag = "Player";

            // move player back to corpse
            transform.position = respawnPoint;

            // change back to normal
            //animController.Moving = true;
            animController.Dead = false;
            alreadyDead = false;
            ghostCoins = 0;
            health = maxHealth;
            canAttack = true;
            canMove = true;
            lockDash = false;

            canTakeDamage = true;
            //RevivePlayer();

            // delete corpse
            //Debug.Log("DESTROY CORPSES");
            Destroy(corpse);
            
        }
        //Move();
        Attack();
        Dash();
        UseItem();
        rb.velocity = new Vector3(rb.velocity.x, (-9.81f)*(rb.mass), rb.velocity.z);

    }

    public Transform sprite;

    private void Awake()
    {
        if (!IS_IN_TEST_SCENE)
        {
            gameManager = GameManager.Instance;
            persistScene = SceneManager.GetSceneByBuildIndex(0);
        }
        if (CharacterType != null)
        {
            SetCharacterData();
        }
        maxHealth = health;
    }
    bool dontForward;
    private void FixedUpdate()
    {
        Debug.Log(rb.velocity);
        //rb.AddForce(new Vector3(0, -12000, 0) * Time.fixedDeltaTime);
        if (canMove && !dashing && sprite != null)
        {
            
            float scale = Mathf.Abs(sprite.localScale.x);
            if (controller.HorizontalMagnitude > 0) { sprite.localScale = new Vector3(-scale, sprite.localScale.y, 1); }
            else if (controller.HorizontalMagnitude < 0) { CharacterFolder.transform.GetChild(0).localScale = new Vector3(scale, sprite.localScale.y, 1); }
        }
        if (hitEnemy)
        {
            //Debug.Log(GetMovementVector());
            float y = rb.velocity.y;
            rb.velocity = ((-GetMovementVector()) * attackKnockback * forceMod/4 * Time.fixedDeltaTime);
            rb.velocity = new Vector3(rb.velocity.x, y, rb.velocity.z);
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
    public float forceMod = 1000;
    private void Move()
    {
        if (canMove)
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
                else if (movementVector.magnitude == 0)
                {
                    rb.velocity = new Vector3(0, rb.velocity.y, 0);
                }
                movementVector.z = movementVector.z * 1.5f;
                movementVector = (movementVector * movementSpeed);

                float y = rb.velocity.y;
                rb.velocity = (movementVector * forceMod * Time.fixedDeltaTime);
                rb.velocity = new Vector3(rb.velocity.x, y, rb.velocity.z);
                

                if ((rb.velocity.magnitude > 30f || movementVector.magnitude > 1) & Mathf.Abs(movementVector.magnitude) > 0 && !alreadyDead)
                {
                    //Debug.Log(alreadyDead);
                    animController.Moving = true;
                }
                else
                {
                    animController.Moving = false;
                }
            }
            
        }
        if (canMovePlayerForexecute && executeTarget != null)
        {
            if (Vector3.Distance(transform.position, executeTarget.transform.position + forExecutePosition) < 1 && !lockExecAnim)
            {
                animController.Finishing = true;
                lockExecAnim = true;
            }
            Debug.Log(Vector3.Distance(transform.position, executeTarget.transform.position + forExecutePosition));
            transform.position = Vector3.MoveTowards(gameObject.transform.position, executeTarget.transform.position + forExecutePosition, executeMoveSpeed * Time.deltaTime);
        }
    }
    public bool lockExecAnim;

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
        deathBody = CharacterType.corpse;
    }
    float x = 0;
    float attackTimer = 0;
    float attackTime = 0.1f;
    private void Attack()
    {
        if (controller.PrimaryAttackDown & canAttack)
        {
            canMove = false;
            canAttack = false;
            

            animController.Attacking = true;
            GameObject attack = Instantiate(prefabForAttack, primaryAttackSpawnPoint);
            SwordSlash.sword.Play();

            if (!hitEnemy && !dontForward)
            {
                float y = rb.velocity.y;
                rb.velocity = (GetMovementVector() * attackKnockback * (forceMod / 2.5f) * Time.deltaTime);
                rb.velocity = new Vector3(rb.velocity.x, y, rb.velocity.z);
                

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
            Debug.Log("DASHING");
            rb.velocity = ((dashDirection * dashSpeed * forceMod * 2) * Time.deltaTime);
            yield return new WaitForEndOfFrame();
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
            enemyAIController = toExecute.transform.parent.GetChild(0).GetComponent<EnemyAIController>();
            executeTarget = toExecute;
            float xInput = controller.HorizontalMagnitude;

            controller.sprite = CharacterFolder.transform.GetChild(0);
            float scale = Mathf.Abs(controller.sprite.localScale.x);
            controller.sprite.localScale = new Vector3(scale, controller.sprite.localScale.y, 1); // to swap so its always facing enemy

            canTakeDamage = false;
            canMove = false;
            enemyAIController.canMove = false;
            canMovePlayerForexecute = true;
            //Debug.Log("EXEC");
            
            toExecute.transform.parent.gameObject.SetActive(false);
            
            yield return new WaitForSeconds(1);

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
            //Debug.Log(health);
        }
    }
    public int RESETINGGHOST; //gets the state of the players death cycle
    [SerializeField]
    private Scene persistScene;
    public void GhostMode()
    {
        if (RESETINGGHOST == 2)
        {
            //Debug.Log("GHOSTMODE");
            RESETINGGHOST += 1;
            // instantiate dead sprite at player position
            respawnPoint = transform.position;

            GameObject c = Instantiate(deathBody, transform.position, Quaternion.identity);
            SceneManager.MoveGameObjectToScene(c, persistScene);
            corpse = c;
            canTakeDamage = false;
            canMove = true;

            // turn player sprite to ghost sprite

            gameObject.tag = "Ghost";

            // turn off attacking, dash, and item use,
            GhostPlayer();
        }
    }

    public void GhostPlayer()
    {
        canAttack = false;
        canTakeDamage = false;
        canExecute = false;
        canUseItem = false;
        lockDash = true;
        canMove = true;
        //RESETINGGHOST = 0;
    }

    public void RevivePlayer()
    {
        Debug.Log("REVIVE");
        canAttack = true;
        canTakeDamage = true;
        canExecute = true;
        canUseItem = true;
        lockDash = false;
        canMove = true;
        RESETINGGHOST = 0;
    }

    public void resetPlayer()
    {
        canAttack = !canAttack;
        canTakeDamage = !canTakeDamage;
        canExecute = !canExecute;
        canUseItem = !canUseItem;
        lockDash = !lockDash;
        RESETINGGHOST = 0;

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
                Vector3 fortniteFellaBalls = transform.position + movementVector;
                GameObject bomb = Instantiate(throwableBombPrefab, fortniteFellaBalls, Quaternion.identity);
                Bomb reference = bomb.GetComponent<Bomb>();
                reference.setDirection(movementVector);

                hasBomb = false;
            }
            if (hasPotion)
            {
                float healAmount = maxHealth * 0.33f;
                AddHealth(healAmount);
                Debug.Log("Player health is: " + health);
                hasPotion = false;
            }
        }
    }

    public bool StartSpam()
    {
        canMove = false;
        canAttack = false;

        StartCoroutine(spamX());

        if(canRelease == true)
        {
            canRelease = false;
            return true;
        }
        else 
        {
            return false;
        }

    }

    IEnumerator spamX()
    {
        int timesHit = 0;

        while(timesHit < 8)
        {
            if(controller.InteractDown)
            {
                timesHit++;
            }
        }

        canRelease = true;
        canMove = true;
        canAttack = true;

        yield return null;
    }
}
