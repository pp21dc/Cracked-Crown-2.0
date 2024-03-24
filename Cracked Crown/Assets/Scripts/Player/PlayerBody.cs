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
    private float health = 50f;
    [SerializeField]
    private GameObject deathBody;

    public int lockIN = -1;
    public int currentIN;

    [Header("CHARACTER DESIGN")]
    public float KnockbackTime = 0.025f;
    public float KnockForwardTime = 0.025f;
    [SerializeField]
    private float attackKnockback = 100;
    public float takenDamageKnockback = 1000;
    public float Health { get { return health; } }
    public float damage = 3f;

    [SerializeField]
    public CharacterType CharacterType;

    [Header("Do Not Touch")]
    [SerializeField]
    private PlayerController controller;
    public PlayerContainer playerContainer;
    [SerializeField]
    public PlayerAudioManager PAM;
    public PlayerManager PM;
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
    public EnemyAIController eac_cur;
    [SerializeField]
    private GameObject throwableBombPrefab;
    [SerializeField]
    public SpriteRenderer spriteRenderer;
    [SerializeField]
    private CrabWalk crabController;

    public GameObject CharacterFolder;
    public bool canMove = true;
    public PlayAnim SwordSlash;

    public bool canAttack = true;
    [HideInInspector]
    public bool hitEnemy = false;
    [HideInInspector]
    public bool canUseItem = true;
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
    public bool canCollect = true;

    
    public bool gotHit = false;
    private bool dashOnCD = false;
    public bool canTakeDamage = true;
    public float executeHeal;
    private float executeMoveSpeed = 150f;
    private GameObject executeTarget;
    public bool canMovePlayerForexecute = false;
    private bool ifHopper = false;
    private Vector3 respawnPoint;
    private GameObject corpse;
    private float maxHealth;
    private GameManager gameManager;
    private float attackSpeed;
    private float moveCooldown;
    public bool dashQueue = false;
    public bool playerLock = false;
    private bool enumDone = false;
    private float attackDelayTime;
    public int timesHit = 0;
    public Vector3 AttackVector;
    public bool Grabbed = false;
    public GameObject dropShadow;
    //hey Ian dont know where you will want this bool
    public bool canRelease = false;
    public int playerID;

    private bool hasReachedExecutePoint = false;
    private bool neverReachedExecutePoint = false;

    public void ResetPlayer()
    {
        health = maxHealth;
        canAttack = true;
        canCollect = true;
        canCollectBomb = false;
        canCollectPotion = false;
        canExecute = true;
        canMove = true;
        canTakeDamage = true;
        playerLock = false;
        Grabbed = false;
        ghostCoins = 20;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.C))
            hasBomb = true;
        if (Input.GetKey(KeyCode.K))
            ghostCoins += 5;

        if (sprite != null)
        {
            if (sprite.localScale.x < 0)
            {
                lookingLeft = false;
                lookingRight = true;
            }
            if (sprite.localScale.x > 0)
            {
                lookingRight = false;
                lookingLeft = true;
            }
        }

        Vector3 dropShadowPos;
        if (CharacterType != null)
        {
            if (CharacterType.ID != 2)
                dropShadowPos = new Vector3(transform.position.x, -2f, transform.position.z);
            else
            {
                dropShadowPos = new Vector3(transform.position.x, -4f, transform.position.z);
            }

            dropShadow.transform.position = dropShadowPos;
        }
        if (canMovePlayerForexecute && enemyAIController != null)
        {
            dropShadow.SetActive(false);
            enemyAIController.dropShadow.SetActive(false);
        }
        else
        {
            dropShadow.SetActive(true);
        }

        if (!playerLock)
        {
            if (PM != null)
            {
                if (controller.InteractDown && lockIN == -1 && PM.CheckPlayers(currentIN))
                    lockIN = currentIN;
                else if (controller.InteractDown && lockIN != -1)
                    lockIN = -1;
            }
            collect();
            onNoDamage();



            if ((health <= 0 || Input.GetKey(KeyCode.O)) && alreadyDead == false)
            {
                rb.velocity = Vector3.zero;
                animController.Moving = false;
                animController.dashing = false;
                animController.Attacking = false;
                animController.Dead = true;
                alreadyDead = true;
                canMove = false;
                canAttack = false;
                lockDash = true;
                StartCoroutine(deathAnim());
            }
            if (alreadyDead && enumDone)
            {
                canMove = true;
                canAttack = false;
                lockDash = true;
                canExecute = false;
            }
            if (ghostCoins >= 5)
            {

                gameObject.tag = "Player";

                // move player back to corpse
                transform.position = respawnPoint;

                // change back to normal
                animController.Dead = false;
                alreadyDead = false;
                ghostCoins = 0;
                health = maxHealth*0.8f;
                canAttack = true;
                lockDash = false;
                canTakeDamage = true;
                Grabbed = false;

                // delete corpse
                Destroy(corpse);
                StartCoroutine(executeAfterRevive());

            }
            Attack();
            Dash();
            UseItem();

            if (transform.position.y < 2)
                vely = 0;
            vely += -9.81f;

            rb.velocity = new Vector3(rb.velocity.x, vely, rb.velocity.z);
        }
    }
    float vely = 0;
    private IEnumerator executeAfterRevive()
    {
        yield return new WaitForSeconds(1.2f);
        canExecute = true;
        canCollect = true;

        yield return new WaitForSeconds(2.0f);
        canMove = true;
    }

    private IEnumerator deathAnim()
    {
        enumDone = false;
        yield return new WaitForSeconds(2f);
        canMove = true;
        enumDone = true;
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
        health = 50;
        maxHealth = health;
        executeHeal = maxHealth * (1.0f / 2.0f);
        canCollect = true;

        playerID = gameObject.GetInstanceID();
    }
    bool dontForward;
    bool lookingRight = false;
    bool lookingLeft = false;
    private void FixedUpdate()
    {
        if (!playerLock)
        {
            if (canMove && !dashing && sprite != null && !canMovePlayerForexecute)
            {

                float scale = Mathf.Abs(sprite.localScale.x);
                if (controller.HorizontalMagnitude > 0) 
                {
                    sprite.localScale = new Vector3(-scale, sprite.localScale.y, 1);
                }
                else if (controller.HorizontalMagnitude < 0) 
                { 
                    CharacterFolder.transform.GetChild(0).localScale = new Vector3(scale, sprite.localScale.y, 1);
                }
            }

            Move();
        }
        else
        {
            animController.Moving = false;
        }
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
    Vector3 noY = new Vector3(1,0,1);
    private bool executeLock = false;
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
                else if (movementVector.magnitude == 0 && !lockHitForward && !gotHit)
                {
                    rb.velocity = new Vector3(0, rb.velocity.y, 0);
                }
                movementVector.z = movementVector.z * 1.5f;
                movementVector = (movementVector * movementSpeed);

                if (!lockHitForward && !gotHit)
                {
                    float y = rb.velocity.y;
                    rb.velocity = (movementVector * forceMod * Time.fixedDeltaTime);
                    rb.velocity = new Vector3(rb.velocity.x, y, rb.velocity.z);
                }


                if ((rb.velocity.magnitude > 30f || movementVector.magnitude > 1) & Mathf.Abs(movementVector.magnitude) > 0 && !alreadyDead)
                {
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
            if (Vector3.Distance(transform.position, executeTarget.transform.position + forExecutePosition) < 2.5f && !lockExecAnim && enemyAIController != null)
            {
                hasReachedExecutePoint = true;

                animController.Finisher(enemyAIController.tag, enemyAIController.colour, true);

                lockExecAnim = true;
                if (executeLock == false)
                {
                    StartCoroutine(TurnOffExecuteMovement());
                }
            }
            else if (Vector3.Distance(transform.position, executeTarget.transform.position + forExecutePosition) > 1.0f)
            {
                Vector3 epicgamer = executeTarget.transform.position + forExecutePosition;
                rb.velocity = Vector3.zero;
                Vector3 test = Vector3.MoveTowards(gameObject.transform.position, epicgamer, executeMoveSpeed * Time.fixedDeltaTime);
                test.y = 0;
                transform.position = test;
                lockDash = true;
                canMove = false;

                if (enemyAIController != null)
                {
                    if (enemyAIController.tag.Equals("Light"))
                    {
                        animController.Finisher(enemyAIController.tag, enemyAIController.colour, true);
                    }
                }

                if (executeLock == false)
                {
                    StartCoroutine(TurnOffExecuteMovement());
                }
            }
        }
        if (dashing)
        {
            float dz = dashDirection.z;
            dashDirection.z *= 1.1f;
            rb.velocity = (new Vector3(0, rb.velocity.y) + ((dashDirection * dashSpeed * forceMod * 0.9f)) * Time.fixedDeltaTime);
            dashDirection.z = dz;
        }
        if (!hitEnemy && (lockHitForward))
        {

            //Debug.Log("FORWARD");
            float y = rb.velocity.y;
            rb.velocity = (GetMovementVector() * attackKnockback);
            rb.velocity = new Vector3(rb.velocity.x, y, rb.velocity.z);
        }
        else if (hitEnemy && lockHitBackward)
        {
            //Debug.Log("BACK");
            float y = rb.velocity.y;
            rb.velocity = (-GetMovementVector() * attackKnockback);
            rb.velocity = new Vector3(rb.velocity.x, y, rb.velocity.z);
        }
        else if (canTakeDamage && gotHit)
        {
            float y = rb.velocity.y;
            Debug.Log(otherPlayerMV);
            rb.velocity = (otherPlayerMV * takenDamageKnockback);
            rb.velocity = new Vector3(rb.velocity.x, y, rb.velocity.z);
            StartCoroutine(GotHitReset());
        }
    }

    IEnumerator GotHitReset()
    {
        yield return new WaitForSeconds(KnockbackTime);
        gotHit = false;
    }

    private IEnumerator TurnOffExecuteMovement()
    {

        yield return new WaitForSeconds(1.1f);

        canMovePlayerForexecute = false;
        canExecute = true;
        canTakeDamage = true;
        canAttack = true;
        executeLock = false;

        animController.Finisher(enemyAIController.tag, enemyAIController.colour, true);

        yield return new WaitForSeconds(1.5f);

        lockDash = false;
        canMove = true;

    }

    public bool lockExecAnim;

    private IEnumerator DamageColourFlash()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
        }
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.25f);
        spriteRenderer.color = Color.white;
    }

    public void DecHealth(float amount) 
    {
        StartCoroutine(DamageColourFlash());
        if (canTakeDamage)
        {
            PAM.PlayAudio(PlayerAudioManager.AudioType.PlayerHit);
            health = Mathf.Max(0, health - amount); // allows taking health from the player
        }
    }
    public void AddHealth(float amount) 
    {
        health = Mathf.Min(maxHealth, health + amount); //allows us to add health to the player
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
        attackSpeed = CharacterType.attackSpeed;
        moveCooldown = CharacterType.moveCd;
        ifHopper = CharacterType.hop;
        attackKnockback = CharacterType.attackKnockback;
        deathBody = CharacterType.corpse;
        attackDelayTime = CharacterType.attackDelayTime;
    }
    float x = 0;
    float attackTimer = 0;
    float attackTime = 0.1f;
    public bool lockHitForward;
    bool lockHitBackward;
    bool attacking;
    private void Attack()
    {
        if (controller.PrimaryAttackDown && canAttack && !dashing)
        {
            canMove = false;
            canAttack = false;
            attacking = true;

            animController.Attacking = true;

            StartCoroutine(attackDelay());
            StartCoroutine(attackCooldown());
            StartCoroutine(attackMoveCooldown());
        }
        

    }

    private IEnumerator attackDelay()
    {
        AttackVector = GetMovementVector();
        if (AttackVector == Vector3.zero)
        {
            if (controller.HorizontalMagnitude == 0 && lookingRight)
            {
                AttackVector =  new Vector3(1, 0, 0);
            }
            if (controller.HorizontalMagnitude == 0 && lookingLeft)
            {
                AttackVector = new Vector3(-1, 0, 0);
            }
        }

        yield return new WaitForSeconds(attackDelayTime);
        GameObject attack = Instantiate(prefabForAttack, primaryAttackSpawnPoint.transform);
        attack.GetComponent<PrototypePrimaryAttack>().playerBody = this;
        
        SwordSlash.sword.Play();
        if (!hitEnemy && !lockHitForward)
        {
            StartCoroutine(forwardHit());
        }
        if (hitEnemy && !lockHitBackward)
        {
            if (!eac_cur.EAC.Dead)
                PAM.PlayAudio(PlayerAudioManager.AudioType.EnemyHit);
            StartCoroutine(backwardHit());
        }
        animController.Attacking = false;
    }
    
    private IEnumerator forwardHit()
    {
        lockHitForward = true;
        yield return new WaitForSeconds(KnockForwardTime);
        lockHitForward = false;
    }
    Vector3 otherPlayerMV = Vector3.zero;
    public IEnumerator backwardHit()
    {
        lockHitBackward = true;
        dontForward = false;
        yield return new WaitForSeconds(KnockbackTime);
        dontForward = true;
        lockHitBackward = false;
        hitEnemy = false;
    }

    public IEnumerator gotHitKnockback(Vector3 movementVect)
    {
        otherPlayerMV = Vector3.zero;
        otherPlayerMV = movementVect;
        gotHit = true;
        dontForward = false;
        yield return new WaitForSeconds(KnockbackTime);
        dontForward = true;
        gotHit = false;
        lockHitForward = false;
        Debug.Log("COT");
    }

    private IEnumerator attackCooldown()
    {
        yield return new WaitForSeconds(attackSpeed * 0.95f);
        attacking = false;
        yield return new WaitForSeconds(attackSpeed * 0.05f);
        canAttack = true;
        AttackVector = Vector3.zero;
    }

    private IEnumerator attackMoveCooldown()
    {
        yield return new WaitForSeconds(moveCooldown);
        canMove = true;
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
        if (((controller.DashDown && dashOnCD == false) || (!controller.DashDown && dashQueue && !dashing && !dashOnCD)) && !attacking && !lockDash && sprite != null)
        {
            float scale = Mathf.Abs(sprite.localScale.x);
            if (!dashQueue)
            {
                if (controller.HorizontalMagnitude > 0) { sprite.localScale = new Vector3(-scale, sprite.localScale.y, 1); }
                else if (controller.HorizontalMagnitude < 0) { CharacterFolder.transform.GetChild(0).localScale = new Vector3(scale, sprite.localScale.y, 1); }
            }
            else
            {
                if (dashDir.x > 0) { sprite.localScale = new Vector3(-scale, sprite.localScale.y, 1); }
                else if (dashDir.x < 0) { CharacterFolder.transform.GetChild(0).localScale = new Vector3(scale, sprite.localScale.y, 1); }
            }
            lockDash = true;
            animController.dashing = true;
            animController.Moving = false;

            float zInput = controller.ForwardMagnitude;
            float xInput = controller.HorizontalMagnitude;

            if (!dashQueue)
            {
                dashDirection = new Vector3(xInput, 0, zInput);
            }
            else
                dashDirection = dashDir;
            
            StartCoroutine(DashCoroutine());
            
            dashOnCD = true;
            StartCoroutine(Cooldown());
        }
        if (controller.DashDown && !dashQueue)
        {
            dashQueue = true;
            dashDir = GetMovementVector();
            StartCoroutine (dashQueueTimer());
        }
    }
    private Vector3 dashDir;
    private IEnumerator dashQueueTimer()
    {
        yield return new WaitForSeconds(0.3f);
        dashQueue = false;
    }

    bool skip;
    private IEnumerator DashCoroutine()
    {  
        skip = false;

        if (dashDirection.magnitude > 0f)
        {
            dashDirection.Normalize();
        }
        else
        {
            dashDirection = new Vector3(0, 0, 0);
            skip = true;
        }

        if (!skip)
        {
            canTakeDamage = false;
            canAttack = false;
            canMove = false;
            dashing = true;
            
            yield return new WaitForSeconds(dashTime);
        }

        canAttack = true;
        canTakeDamage = true;
        canMove = true;
        animController.dashing = false;
        lockDash = false;
        dashing = false;
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(0.2f);
        dashOnCD = false;
    }

    private IEnumerator InExecute(GameObject toExecute)
    {
        if (toExecute != null && canExecute && toExecute.transform.position.y <= 8.0f)
        {
            if (toExecute.transform.parent.GetChild(0).GetComponent<EnemyAIController>() != null)
            {
                enemyAIController = toExecute.transform.parent.GetChild(0).GetComponent<EnemyAIController>();
                executeTarget = toExecute;
                float xInput = controller.HorizontalMagnitude;

                controller.sprite = CharacterFolder.transform.GetChild(0);
                float scale = Mathf.Abs(controller.sprite.localScale.x);
                controller.sprite.localScale = new Vector3(scale, controller.sprite.localScale.y, 1); // to swap so its always facing enemy

                canTakeDamage = false;
                canMove = false;
                canAttack = false;
                enemyAIController.canMove = false;
                canExecute = false;
                executeLock = true;
                lockDash = true;
                canMovePlayerForexecute = true;

                yield return new WaitForSeconds(0);
                toExecute.transform.parent.gameObject.SetActive(false);
                if (gameManager != null)
                {
                    if (enemyAIController.tag == "Light")
                        gameManager.eyeCount += 2;
                    else if (enemyAIController.tag == "Medium")
                        gameManager.eyeCount += 5;
                    else if (enemyAIController.tag == "Heavy")
                        gameManager.eyeCount += 8;
                    if (LevelManager.Instance != null)
                        LevelManager.Instance.EnemyKilled();
                }
                yield return new WaitForSeconds(1.4f);

                canTakeDamage = true;
                canMove = true;
                canAttack = true;
                canExecute = true;
                canMovePlayerForexecute = false;
                lockDash = false;

            }
            else
            {
                crabController = toExecute.transform.parent.GetChild(0).GetComponent<CrabWalk>();
                executeTarget = toExecute;
                float xInput = controller.HorizontalMagnitude;

                controller.sprite = CharacterFolder.transform.GetChild(0);
                float scale = Mathf.Abs(controller.sprite.localScale.x);
                controller.sprite.localScale = new Vector3(scale, controller.sprite.localScale.y, 1); // to swap so its always facing enemy

                if (crabController.hasDied == false)
                {
                    canTakeDamage = false;
                    canMove = false;
                    canAttack = false;
                    canExecute = false;
                    canMovePlayerForexecute = true;
                    lockDash = true;

                    // turn player sprite off for a second while animation because animation is on crab
                    transform.parent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);

                    // turn trigger for execute animation on
                    if (CharacterType.ID == 1)
                    {
                        crabController.animator.SetBool("BunnyExecute", true);
                    }
                    if (CharacterType.ID == 3)
                    {
                        crabController.animator.SetBool("FrogExecute", true);
                    }

                    yield return new WaitForSeconds(1.1f);
                   
                    canTakeDamage = true;
                    canMove = true;
                    canAttack = true;
                    canExecute = true;
                    canMovePlayerForexecute = false;
                    lockDash = false;
                    transform.parent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
                    toExecute.transform.parent.gameObject.SetActive(false);
                }
            }
        }
    }

    private void onNoDamage()
    {
        if (controller.NoDamageDown && enemyAIController != null)
        {
            enemyAIController.canMove = !enemyAIController.canMove;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            health = health - 1;
        }
        if (collision.gameObject.tag == "Revive" && gameObject.tag == "Ghost")
        {
            Debug.Log("Collided With Revive Stone as Ghost");

            gameObject.tag = "Player";

            transform.position = respawnPoint;

            animController.Dead = false;
            alreadyDead = false;
            ghostCoins = 0;
            health = maxHealth * 0.8f;
            canAttack = true;
            canMove = true;
            lockDash = false;
            canTakeDamage = true;
            Grabbed = false;

            Destroy(corpse);
            StartCoroutine(executeAfterRevive());
        }
    }
    public int RESETINGGHOST; //gets the state of the players death cycle
    [SerializeField]
    private Scene persistScene;
    public void GhostMode()
    {
        if (RESETINGGHOST == 2)
        {
            RESETINGGHOST += 1;
            // instantiate dead sprite at player position
            respawnPoint = transform.position;

            canMove = false;

            GameObject c = Instantiate(deathBody, transform.position, Quaternion.identity);
            SceneManager.MoveGameObjectToScene(c, persistScene);
            corpse = c;
            canTakeDamage = false;

            // turn player sprite to ghost sprite

            gameObject.tag = "Ghost";

            // turn off attacking, dash, and item use,
            GhostPlayer();
        }
    }

    private IEnumerator resetCanMoveOnRevive()
    {
        canMove = false;
        yield return new WaitForSeconds(1.0f);
    }

    public void GhostPlayer()
    {
        canAttack = false;
        canTakeDamage = false;
        canExecute = false;
        canUseItem = false;
        lockDash = true;
        gameObject.layer = 7;
        canCollect = true;
        StartCoroutine(resetCanMoveOnRevive());
    }

    public void RevivePlayer()
    {
        canMove = true;

        //Debug.Log("REVIVE");
        canAttack = true;
        canTakeDamage = true;
        canExecute = true;
        canUseItem = true;
        lockDash = false;
        RESETINGGHOST = 0;
        gameObject.layer = 3;
        canCollect = true;
    }

    private void collect()
    {
        if (canCollectBomb)
        {
            if (controller.InteractDown)
            {
                if (gameManager.eyeCount >= 30 && hasPotion == false && hasBomb == false)
                {
                    gameManager.eyeCount -= 30;
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
                if (gameManager.eyeCount >= 30 && hasPotion == false && hasBomb == false)
                {
                    gameManager.eyeCount -= 30;
                    hasBomb = false;
                    hasPotion = true;
                    collectable.gameObject.SetActive(false);

                    Debug.Log("Player has a bomb: " + hasPotion);
                }
            }
        }
    }

    private void UseItem()
    {

        Vector3 stinky = new Vector3 (7.5f, 5, 0); // so it spawns above the ground and infront
        Vector3 negativeStinky = new Vector3 (-7.5f, 5, 0);
        Vector3 fortniteFellaBalls = Vector3.zero; // placeholder

        if (controller.ItemDown)
        {
            if (hasBomb && !Grabbed)
            {
                if (controller.HorizontalMagnitude >= 0)
                {
                    fortniteFellaBalls = transform.position + movementVector + stinky;
                }
                else if (controller.HorizontalMagnitude < 0)
                {
                    fortniteFellaBalls = transform.position + movementVector + negativeStinky;
                }

                GameObject bomb = Instantiate(throwableBombPrefab, fortniteFellaBalls, Quaternion.identity);
                Bomb reference = bomb.GetComponent<Bomb>();
                reference.setDirection(movementVector);
                reference.SetController(controller);

                hasBomb = false;
            }
            if (hasPotion & !Grabbed)
            {
                float healAmount = maxHealth * 0.75f;
                AddHealth(healAmount);
                Debug.Log("Player health is: " + health);
                hasPotion = false;
            }
        }
    }

    public void StartSpam()
    {
        canMove = false;
        canAttack = false;
        
        if (controller.InteractDown)
        {
            timesHit++;
            Debug.Log(timesHit);
        }

        if (timesHit >= 8)
        {
            canRelease = true;
            canMove = true;
            canAttack = true;
            lockDash = false;
            canExecute = true;
            Grabbed = false;
            Debug.Log("Free");
                
        }
    }

    private IEnumerator ExecuteCooldown()
    {
        yield return new WaitForSeconds(1.25f);
        lockDash = false;
        canMove = true;
        canAttack = true;
        canExecute = true;
        canMovePlayerForexecute = false;
        lockExecAnim = false;
    }

    public void ResetSprite(string msg)
    {
        Debug.Log("RESET SPRITE " + msg);
        if (spriteRenderer.enabled == true)
            spriteRenderer.enabled = false;
        else if(spriteRenderer.enabled == false)
            spriteRenderer.enabled = true;

        canMove = true;
        canAttack = true;
    }
    float i = 0;
    public void MoveToEnemy(GameObject enemyBody)
    {
        while (i < 5)
        {
            i += Time.deltaTime;
            gameObject.transform.position = new Vector3(enemyBody.transform.position.x, enemyBody.transform.position.y - 5, enemyBody.transform.position.z);
        }
        i = 0;
    }

}
