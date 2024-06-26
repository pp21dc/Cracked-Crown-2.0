using System;
using System.Collections;
using System.Collections.Generic;
//using System.Data;
//using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.UIElements;
using UnityEngine.UI;
//using static UnityEngine.EventSystems.EventTrigger;
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
    public float health = 50f;
    [SerializeField]
    private GameObject deathBody;

    public int lockIN = -1;
    public int currentIN = -1;

    [Header("CHARACTER DESIGN")]
    public float KnockbackTime = 0.025f;
    public float KnockForwardTime = 0.025f;
    [SerializeField]
    public float attackKnockback = 100;
    public float takenDamageKnockback = 100;
    public float Health = 0;
    public float damage = 3f;

    [SerializeField]
    public CharacterType CharacterType;

    [Header("Do Not Touch")]
    [SerializeField]
    public PlayerController controller;
    public PlayerContainer playerContainer;
    [SerializeField]
    public PlayerAudioManager PAM;
    public PlayerManager PM;
    public FinisherCollider executeCollideScript;
    [SerializeField]
    public PlayerAnimController animController;
    [SerializeField]
    public Rigidbody rb;
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
    [SerializeField]
    private Animator healAnimator;

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

    public Slider ghostCoinSlider;
    public GameObject ghostCoinSlider_obj;
    
    public bool gotHit = false;
    public bool dashOnCD = false;
    public bool canTakeDamage = true;
    public float executeHeal;
    private float executeMoveSpeed = 150f;
    private GameObject executeTarget;
    public bool canMovePlayerForexecute = false;
    private bool ifHopper = false;
    private Vector3 respawnPoint;
    public GameObject corpse;
    public Vector3 resStonePos;
    public float maxHealth;
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

    int swings;
    float swingRecoverTime;

    private bool hasReachedExecutePoint = false;
    private bool neverReachedExecutePoint = false;
    private Animator walrusAnimator;

    [SerializeField]
    private GameObject sparkleObject;
    public Scoreboard scoreboard;
    public void ResetPlayer(bool seAll)
    {
        if (seAll)
        {
            animController.SetAll();
            StopAllCoroutines();
        }
        health = maxHealth;
        alreadyDead = true;
        canAttack = true;
        canCollect = true;
        canCollectBomb = false;
        canCollectPotion = false;
        hasBomb = false;
        hasPotion = false;
        canExecute = true;
        canMove = true;
        canTakeDamage = true;
        playerLock = false;
        Grabbed = false;
        ghostCoins = 0;
        reving = false;
        canMovePlayerForexecute = false;
        canUseItem = true;
        gotHit = false;
        timesHit = 0;
        lockDash = false;
        dashing = false;
        Grabbed = false;
        dashOnCD = false;
        gameObject.tag = "Player";
        gameObject.layer = 3;
    }

    private void Update()
    {
        if (alreadyDead) 
        {
            ghostCoinSlider_obj.SetActive(true);
            ghostCoinSlider.value = ghostCoins;
        }
        else
        {
            ghostCoinSlider.value = 0;
            ghostCoinSlider_obj.SetActive(false);
        }

        //scoreboard.LogAll();

        Health = health;
        if (Input.GetKeyUp(KeyCode.C))
            hasBomb = true;
        if (Input.GetKeyUp(KeyCode.K))
            ghostCoins = 10;
        if (Input.GetKey(KeyCode.I))
            gameManager.eyeCount += 1;

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
                dropShadowPos = new Vector3(transform.position.x, -2.25f, transform.position.z);
            else
            {
                dropShadowPos = new Vector3(transform.position.x, -3.25f, transform.position.z);
            }
            if (CharacterType.ID == 0)
            {
                dropShadow.transform.localScale = new Vector3(17.5f, 3.75f, 1);
            }
            else
            {
                dropShadow.transform.localScale = new Vector3(11.5f, 2.5f, 0.45f);
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

        if (Grabbed)
            playerLock = true;
        
        DeathAndRevive();
        if (!playerLock && !gameManager.waitforvideo && !gameManager.isLoading)
        {
            if (PM != null && PM.isActiveAndEnabled)
            {
                if (controller.InteractDown && lockIN == -1 && PM.CheckPlayers(currentIN))
                {
                    lockIN = currentIN;
                    PM.LockIN(true);
                }
                else if (controller.DashDown && lockIN != -1)
                {
                    lockIN = -1;
                    PM.LockIN(false);
                }
            }
            collect();
            onNoDamage();
            Attack();
            Dash();
            UseItem();

            if (Grabbed && !lockRelease)
            {
                lockRelease = true;
                //StartCoroutine(Release());
            }
            if (transform.position.y < 2.5f || Grabbed)
            {
                animController.Falling = false;
            }
            else if (!alreadyDead)
            {
                animController.Falling = true;
            }
            vely += -250 * Time.deltaTime;
            if (transform.position.y < 0.2f)
                vely = 0;
            rb.velocity = new Vector3(rb.velocity.x, vely, rb.velocity.z);

            if (!alreadyMovedCorpse && gameManager.currentLevelName == "GreenShop" || gameManager.currentLevelName == "RedShop" || gameManager.currentLevelName == "PurpleShop")
            {
                GameObject resStone = GameObject.FindGameObjectWithTag("Revive");

                if (resStone != null)
                    resStonePos = new Vector3(resStone.transform.parent.position.x, 0f, resStone.transform.parent.position.z - 20.0f);
                respawnPoint = resStonePos;

                if (corpse != null)
                {
                    alreadyMovedCorpse = true;
                    corpse.transform.position = resStonePos;
                }
            }
            if (gameManager.currentLevelName == "GreenShop" || gameManager.currentLevelName == "RedShop" || gameManager.currentLevelName == "PurpleShop")
            {
                GameObject walrus = GameObject.FindGameObjectWithTag("Walrus");
                if (walrus != null && walrusAnimator == null)
                {
                    walrusAnimator = walrus.GetComponent<Animator>();
                }
            }
        }
    }
    float vely = 0;
    bool alreadyMovedCorpse = false;

    private void DeathAndRevive()
    {
        if ((health <= 0 || Input.GetKey(KeyCode.O)) && alreadyDead == false)
        {
            scoreboard.Deaths++;
            gameObject.tag = "Ghost";
            gameObject.layer = 7;
            rb.velocity = Vector3.zero;
            animController.Moving = false;
            animController.dashing = false;
            animController.Attacking = false;
            animController.Dead = true;
            alreadyDead = true;
            canMove = false;
            canAttack = false;
            canExecute = false;
            canTakeDamage = false;
            lockDash = true;
            Grabbed = false;
            StartCoroutine(deathAnim());
            StartCoroutine(resetCanMoveOnRevive());
        }
        if (alreadyDead && enumDone)
        {
            //canMove = true;
            canAttack = false;
            lockDash = true;
            canExecute = false;
        }
        if (health > 0 && alreadyDead && !reving)
            ghostCoins = 10;
        if (ghostCoins >= 10 && alreadyDead && !reving)
        {
            reving = true;
            ghostCoins = 0;

            if (canMove && gameManager.currentLevelName != gameManager.MainMenuName)
            {
                StartCoroutine(executeAfterRevive(false));
            }
            else if (gameManager.currentLevelName == gameManager.MainMenuName)
            {
                StartCoroutine(executeAfterRevive(true));
            }
            

        }
    }
    public bool reving;
    public void ExitLevel()
    {
        StopPlayer(true);
        //spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);
        StartCoroutine(FadeSprite(0.001f, 0.5f));
    }
    public int timesSwung = 0;
    bool waitingforImproved;
    IEnumerator AttackImproved()
    {
        canAttack = false;
        waitingforImproved = true;
        yield return new WaitForSeconds(swingRecoverTime);
        canAttack = true;
        waitingforImproved = false;
        attackImpLock = false;
    }

    public void EnterLevel()
    {
        StopPlayer(false);
        StopAllCoroutines();
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
        GameManager.Instance.FreezePlayers(false);
        rb.isKinematic = false;
    }

    public void StopPlayer(bool kine)
    {
        playerLock = kine;
        rb.isKinematic = kine;
        rb.velocity = Vector3.zero;
    }

    private IEnumerator FadeSprite(float to, float speed)
    {
        if (to < 0.5f)
        {
            while (spriteRenderer != null && spriteRenderer.color.a >= to)
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a - (speed * Time.deltaTime));
                dropShadow.SetActive(false);
                yield return new WaitForEndOfFrame();
            }
            
        }
        else
        {
            while (spriteRenderer != null && spriteRenderer.color.a <= to)
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a + (speed * Time.deltaTime));
                dropShadow.SetActive(true);
                yield return new WaitForEndOfFrame();
            }
            
        }
        yield break;
    }

    bool lockRelease;
    private IEnumerator Release()
    {
        while (true)
        {
            if (!Grabbed)
                break;

            yield return new WaitForSeconds(9f);
            break;
        }
        Grabbed = false;
        lockRelease = false;
    }

    
    private IEnumerator executeAfterRevive(bool skipMoveTo)
    {
        canMove = false;
        if (!skipMoveTo)
        {
            while (Vector3.Distance(transform.position, respawnPoint) > 3.5f)
            {
                canMove = false;
                rb.isKinematic = true;
                //Debug.Log("RUN: " + Vector3.Distance(transform.position, respawnPoint));
                respawnPoint.y = 0;
                rb.MovePosition(Vector3.MoveTowards(transform.position, respawnPoint, 80 * Time.deltaTime));
                yield return new WaitForEndOfFrame();
            }

            canMove = false;

            String tag = "BeenRevived";
            if (corpse != null)
                corpse.tag = tag;



            yield return new WaitForSeconds(1.2f);
        }
        else
        {
            rb.MovePosition(respawnPoint);
        }
        animController.Dead = false;
        animController.Revive = true;
        Debug.Log("REVIVING");
        canMove = false;
        if (true)
            yield return new WaitForSeconds(1.0f);
        canMove = false;
        if (corpse != null)
            Destroy(corpse);
        if (true)
            yield return new WaitForSeconds(1.5f);
        canMove = false;
        animController.Revive = false;
        
        rb.velocity = Vector3.zero;
        RevivePlayer();
        alreadyDead = false;
        
        canMove = true;
        canExecute = true;
        canCollect = true;
        health = maxHealth;
        rb.isKinematic = false;
        scoreboard.Revives++;
        GameManager.Instance.ResetPlayer(this);
        reving = false;
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
        scoreboard = new Scoreboard();
        playerID = gameObject.GetInstanceID();
        gameObject.layer = 3;
    }
    bool dontForward;
    bool lookingRight = false;
    bool lookingLeft = false;
    private void FixedUpdate()
    {
        if (!playerLock && !gameManager.waitforvideo && !gameManager.isLoading)
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

            if (spriteRenderer == null)
            {
                spriteRenderer = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
            }

            Move();
        }
        else
        {
            animController.Moving = false;
        }

        if (!offSprite && spriteRenderer != null && !spriteRenderer.enabled)
        {
            offSprite = true;
            StartCoroutine(SpriteOn());
        }

    }
    SpriteRenderer sr;
    bool offSprite;

    public Vector3 GetMovementVector()
    {
        float zInput = controller.ForwardMagnitude;
        float xInput = controller.HorizontalMagnitude;
        Vector3 mV= new Vector3(xInput, 0, zInput);
        return mV;
    }

    public IEnumerator SpriteOn()
    {
        yield return new WaitForSeconds(9);
        spriteRenderer.enabled = true;
        offSprite = false;
    }

    public Vector3 movementVector;
    public float forceMod = 1000;
    Vector3 noY = new Vector3(1,0,1);
    private bool executeLock = false;
    Vector3 toEnemy;
    Vector3 offSet;
    private void Move()
    {
        if (!playerLock)
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

                if ((enemyAIController != null && ((enemyAIController.tag.Equals("Heavy") && (Vector3.Distance(transform.position, executeTarget.transform.position + forExecutePosition) < 5f))
                    || (Vector3.Distance(transform.position, executeTarget.transform.position + forExecutePosition) < 1f)) 
                    || ((executingCrab == true) && Vector3.Distance(transform.position, executeTarget.transform.position + offSet) < 5.0f)) 
                    && !lockExecAnim)
                {
                    if (executingCrab)
                    {

                        if (CharacterType.ID == 0) // badger
                        {
                            crabController.animator.SetBool("BadgerExecute", true);
                            StartCoroutine(PlayAnimAnyways());
                        }
                        if (CharacterType.ID == 1) // bunny
                        {
                            crabController.animator.SetBool("BunnyExecute", true);
                            StartCoroutine(PlayAnimAnyways());
                        }
                        if (CharacterType.ID == 2) // duck
                        {
                            crabController.animator.SetBool("DuckExecute", true);
                            StartCoroutine(PlayAnimAnyways());
                        }
                        if (CharacterType.ID == 3) // frog
                        {
                            crabController.animator.SetBool("FrogExecute", true);
                            StartCoroutine(PlayAnimAnyways());
                        }
                    }

                    animController.Moving = false;
                    hasReachedExecutePoint = true;

                    if (enemyAIController != null)
                    {
                        animController.Finisher(enemyAIController.tag, enemyAIController.colour, true);
                    }

                    lockExecAnim = true;
                    if (executeLock == false)
                    {
                        executeLock = true;
                        StartCoroutine(TurnOffExecuteMovement());
                    }
                }
                else if (Vector3.Distance(transform.position, executeTarget.transform.position + forExecutePosition) > 0.85f)
                {
                    animController.Moving = true;
                    if (executingCrab)
                    {
                        if (CharacterType.ID == 0) // badger
                        {
                            offSet = new Vector3(-20, 0, -4);
                            transform.parent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
                        }
                        if (CharacterType.ID == 1) // bunny
                        {
                            offSet = new Vector3(20, 0, -5);
                        }
                        if (CharacterType.ID == 2) // duck
                        {
                            offSet = new Vector3(5, 0, -4);
                        }
                        if (CharacterType.ID == 3) // frog
                        {
                            offSet = new Vector3(12, 0, 0);
                        }

                        toEnemy = executeTarget.transform.position + offSet;
                    }
                    else
                    {
                        toEnemy = executeTarget.transform.position + forExecutePosition;
                    }
                    rb.velocity = Vector3.zero;
                    Vector3 test = Vector3.MoveTowards(gameObject.transform.position, toEnemy, executeMoveSpeed * Time.fixedDeltaTime);
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
            if (dashing && !Grabbed)
            {
                float dz = dashDirection.z;
                //canTakeDamage = false;
                dashDirection.z *= 1.1f;
                rb.velocity = (new Vector3(0, rb.velocity.y) + ((dashDirection * dashSpeed * forceMod * 0.9f)) * Time.fixedDeltaTime);
                dashDirection.z = dz;
            }
            //if (!hitEnemy && (lockHitForward))
            //{

            //    //Debug.Log("FORWARD");
            //    float y = rb.velocity.y;
            //    rb.velocity = (GetMovementVector() * attackKnockback);
            //    rb.velocity = new Vector3(rb.velocity.x, y, rb.velocity.z);
            //}
            //else if (hitEnemy && lockHitBackward)
            //{
            //    //Debug.Log("BACK");
            //    float y = rb.velocity.y;
            //    rb.velocity = (-GetMovementVector() * attackKnockback);
            //    rb.velocity = new Vector3(rb.velocity.x, y, rb.velocity.z);
            //}
            if (canTakeDamage && gotHit)
            {
                float y = rb.velocity.y;
                rb.velocity = (otherPlayerMV * takenDamageKnockback);
                rb.velocity = new Vector3(rb.velocity.x, y, rb.velocity.z);
                StartCoroutine(GotHitReset());
            }
        }
    }


    private IEnumerator PlayAnimAnyways()
    {
        yield return new WaitForSeconds(0.1f);
        transform.parent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);

        if (CharacterType.ID == 0) // badger
        {
            // move to 
            //offSet = new Vector3(-20, 0, -4);
            //toEnemy = executeTarget.transform.position + offSet;
            //Vector3 test = Vector3.MoveTowards(gameObject.transform.position, toEnemy, executeMoveSpeed * Time.fixedDeltaTime);
            //test.y = 0;
            //transform.position = test;

            yield return new WaitForSeconds(1.3f);
            canTakeDamage = true;
            canMove = true;
            canAttack = true;
            canExecute = true;
            canMovePlayerForexecute = false;
            lockDash = false;
            executingCrab = false;

            //transform.parent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
            transform.parent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
            executeTarget.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(false);
            crabController.hasBeenExecuted = true;
            StartCoroutine(crabController.respawnCrab());
        }
        if (CharacterType.ID == 1) // bunny
        {
            yield return new WaitForSeconds(1.4f);
            canTakeDamage = true;
            canMove = true;
            canAttack = true;
            canExecute = true;
            canMovePlayerForexecute = false;
            lockDash = false;
            executingCrab = false;

            //transform.parent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
            transform.parent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
            executeTarget.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(false);
            crabController.hasBeenExecuted = true;
            StartCoroutine(crabController.respawnCrab());
        }
        if (CharacterType.ID == 2) // duck
        {
            yield return new WaitForSeconds(0.9f);
            canTakeDamage = true;
            canMove = true;
            canAttack = true;
            canExecute = true;
            canMovePlayerForexecute = false;
            lockDash = false;
            executingCrab = false;

            //transform.parent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
            transform.parent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
            executeTarget.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(false);
            crabController.hasBeenExecuted = true;
            StartCoroutine(crabController.respawnCrab());
        }
        if (CharacterType.ID == 3) // frog
        {
            yield return new WaitForSeconds(1.4f);

            canTakeDamage = true;
            canMove = true;
            canAttack = true;
            canExecute = true;
            canMovePlayerForexecute = false;
            lockDash = false;
            executingCrab = false;

            //transform.parent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
            transform.parent.GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
            executeTarget.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(false);
            crabController.hasBeenExecuted = true;
            StartCoroutine(crabController.respawnCrab());
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
        lockExecAnim = false;
        if (animController != null && enemyAIController != null)
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
        
        if (canTakeDamage && !alreadyDead)
        {
            StartCoroutine(DamageColourFlash());
            PAM.PlayAudio(PlayerAudioManager.AudioType.PlayerHit);
            if (!animController.Dead)
                animController.SetAll();
            //animController.HitReact = true;
            health = Mathf.Max(0, health - amount); // allows taking health from the player
        }
    }
    public void AddHealth(float amount) 
    {
        healAnimator.SetTrigger("PlayHeal");
        PAM.PlayAudio(PlayerAudioManager.AudioType.PotionUse);
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
        health = CharacterType.health;
        maxHealth = health;
        swings = CharacterType.swingCount;
        swingRecoverTime = CharacterType.swingRecoverTime;
        spriteRenderer = CharacterFolder.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
    }
    float x = 0;
    float attackTimer = 0;
    float attackTime = 0.1f;
    public bool lockHitForward;
    bool lockHitBackward;
    bool attacking;
    public bool attackImpLock = false;
    public float timer_ifswung;
    int time_resetSwung = 2;
    private void swingTimer()
    {
        if (timesSwung > 0 && timer_ifswung < time_resetSwung)
        {
            timer_ifswung += Time.deltaTime;

        }
        else if (timer_ifswung >= time_resetSwung)
        {
            timesSwung = 0;
            timer_ifswung = 0;
        }
        else if (timesSwung == 0)
        {
            timer_ifswung = 0;
        }
    }
    private void Attack()
    {
        swingTimer();

        if (controller.PrimaryAttackDown && canAttack && !dashing && timesSwung <= swings-1)
        {
            scoreboard.TimesSwung++;
            timesSwung++;
            canMove = false;
            canAttack = false;
            attacking = true;
            rb.velocity = Vector3.zero;
            animController.Attacking = true;

            StartCoroutine(attackDelay());
            StartCoroutine(attackCooldown());
            StartCoroutine(attackMoveCooldown());
        }
        else if (!attackImpLock && timesSwung > swings-1)
        {
            timesSwung = 0;
            attackImpLock = true;
            StartCoroutine(AttackImproved());
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
            //StartCoroutine(forwardHit());
        }
        if (hitEnemy && !lockHitBackward)
        {
            if (eac_cur != null && eac_cur.Health <= 0 && eac_cur.transform.parent.gameObject.activeSelf)
                PAM.PlayAudio(PlayerAudioManager.AudioType.EnemyHit);
            //StartCoroutine(backwardHit());
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
    public PlayerBody pbOther;
    public IEnumerator gotHitKnockback(Vector3 movementVect)
    {
        otherPlayerMV = Vector3.zero;
        otherPlayerMV = movementVect;
        if (otherPlayerMV.magnitude < 0.2f && otherPlayerMV.magnitude > -0.2f)
        {
            otherPlayerMV += new Vector3(0.5f, 0, 0.5f);
            otherPlayerMV *= 1;
        }
        gotHit = true;
        dontForward = false;
        yield return new WaitForSeconds(KnockbackTime);
        dontForward = true;
        gotHit = false;
        lockHitForward = false;
        otherPlayerMV = Vector3.zero;
        rb.velocity = Vector3.zero;
        //Debug.Log("COT");
    }

    private IEnumerator attackCooldown()
    {
        yield return new WaitForSeconds(attackSpeed * 0.95f);
        attacking = false;
        yield return new WaitForSeconds(attackSpeed * 0.05f);
        if (!waitingforImproved)
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

    public bool lockDash;
    public bool dashing;
    Vector3 dashDirection;
    private void Dash()
    {
        if (((controller.DashDown && dashOnCD == false) || (!controller.DashDown && dashQueue && !dashing && !dashOnCD)) && !attacking && !lockDash && sprite != null)
        {
            scoreboard.Dashes++;
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

    bool frogExecuted = false;
    bool executingCrab = false;
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
                if (LevelManager.Instance != null)
                    LevelManager.Instance.EnemyKilled();
                yield return new WaitForSeconds(0.8f);
                enemyAIController.DropEyes();
                scoreboard.ExecutesDone++;
                yield return new WaitForSeconds(0.75f);
                
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
                executeTarget = toExecute.transform.parent.GetChild(0).gameObject;
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
                    lockDash = true;

                    executingCrab = true;
                    canMovePlayerForexecute = true;

                    crabController.speed = 0;
                }
            }
        }
    }

    private void onNoDamage()
    {
        if (controller.NoDamageDown && enemyAIController != null)
        {
            //enemyAIController.canMove = !enemyAIController.canMove; //NEWW
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Revive" && gameObject.tag == "Ghost")
        {
            StartCoroutine(executeAfterRevive(true));
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
        yield return new WaitForSeconds(1.2f);
        GameObject c = Instantiate(deathBody, spriteRenderer.gameObject.transform.position, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(c, persistScene);
        corpse = c;
        Transform e = CharacterFolder.transform.GetChild(0).GetChild(0).transform;
        if (e.localScale.x < 0)
        {
            c.transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = true;
        }
        if (e.localScale.x > 0)
        {
            c.transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = false;
        }
        
        respawnPoint = c.transform.position;
    }

    public void GhostPlayer()
    {
        canAttack = false;
        canTakeDamage = false;
        canExecute = false;
        canUseItem = false;
        lockDash = true;
        gameObject.tag = "Ghost";
        gameObject.layer = 7;
        canCollect = true;
        //resetCanMoveOnRevive();
    }

    public void RevivePlayer()
    {
        canMove = true;
        canAttack = true;
        canTakeDamage = true;
        canExecute = true;
        canUseItem = true;
        lockDash = false;
        RESETINGGHOST = 0;
        gameObject.tag = "Player";
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
                    if (walrusAnimator != null)
                    {
                        walrusAnimator.SetTrigger("EatEye");
                    }
                    canCollectBomb = false;
                    gameManager.eyeCount -= 30;
                    hasPotion = false;
                    hasBomb = true;
                    collectable.gameObject.SetActive(false);
                    PAM.PlayAudio(PlayerAudioManager.AudioType.Buy);
                    StartCoroutine(resetCollectable(collectable.gameObject));
                }
            }
        }
        if (canCollectPotion)
        {
            if (controller.InteractDown)
            {
                if (gameManager.eyeCount >= 30 && hasPotion == false && hasBomb == false)
                {
                    if (walrusAnimator != null)
                    {
                        walrusAnimator.SetTrigger("EatEye");
                    }
                    canCollectPotion = false;
                    gameManager.eyeCount -= 30;
                    hasBomb = false;
                    hasPotion = true;
                    collectable.gameObject.SetActive(false);
                    PAM.PlayAudio(PlayerAudioManager.AudioType.Buy);
                    StartCoroutine(resetCollectable(collectable.gameObject));
                }
            }
        }
    }

    private IEnumerator resetCollectable(GameObject collectable)
    {
        yield return new WaitForSeconds(1.0f);
        collectable.SetActive(true);
    }

    private void UseItem()
    {

        Vector3 stinky = new Vector3 (7.5f, 5, 0); // so it spawns above the ground and infront
        Vector3 negativeStinky = new Vector3 (-7.5f, 5, 0);
        Vector3 fortniteFellaBalls = Vector3.zero; // placeholder

        if (controller.ItemDown && !alreadyDead)
        {
            if (hasBomb && !Grabbed)
            {
                scoreboard.BombsThrown++;
                hasBomb = false;
                hasPotion = false;

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
                PAM.PlayAudio(PlayerAudioManager.AudioType.Swing1);
            }
            if (hasPotion & !Grabbed)
            {
                scoreboard.PotionsUsed++;
                hasPotion = false;
                hasBomb = false;

                //sparkleObject.GetComponent<Animator>().SetTrigger("startSparkle");
                float healAmount = maxHealth * 0.75f;
                AddHealth(healAmount);
                hasPotion = false;
            }
        }
    }

    public void hideSparkleObject() //Runs after the health pot sparkle as an anim event to disable the sparkle effect
    {
        //sparkleObject.SetActive(false);
    }
    public void StartSpam()
    {
        canMove = false;
        canAttack = false;
        
        if (controller.InteractDown)
        {
            scoreboard.Struggled++;
            timesHit++;
        }

        if (timesHit >= 8)
        {
            canRelease = true;
            canMove = true;
            canAttack = true;
            lockDash = false;
            canExecute = true;
            Grabbed = false;  
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
        spriteRenderer.enabled = !spriteRenderer.enabled;
    }
    float i = 0;
    public void MoveToEnemy(GameObject enemyBody)
    {
        while (i < 2)
        {
            i += Time.deltaTime;
            rb.velocity = Vector3.zero;
            rb.MovePosition(new Vector3(enemyBody.transform.position.x, enemyBody.transform.position.y - 15, enemyBody.transform.position.z));
        }
        i = 0;
    }

}
