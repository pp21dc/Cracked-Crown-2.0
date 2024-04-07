using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using UnityEngine.Rendering;
using Unity.VisualScripting;
using System.Collections.Generic;

[System.Serializable]
public abstract class AIProperties // the properties that are most commonly used by all states and are now accesible to those states
{
    public float speed = 3.0f; //speed of enemy
    
}





public class EnemyAIController : AdvancedFSM
{

    public float EnemyID;


    [SerializeField]
    public GameObject stunObj;
    [SerializeField]
    Material baseMat;
    [SerializeField]
    Material whiteMat;

    [SerializeField]
    public EnemyAnimationController EAC;
    [SerializeField]
    GameManager GM;
    [SerializeField]
    private bool debugDraw;//draws the debug text
    [SerializeField]
    private Text StateText;//shows the state the enmy is currently on
    [SerializeField]
    private Text HealthText;//shows enemy current health
    public string colour;
    //for finding the players
    [SerializeField]
    private GameObject[] Players = new GameObject[4]; //holds all players
    public GameObject closest;//holds the closest player
    private float currShortest = 100000f; //current shortest distance
    private Vector3 movementVector = Vector3.zero; // the vector that the enemy is moving towards

    //the enemy body
    [SerializeField]
    public Transform enemyPosition; //holds the enemy player position
    public Transform hitBy;
    [SerializeField]
    private GameObject enemy;
    [SerializeField]
    private Rigidbody enemyBody;
    [SerializeField]
    public CheckPlayerBelow checkPlayerBelow;
    public Rigidbody eBody { get { return enemyBody; } }

    public Transform ePosition { get { return enemyPosition; } }

    //speedChanges
    [SerializeField]
    private float speed = 5f; //speed of the enemy
    [SerializeField]
    private float HeavyDashSpeed = 2f;
    private float mediumSpeed;
    private float heavySpeed;
    private float lightSpeed;
    private float mediumRoamSpeed;

    public bool canMove = true;

    public CameraShake shakeSprite;

    //light enemy
    [SerializeField]
    private Transform fireLocation;
    [SerializeField]
    private GameObject Goop;
    

    //Medium needed variables
    public Vector3 TargetPlayerPos;
    public bool isHeavyDashing = true;
    public bool isDoneDashing = false;

    //Light needed vriables
    private float intialY;
    private bool canGoop;
    private bool startGoop;

    [SerializeField]
    private CheckPlayerBelow belowChecker;

    public bool startSlam;

    public bool canSlam;

    public bool moveToStunned;
    public bool moveToCarry;

    [SerializeField]
    private SlamAttack slamAttack;

    [SerializeField]
    private float slamSpeed = 15f;

    [SerializeField]
    private Transform SlamLocation;

    public bool doneCarry;
    public bool doneStun;

    private bool doneOnGround;

    public bool canStun;
    private bool canCarry;
    private bool startCarryingUp;
    private bool startCarrying;
    private bool canSpam;
    public bool canPickup;
    public bool canWait;

    public GameObject dropShadow;

   public Vector3 groundTrans;

    [SerializeField]
    private Transform upPlacement;

    private bool noTransform;

    [SerializeField]
    private Vector3 randTrans;

    public int maxAmmo = 10;
    public int heavyBullets = 10;
    public bool doneReloading;
    public bool doneShockwave;
    public bool startShooting;
    public bool startShock;
    public bool startReload;
    public bool canShoot;
    public bool noShockCooldown;
    public GameObject toothLeft;
    public GameObject toothRight;
    public GameObject shockwaveCol;
    public Vector3 targetShockwaveScale;
    public Vector3 shockwaveScaleInitial;
    public Transform bodyShootLoc;
    public GameObject ToothShotLocation;
    public GameObject Hole;
    public Transform HoleSpawnLoc;
    private GameObject correctTooth;
    

    //seperate vars
    public bool InContact;
    public SeperateCheck sepCheck;
    public bool canSeperate;
    public GameObject otherAI;
    private bool goLeft;
    private bool goRight;
    public Transform SepLoc;
    public bool doneSeperating;

    //wall seperation
    public bool wallContact;
    [SerializeField]
    public WallCheck wallCheck;

    private bool wallGo;
    public bool canWall;
    public bool doneWall;

    public GameObject shockWave;
    //cooldown vars
    public bool dashOnCD;
    public bool shockwaveOnCD;
    public bool shootOnCD;
    private bool roamOnCD;

    //roam vars
    private Vector3 roamLoc;
    private bool firstRoam;
    private bool isRoaming;
    

    //health, finisher, and death states
    public float maxHealth = 40; // its total Health
    [SerializeField]
    private float health = 40;//health of the enemy
    

    [SerializeField]
    private Collider Damage;


    //heavy enemy shoot arc
    private bool shootUp;
    private bool shootDown;
    
    public float Health
    {
        get { return health; }
    }
    public void DecHealth(float amount) { health = Mathf.Max(0, health - amount); StartCoroutine(FlashRed(EAC.SR)); }//allows us to decrease the health of an enemy
    public void AddHealth(float amount) { health = Mathf.Min(100, health + amount); }//allows us to add health to the enemy

    
    public string state = "NONE";


    //allows us to grab the state in which the enemy should be on
    private string GetStateString()
    {

        
        if (CurrentState.ID == FSMStateID.Dead)
        {
            state = "DEAD";
        }
        else if (CurrentState.ID == FSMStateID.FindPlayer)
        {
            state = "FINDPLAYER";
        }
        else if (CurrentState.ID == FSMStateID.Finished)
        {
            state = "FINISHED";
        }
        else if (CurrentState.ID == FSMStateID.SlamGround)
        {
            state = "SLAMGROUND";
        }
        else if (CurrentState.ID == FSMStateID.Carry)
        {
            state = "CARRY";
        }
        else if (CurrentState.ID == FSMStateID.Stunned)
        {
            state = "STUNNED";
        }
        else if (CurrentState.ID == FSMStateID.HeavyDash)
        {
            state = "HEAVYDASH";
        }
        else if (CurrentState.ID == FSMStateID.Gun)
        {
            state = "GUN";
        }
        else if (CurrentState.ID == FSMStateID.Shockwave)
        {
            state = "SHOCKWAVE";
        }
        else if (CurrentState.ID == FSMStateID.Reload)
        {
            state = "RELOAD";
        }
        else if (CurrentState.ID == FSMStateID.Seperate)
        {
            state = "SEPERATE";
        }
        else if (CurrentState.ID == FSMStateID.Wall)
        {
            state = "WALL";
        }
        else if (CurrentState.ID == FSMStateID.Roam)
        {
            state = "ROAM";
        }
        
        


        return state;
    }

    

    //intializes the enemy with the player location and sets enemy health to 100 theb calls Construct FSM
    protected override void Initialize()
    {

        EnemyID = gameObject.GetInstanceID();

        if (LevelManager.Instance != null)
        {
            if (CompareTag("Light"))
                EAC.SetAnimController(LevelManager.Instance.CURRENT_ROOM, 0);
            else if (CompareTag("Medium"))
                EAC.SetAnimController(LevelManager.Instance.CURRENT_ROOM, 1);
            else if (CompareTag("Heavy"))
                EAC.SetAnimController(LevelManager.Instance.CURRENT_ROOM, 2);
        }
        Players = GameObject.FindGameObjectsWithTag("Player");//finds and add all players to array
        if(Players.Length <= 0) { Players = null; }
        if (Players != null)
        {
            playerTransform = Players[0].transform.GetChild(1);
            closest = playerTransform.gameObject;
        }
        if (GameManager.Instance != null)
        {
            playerTransform = GameManager.Instance.Players[0].transform.GetChild(1);
            closest = playerTransform.gameObject;
        }
        Damage = gameObject.GetComponent<Collider>();
        Damage.enabled = false; // deactivates the damage collider
        isHeavyDashing = true;
        if (CompareTag("Medium"))
        {
            health = 55;
            maxHealth = 55;
        }
        else if (CompareTag("Light"))
        {
            health = 35;
            maxHealth = 35;
        }
        else if (CompareTag("Heavy"))
        {
            health = 80;
            maxHealth = 80;
            shockWave.SetActive(true);
        }

        if(gameObject.CompareTag("Light"))
        {
            intialY = enemyBody.transform.position.y;

        }

        startGoop = true;
        canGoop = true;

        
        startSlam = false;

        canSlam = true;

        slamSpeed = 65f;

        moveToStunned = false;
        moveToCarry = false;

        doneCarry = false;
        doneStun = false;

        doneOnGround = false;

        canStun = true;

        canCarry = true;

        startCarrying = false;
        startCarryingUp = false;
        canSpam = true;
        canPickup = true;
        canWait = true;

        noTransform = true;

        firstRoam = true;
        isRoaming = true;
        roamOnCD = false;

        shockwaveCol.SetActive(false);

        randTrans = new Vector3(0,0,0);

        GameObject ground = GameObject.FindGameObjectWithTag("Ground");
        GM = GameManager.Instance;
        groundTrans = ground.transform.position;

        doneReloading = false;
        doneShockwave = false;
        startReload = true;
        startShock = true;
        startShooting = true;
        canShoot = true;
        maxAmmo = 6;
        heavyBullets = 6;
        if (shockwaveCol != null)
        {
            targetShockwaveScale = new Vector3(35, shockwaveCol.transform.localScale.y, 35);
            shockwaveScaleInitial = shockwaveCol.transform.localScale;
        }
        noShockCooldown = true;
        
        firstRoam = true;

        //cooldowns
        dashOnCD = false;
        shootOnCD = false;
        shockwaveOnCD = false;

        //seperate vars
        InContact = false;
        goLeft = false;
        goRight = false;
        doneSeperating = false;
        canSeperate = true;

        //wall seperate vars
        wallContact = false;
        wallGo = false;
        canWall = false;
        doneWall = false;

        //enemy speeds
        lightSpeed = 50f;
        mediumSpeed = 35f;
        mediumRoamSpeed = 12f;
        HeavyDashSpeed = 95f;
        heavySpeed = 25f;

        //heavy enemy arcs
        shootUp = false;
        shootDown = false;

        roamLoc = new Vector3(enemyPosition.localPosition.x + (int)Random.Range(-130, 130), 0, enemyPosition.localPosition.z + (int)Random.Range(-130, 130));

        ConstructFSM();
    }
    public bool act = false;
    public bool starting;
    protected override void FSMUpdate()
    {
        GetStateString();
        if (LevelManager.Instance != null)
        {
            //act = !LevelManager.Instance.loc;
        }
        else
        {
            act = true;
        }
        if (tag == "Medium")
        {
            dropShadow.transform.localPosition = new Vector3(dropShadow.transform.localPosition.x, -0.25f, 0.52f);
        }
        else if (tag == "Light")
        {
            dropShadow.transform.position = new Vector3(dropShadow.transform.position.x, -0.5f, EAC.transform.position.z - 1.1f);
        }
        if (CurrentState != null && act)
        {
            if (!shaking && !CompareTag("Heavy"))
                EAC.transform.localPosition = new Vector3(0, 6f, 5.15f);
                

            if (CompareTag("Light"))
            {
                dropShadow.transform.localScale = new Vector3(8, 1.5f, 0.5f);
            }
            else
            {
                dropShadow.transform.localScale = new Vector3(13, 3, 0.5f);
            }

            CurrentState.Reason(playerTransform, transform);
            CurrentState.Act(playerTransform, transform);
            if (!canPickup)
            {
                StartCoroutine(ResetCanPickUp());
            }
        }
    }

    IEnumerator ResetCanPickUp()
    {
        yield return new WaitForSeconds(8);
        canPickup = true;
    }

    public void StartUp()
    {
        if (LevelManager.Instance != null)
        {
            if (CompareTag("Light"))
                EAC.SetAnimController(LevelManager.Instance.CURRENT_ROOM, 0);
            else if (CompareTag("Medium"))
                EAC.SetAnimController(LevelManager.Instance.CURRENT_ROOM, 1);
            else if (CompareTag("Heavy"))
                EAC.SetAnimController(LevelManager.Instance.CURRENT_ROOM, 2);
        }

        if (gameObject.CompareTag("Light"))
        {
            intialY = enemyBody.transform.position.y;
        }
    }

    //lets us add states and transitions in which we can use to move to other states when needed.
    private void ConstructFSM()
    {

        RoamState roamState = new RoamState(this);
        roamState.AddTransition(Transition.LowHealth, FSMStateID.Finished);
        roamState.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        roamState.AddTransition(Transition.LookForPlayer, FSMStateID.FindPlayer);
        roamState.AddTransition(Transition.enemiesInContact, FSMStateID.Seperate);
        roamState.AddTransition(Transition.hitDaWall, FSMStateID.Wall);


        //follows player, transitions out if it is above the player
        FindPlayerState findPlayerState = new FindPlayerState(this);
        findPlayerState.AddTransition(Transition.AbovePlayer, FSMStateID.SlamGround);
        findPlayerState.AddTransition(Transition.InFirstRange, FSMStateID.HeavyDash);
        findPlayerState.AddTransition(Transition.InShootingRange, FSMStateID.Gun);
        findPlayerState.AddTransition(Transition.InShockwaveRange, FSMStateID.Shockwave);
        findPlayerState.AddTransition(Transition.LowHealth, FSMStateID.Finished);
        findPlayerState.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        findPlayerState.AddTransition(Transition.PlayerDead, FSMStateID.FindPlayer);
        findPlayerState.AddTransition(Transition.enemiesInContact, FSMStateID.Seperate);
        findPlayerState.AddTransition(Transition.hitDaWall, FSMStateID.Wall);

        //if at low health it allows the enemy to be finished, tranistions if no health and not finished.
        FinishedState finishedState = new FinishedState(this);
        finishedState.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        finishedState.AddTransition(Transition.HealthBack, FSMStateID.FindPlayer);
        
        

        //ded
        DeadState deadState = new DeadState(this);
        

        //light enemy states down here

        //Slams the ground bellow it, transistions if it succeeds to carry, fails to stunned, low health to finished, no health to dead
        SlamGroundState slamGroundState = new SlamGroundState(this);
        slamGroundState.AddTransition(Transition.SlamSuceeded, FSMStateID.Carry);
        slamGroundState.AddTransition(Transition.SlamFailed, FSMStateID.Stunned);
        slamGroundState.AddTransition(Transition.LowHealth, FSMStateID.Finished);
        slamGroundState.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        //carries the player, transitions if it drops player to find
        CarryState carryState = new CarryState(this);
        carryState.AddTransition(Transition.LookForPlayer, FSMStateID.FindPlayer);
        

        //if it fails to slam its stunned, transitions if stun finishes to find, low health to finsished, no health to dead.
        StunnedState stunnedState = new StunnedState(this);
        stunnedState.AddTransition(Transition.LookForPlayer, FSMStateID.FindPlayer);
        stunnedState.AddTransition(Transition.LowHealth, FSMStateID.Finished);
        stunnedState.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        //Medium enemy states down here

        //Slams into player knocking them back, Transition if done to find player, low health to finished, no health to dead
        HeavyDashState heavyDashState = new HeavyDashState(this);
        heavyDashState.AddTransition(Transition.LookForPlayer, FSMStateID.FindPlayer);
        heavyDashState.AddTransition(Transition.LowHealth, FSMStateID.Finished);
        heavyDashState.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        

        //Heavy enemy states down here

        //Shoots at the player from a distance, Transition to reload if out of bullets, low health to finish, no health to dead
        GunState gunState = new GunState(this);
        gunState.AddTransition(Transition.NoBullets, FSMStateID.Reload);
        gunState.AddTransition(Transition.InShockwaveRange, FSMStateID.Shockwave);
        gunState.AddTransition(Transition.LowHealth, FSMStateID.Finished);
        gunState.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        

        //sends out a mini shockwave to knock players away from it, Tranisition if done to find player, low health if finish, no health to dead
        ShockwaveState shockwaveState = new ShockwaveState(this);
        shockwaveState.AddTransition(Transition.LookForPlayer, FSMStateID.FindPlayer);
        shockwaveState.AddTransition(Transition.LowHealth, FSMStateID.Finished);
        shockwaveState.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        shockwaveState.AddTransition(Transition.InShootingRange, FSMStateID.Gun);

        //Reloads if out of teeth, Tranisition if done to find player, low health if finished, no health to dead
        ReloadState reloadState = new ReloadState(this);
        reloadState.AddTransition(Transition.LookForPlayer, FSMStateID.FindPlayer);
        reloadState.AddTransition(Transition.LowHealth, FSMStateID.Finished);
        reloadState.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        reloadState.AddTransition(Transition.InShockwaveRange, FSMStateID.Shockwave);
        reloadState.AddTransition(Transition.InShootingRange, FSMStateID.Gun);

        SeperateState seperateState = new SeperateState(this);
        seperateState.AddTransition(Transition.LowHealth, FSMStateID.Finished);
        seperateState.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        seperateState.AddTransition(Transition.LookForPlayer, FSMStateID.FindPlayer);
        seperateState.AddTransition(Transition.hitDaWall, FSMStateID.Wall);
        
        WallSeperateState wallState = new WallSeperateState(this);
        wallState.AddTransition(Transition.LowHealth, FSMStateID.Finished);
        wallState.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        wallState.AddTransition(Transition.LookForPlayer, FSMStateID.FindPlayer);

        //Add all states to the state list

        AddFSMState(roamState);
        AddFSMState(findPlayerState);
        AddFSMState(finishedState);
        AddFSMState(deadState);
        AddFSMState(seperateState);
        AddFSMState(wallState);

        AddFSMState(slamGroundState);
        AddFSMState(carryState);
        AddFSMState(stunnedState);

        AddFSMState(heavyDashState);
        

        AddFSMState(gunState);
        AddFSMState(shockwaveState);
        AddFSMState(reloadState);
        
        
        
    }

    public void StartRoam()
    {
        
        if(!roamOnCD)
        {
            if (Random.Range(0, 1000) < 2)
            {
                roamLoc = new Vector3(enemyPosition.localPosition.x + (int)Random.Range(-130, 130), 0, enemyPosition.localPosition.z + (int)Random.Range(-130, 130));
                roamOnCD = true;
            }
        }

        EAC.Moving = true;

        movementVector = (roamLoc - enemyPosition.transform.position).normalized * mediumRoamSpeed;
        enemyPosition.transform.position += movementVector * Time.deltaTime;//moves to player

        if(Vector3.Distance(enemyPosition.localPosition, roamLoc) <= 5f)
        {
            roamLoc = new Vector3(enemyPosition.localPosition.x + (int)Random.Range(-130, 130), 0, enemyPosition.localPosition.z + (int)Random.Range(-130, 130));
        }

    }

    


    //finds the closest player and sets the target position
    public void checkShortestDistance()
    {
        if (GM.Players != null)
        {
            float check;
            currShortest = 10000;
            float closestInt = 0;
            //closest = null;
            //simple distance check where it checks the current shortest and compares to the other players, replacing when neccisary
            for (int i = 0; i < GM.Players.Length; i++)
            {

                PlayerBody currentBody = GM.Players[i].PB;

                if (!currentBody.alreadyDead && !currentBody.Grabbed)
                {
                    check = Vector3.Distance(enemyPosition.transform.position, currentBody.transform.position);
                    //Debug.Log(currentBody.alreadyDead + "//" + currentBody.Grabbed);

                    if (check < currShortest)
                    {

                        currShortest = check;
                        closestInt += 1; 
                        closest = GM.Players[i].PB.gameObject;
                        playerTransform = closest.transform;

                    }

                }
            }
            if (closestInt == 0)
            {
                closest = null;
            }
            if (closest == null && !GM.Players[0].PB.alreadyDead && !GM.Players[0].PB.Grabbed)
            {
                closest = GM.Players[0].PB.gameObject;
                playerTransform = GM.Players[0].PB.transform;
            }
            if (true)//is there a player and we aren't being knocked back
            {
                if(gameObject.CompareTag("Light"))
                {
                    setAndMoveToTargetLight(lightSpeed);
                }
                else if(gameObject.CompareTag("Medium"))
                {
                    setAndMoveToTarget(mediumSpeed * 0.8f);
                }
                else if(gameObject.CompareTag("Heavy"))
                {
                    setAndMoveToTargetHeavy(heavySpeed);
                }
            }
                
        }
        else
        {
            Debug.Log("CANT FIND PLAYERS");
        }
    }

    //sets enemy target position and moves towards it
    private void setAndMoveToTarget(float Speed)
    {
        if (Speed > 0.5f)
        {
            EAC.Moving = true;
        }
        else
        {
            EAC.Moving = false;
        }
        if (closest != null)
        {
            if (!lockKnock)
            {
                movementVector = (closest.transform.position - enemyPosition.transform.position).normalized * Speed;
                if (tag != "Light")
                    movementVector.y = 0;
                enemyPosition.transform.position += movementVector * Time.deltaTime;//moves to player
            }
            //enemyBody.transform.position = new Vector3(enemyBody.position.x, 0, enemyBody.position.z); //keeps it on ground
            if (closest.transform.position.x + 1 > enemyPosition.transform.position.x)
            {
                EAC.SR.flipX = false;
            }
            else
            {
                EAC.SR.flipX = true;
            }
        }
        else
        {
            PlayerBody tmp = GM.Players[Random.Range(0, GM.Players.Length)].PB;
            if (!tmp.alreadyDead && !tmp.Grabbed)
            {
                closest = tmp.gameObject;
            }
        }

        

    }

    private void setAndMoveToTargetLight(float Speed)
    {

        Debug.Log(closest);
        if (closest != null)
        {
            EAC.Moving = true;
            Vector3 currPlayerPos = new Vector3(closest.transform.position.x, groundTrans.y + 30, closest.transform.position.z);
            movementVector = (currPlayerPos - enemyPosition.transform.position).normalized * Speed;
            enemyPosition.transform.position += movementVector * Time.deltaTime;//moves to player
            if (closest.transform.position.x + 1 > enemyPosition.transform.position.x)
            {
                //EAC.SR.flipX = false;
            }
            else
            {
                //EAC.SR.flipX = true;
            }
        }
        else
        {
            PlayerBody tmp = GM.Players[Random.Range(0, GM.Players.Length)].PB;
            if (!tmp.alreadyDead && !tmp.Grabbed)
            {
                closest = tmp.gameObject;
            }
        }

        //if it can goop, it shoot projectiles
       /* if (startGoop)
        {
            startGoop = false;
            StartCoroutine(GoopRoutine());
        }*/
        
        //if player is below it, start the slam state
        if(belowChecker.IsPlayerBelow())
        {
            belowChecker.enabled = false; //will set back to true during fail or suceed states
            canGoop = false;
            startSlam = true;
        }

    }

    private void setAndMoveToTargetHeavy(float Speed)
    {
        if (Speed > 0.5f)
        {
            EAC.Moving = true;
        }
        else
        {
            EAC.Moving = false;
        }
        Debug.Log(closest == null);
        if (closest != null)
        {
            if(Vector3.Distance(this.ePosition.position, closest.transform.position) <= 55f)
            {
                if (!lockKnock)
                {
                    movementVector = (closest.transform.position - enemyPosition.transform.position).normalized * Speed;
                    if (tag != "Light")
                        movementVector.y = 0;
                    enemyPosition.transform.position -= movementVector * Time.deltaTime;//moves to player
                }
                //enemyBody.transform.position = new Vector3(enemyBody.position.x, 0, enemyBody.position.z); //keeps it on ground
                if (closest.transform.position.x + 1 > enemyPosition.transform.position.x)
                {
                    EAC.SR.flipX = true;
                }
                else
                {
                    EAC.SR.flipX = false;
                }
            }
            else if (Vector3.Distance(this.ePosition.position, closest.transform.position) >= 75f)
            {
                if (!lockKnock)
                {
                    movementVector = (closest.transform.position - enemyPosition.transform.position).normalized * Speed;
                    if (tag != "Light")
                        movementVector.y = 0;
                    enemyPosition.transform.position += movementVector * Time.deltaTime;//moves to player
                }
                //enemyBody.transform.position = new Vector3(enemyBody.position.x, 0, enemyBody.position.z); //keeps it on ground
                if (closest.transform.position.x + 1 > enemyPosition.transform.position.x)
                {
                    EAC.SR.flipX = false;
                }
                else
                {
                    EAC.SR.flipX = true;
                }
            }
            
        }
        else
        {
            PlayerBody tmp = GM.Players[Random.Range(0, GM.Players.Length)].PB;
            if (!tmp.alreadyDead && !tmp.Grabbed)
            {
                closest = tmp.gameObject;
            }
        }



    }




    //shooting code from franks class
    IEnumerator GoopRoutine()
    {

        while(canGoop)
        {
            StartShootGoop(enemyPosition, fireLocation);
            yield return new WaitForSeconds(0.45f);
        }

        //yield return null;
    }

    //shooting code from franks class
    private void StartShootGoop(Transform body, Transform fireLocation)
    {
        if(Goop)
        {
            Vector3 direction = fireLocation.position - body.position;
            direction.x = 0;
            direction.z = 0;
            direction.Normalize();

            GameObject GoopGO = GameObject.Instantiate(Goop, fireLocation.position, Quaternion.identity);
            Goop goop = GoopGO.GetComponent<Goop>();
            GoopGO.SetActive(true);
            goop.Fire(direction);
        }
    }



    public bool inFinish;
    //starts the finished coroutine
    public void StartFinish()
    {
        if (!inFinish)
        {
            inFinish = true;
            StartCoroutine(Finished());
        }

        if (doneOnGround == true)
        {
            EAC.Moving = true;//NEW
            if (doneStun == false)
            {
                Debug.Log("LetsGoUp");
                EAC.Moving = true;//NEW
                EAC.Stunned = false;
                Vector3 upVector = new Vector3(enemyPosition.position.x, 30f, enemyPosition.position.z);
                movementVector = (upVector - enemyPosition.transform.position).normalized * speed;
                if (tag == "Medium" || tag == "Heavy")
                    movementVector.y = 0;
                enemyPosition.transform.position += movementVector * Time.deltaTime;//moves to player
            }
        }
    }


    //starts a death coroutine
    public void StartDeath()
    {

        if (!dead)
        {
            dead = true;
            StartCoroutine(Death());
            
        }
    }

    //stops the enemy starts the animation and heals the enemy overtime until it can move again
    IEnumerator Finished()
    {

        //stunned animation here
        EAC.Stunned = true;
        

        while (health < maxHealth*(0.75))
        {
            health += Time.deltaTime * 2;
            //Debug.Log("HEALTH");
            yield return null;
        }

        EAC.Stunned = false;
        inFinish = false;

        if(gameObject.CompareTag("Light"))
        {
            doneOnGround = true;

            yield return new WaitForSeconds(2f);

            EAC.Stunned = false;
            doneStun = true;
        }

    }
    bool dead;

    //destroys the enemy game object
    IEnumerator Death()
    {


        //animation here
        EAC.Dead = true;
        //scale time to animation

        yield return new WaitForSeconds(0.01f);

        Debug.Log("DEATH");
        //DropEyes();

        DropEyes();
        yield return new WaitForSeconds(0.69f);
        if (gameObject.CompareTag("Light") || gameObject.CompareTag("Medium") || gameObject.CompareTag("Heavy"))
        {
            LevelManager.Instance.EnemyKilled();

            Destroy(transform.parent.gameObject);
        }
        else
        {
            GameObject holeGO = GameObject.Instantiate(Hole, HoleSpawnLoc);
        }

        //yield return null;
    }

    [SerializeField]
    private GameObject eyes;

   // chooses a random number in a range for the enemy and drops that many eyes
    public void DropEyes()
    {
        int dropRate = 0;

        if (gameObject.CompareTag("Light"))
        {
            dropRate = 2;
        }
        else if (gameObject.CompareTag("Medium"))
        {
            dropRate = 3;
        }
        else if (gameObject.CompareTag("Heavy"))
        {
            dropRate = 7;
        }

        for (int i = 0; i < dropRate; i++)
        {
            Instantiate(eyes, enemyBody.transform.position + new Vector3(Random.Range(-10, 10), transform.position.y + 5, Random.Range(-10, 10)), Quaternion.identity);
        }
    }
    
    public bool lockKnock = false;
    public IEnumerator KB(Vector3 dir)
    {
        lockKnock = true;
        if (enemyBody != null)
        {
            enemyBody.isKinematic = false;
            enemyBody.AddForce(dir * 0.45f);
            yield return new WaitForSeconds(0.3f);

            enemyBody.velocity = Vector3.zero;
            enemyBody.isKinematic = true;
        }
        lockKnock = false;
    }


    //starts the slam method
    public void StartSlam()
    {

        EAC.Attacking = true;
        if (slamAttack.hasHit == true)
        {
            //EAC.Attacking = false;
            moveToCarry = true;
            
            Debug.Log("Carry");

        }
        else if (slamAttack.HitGround == true && !slamAttack.hasHit)
        {
            EAC.Attacking = false;
            EAC.Stunned = true;
            moveToStunned = true;
            Debug.Log("Stunned");
             
        }
        else if (slamAttack.hasHit == false && slamAttack.HitGround == false)
        {
            EAC.Attacking = true;
            Vector3 hit = new Vector3(enemyPosition.position.x, 0, enemyPosition.position.z);
            movementVector = (hit - enemyPosition.transform.position).normalized * slamSpeed;
            if (tag == "Medium" || tag == "Heavy")
                movementVector.y = 0;
            enemyPosition.transform.position += movementVector * Time.deltaTime;//moves to player
        }
        else
        {
            EAC.Attacking = false;
        }
    }



    //checks slam code for if it has ti the player or the ground

    //resets all of the slam variables
    public bool carrying;
    public void ResetSlamVar()
    {
        slamAttack.hasHit = false;
        slamAttack.HitGround = false;
        EAC.Attacking = false;
        moveToCarry = false;
        moveToStunned = false;
       
    }

    //if light enemy is stunned, runs the following code
    public void StartStunned()
    {
        if (canStun)
        {
            canStun = false;
            doneOnGround = false;
            StartCoroutine(Stunned());
        }

        if (doneOnGround)
        {
            slamAttack.HitGround = false;  
            Debug.Log("LetsGoUp");
            EAC.Moving = true;
            EAC.Stunned = false;
            Vector3 upVector = new Vector3(enemyPosition.position.x, 30f, enemyPosition.position.z);
            if (tag == "Medium" || tag == "Heavy")
                movementVector.y = 0;
            movementVector = (upVector - enemyPosition.transform.position).normalized * speed;
            enemyPosition.transform.position += movementVector * Time.deltaTime;//moves to player
        }
    }

    //keeps enemy on the ground and then moves it up through the method once 3 seconds are up
    IEnumerator Stunned()
    {
        EAC.Stunned = true;
        doneStun = false;
        yield return new WaitForSeconds(3f);

        EAC.Moving = true;
        doneOnGround = true;
        EAC.Stunned = false;

        yield return new WaitForSeconds(2f);

        doneStun = true;
        
        belowChecker.enabled = true;
        doneStun = true;
        canPickup = true;
        


        //yield return null; //CAN THIS EXIT?
    }

    void SetGrabAnim(PlayerBody body, bool active)
    {
        Debug.Log("ANIM: " + active);
        if (body.CharacterType.ID == 0)
            EAC.Badger_Grabbed = active;
        else if (body.CharacterType.ID == 1)
            EAC.Bunny_Grabbed = active;
        else if (body.CharacterType.ID == 2)
            EAC.Duck_Grabbed = active;
        else if (body.CharacterType.ID == 3)
            EAC.Frog_Grabbed = active;
        EAC.Moving = false;
        EAC.Attacking = false;
        body.Grabbed = active;
        //StartCoroutine(ResetPlayerGrab(body));
    }

    public IEnumerator ResetPlayerGrab(PlayerBody pb)
    {
        yield return new WaitForSeconds(5f);
        pb.Grabbed = false;
    }

    public void StopDrops()
    {
        foreach (Coroutine cout in couts)
        {
            StopCoroutine(cout);
        }
    }

    int prevHit = 0;
    bool shaking;
    //method for carrying the player
    public void StartCarry()
    {
        if (slamAttack.hitPlayer == null || doneCarry)
            return;
        PlayerBody body = slamAttack.hitPlayer;
        body.StartSpam();
        
        carrying = true;
        body.MoveToEnemy(enemyPosition.gameObject);
        if (body.timesHit >= 7)
        {
            prevHit = 0;
            canSpam = false;

            //couts.Add(StartCoroutine(Drop(body)));
            body.timesHit = 0;
            body.canRelease = true;
        }
        else if (body.timesHit > prevHit)
        {
            shaking = true;
            prevHit = body.timesHit;
            StartCoroutine(shakeSprite.Shake(0.35f, 1.8f));
        }

        if(body.Health <= 0)
        {
            canSpam = false;

            //couts.Add(StartCoroutine(Drop(body)));
            body.timesHit = 0;
            body.canRelease = true;
        }


        if (body.canRelease && canSpam == false)
        {
            Debug.Log("RELEASE DROP");
            canSpam = true;
            GM.ResetPlayer(body);
            StartCoroutine(Drop(body));
            SetGrabAnim(body, false);
            EAC.Grabbing = false;
            EAC.Attacking = false;
        }
        else 
        {

            SetGrabAnim(body, true);
        }
        int temp = Random.Range(1, 4);
        int xDir = 1;
        int zDir = 1;

        if(temp == 1)
        {
            xDir = 1; zDir = 1;
        }
        else if (temp == 2)
        {
            xDir= 1; zDir = -1;
        }
        else if (temp == 3)
        {
            xDir = -1; zDir= 1;
        }
        else if(temp == 4)
        {
            xDir = -1; zDir = -1;
        }

        if (canCarry == true)
        {
            canCarry = false;
            body.Grabbed = true;
            StartCoroutine(Carry(body));
            StartCoroutine(DOT());
        }

        if(startCarryingUp == true)
        {

            SetGrabAnim(body, true);
            Debug.Log("Lets go up");
            Vector3 upVector = new Vector3(enemyPosition.position.x, 30f, enemyPosition.position.z);
            movementVector = (upVector - enemyPosition.transform.position).normalized * speed;
            if (tag == "Medium" || tag == "Heavy")
                movementVector.y = 0;
            enemyPosition.transform.position += movementVector * Time.deltaTime;//moves to player

            
        }

        if (startCarrying == true)
        {
            if (noTransform)
            {
                noTransform = false;
                randTrans = new Vector3(xDir * (randTrans.x + (int)Random.Range(1, 200)), groundTrans.y + 30, zDir * (randTrans.z + (int)Random.Range(1, 200)));
            }

            //Debug.Log(randTrans);
            movementVector = (randTrans - enemyPosition.transform.position).normalized * speed;
            enemyPosition.transform.position += movementVector * Time.deltaTime;//moves to player
        }
        //call player carry method, move the light enemy to a random point, if timer runs out, drop player go to find player, use rand on an x and z for a random direction
        
        

    }
    List<Coroutine> couts = new List<Coroutine>();
    IEnumerator Carry(PlayerBody pb)
    {
        //call a method on the player that sets the sprite active to false and sets movement to false
        pb.ResetSprite("FROM CARRY CORUT");
        pb.spriteRenderer.enabled = false;
        SetGrabAnim(pb, true);
         
        startCarryingUp = true;
        EAC.Attacking = false;
        EAC.Grabbing = false;
        yield return new WaitForSeconds(0.5f);
        EAC.Grabbing = true;

        yield return new WaitForSeconds(2.5f);

        
        startCarryingUp = false;

        startCarrying = true;

        yield return new WaitForSeconds(4f);

        if (slamAttack.hitPlayer != null)
        {
            Debug.Log("TIMED DROP");
            StartCoroutine(Drop(pb));
            EAC.Attacking = false;
            EAC.Grabbing = false;
            pb.Grabbed = false;
        }

        yield return null;
    } 
    
    IEnumerator DOT()
    {
        PlayerBody body = slamAttack.hitPlayer;

        while (doneCarry == false)
        {
            yield return new WaitForSeconds(0.35f);

            body.DecHealth(2f);

            yield return new WaitForSeconds(0.35f);

        }

        yield return null;
    }
    
    IEnumerator Drop(PlayerBody pb)
    {
        shaking = false;
        pb.timesHit = 0;
        GM.ResetPlayer(pb);
        pb.Grabbed = false;
        pb.MoveToEnemy(enemyPosition.gameObject); // asks to move player to enemy
        StartCoroutine(PickUpAgainCoolDown());
        if(!pb.spriteRenderer.enabled)
            pb.ResetSprite(": FROM DROP CORUT");

        SetGrabAnim(pb, false);
        EAC.Grabbing = false;
        belowChecker.enabled = false;
        slamAttack.enabled = false;
        pb.Grabbed = false;
        pb.playerLock = false;
        pb.canRelease = false;
        ResetCarryVar();

        doneCarry = true;
        slamAttack.hitPlayer = null; // could cause erropr
        

        yield return null;
        //StopDrops();
    }

    public IEnumerator PickUpAgainCoolDown()
    {
        yield return new WaitForSeconds(2);
        Debug.Log("PICKUP AGAIN COOLDOWN: COMPLETE");
        belowChecker.enabled = true;
        slamAttack.enabled = true;
        canPickup = true;
        canCarry = true;
        startSlam = false;
        doneCarry = false;
        carrying = false;
    }
    
    public void ResetCarryVar()
    {
        couts = new List<Coroutine>();
        startCarryingUp = false;
        startCarrying = false;
        //doneCarry = false;
        noTransform = true;
        canSpam = false;
        doneStun = false;
        //canPickup = true;
        StartCoroutine(PickUpAgainCoolDown());
        randTrans = new Vector3(0, 0, 0);
    }


    //starts the heavy dash if in range
    public void StartDash()
    {

        //Debug.Log("Outside the If statement");
        if (isHeavyDashing)
        {
            //Debug.Log("Made it to the if statement");
            isHeavyDashing = false;
            
            StartCoroutine(Dash());
        }
        else
        {
            if (Vector3.Distance(enemyPosition.transform.position, TargetPlayerPos) > 1f && !knockback)
            {
                movementVector = (TargetPlayerPos - enemyPosition.transform.position).normalized * HeavyDashSpeed;
                movementVector.y = 0;
                enemyPosition.transform.position += movementVector * Time.deltaTime;//moves to player
            }
            else
            {
                EAC.Moving = false;
                EAC.Dashing = false;
            }
            if (TargetPlayerPos.x > enemyPosition.transform.position.x)
            {
                //EAC.SR.flipX = false;
            }
            else
            {
                //EAC.SR.flipX = true;
            }

        }
        //enemyBody.transform.position = new Vector3(enemyBody.position.x, 0f, enemyBody.position.z); //keeps it on ground
        

        

    }

    //moves fastly towars the player direction to try and knock them back
    
    IEnumerator Dash()
    {
        if (closest != null)
        {
            EAC.Dashing = true;

            TargetPlayerPos = new Vector3(closest.transform.position.x + Random.Range(-2,2), closest.transform.position.y, closest.transform.position.z + Random.Range(-2,2));

            Damage.enabled = true;

            yield return new WaitForSeconds(1.2f);

            
            Damage.enabled = false;
            EAC.Dashing = false;
            ResetDashVar();
            isDoneDashing = true;
            dashOnCD = true;
            StartCoroutine(cooldown());
        }
        //yield return null;
    }

    public void ResetDashVar()
    {
        isHeavyDashing = true;
    }

    float knockbackTimer;
    float knockbackTime = 0.10f;
    public bool knockback;
    IEnumerator KnockBack(Vector3 playerPos)
    {
        Debug.Log("KNOCKBACK");
        knockback = true;
        Vector3 dir = -(playerPos-enemyPosition.position).normalized * 500;
        while(knockbackTimer < knockbackTime)
        {
            knockbackTimer += Time.deltaTime;
            if (tag == "Medium" || tag == "Heavy")
                dir.y = 0;
            if (knockbackTimer <= 0.05f)
                enemyPosition.transform.position += dir * Time.deltaTime;
            yield return null;
        }
        knockback = false;
        knockbackTimer = 0;
    }

    public void StartShooting()
    {
        if(startShooting)
        {

            if(closest != null && closest.transform.position.x + 1 > enemyPosition.transform.position.x) 
            {

                EAC.SR.flipX = false;
                bodyShootLoc.localPosition = new Vector3(2.2f, 5.5f, 0);
                correctTooth = toothRight;

                if(closest.transform.position.z + 1 < enemyPosition.transform.position.z)
                {
                    ToothShotLocation.transform.localPosition = new Vector3(7.5f, 5.5f, 1.75f);
                    shootDown = true;
                }
                else
                {
                    ToothShotLocation.transform.localPosition = new Vector3(7.5f, 5.5f, -1.75f);
                    shootUp = true;
                }

            }
            else if (closest != null)
            {
                EAC.SR.flipX = true;
                bodyShootLoc.localPosition = new Vector3(-2.2f, 5.5f, 0);
                correctTooth = toothLeft;

                if (closest.transform.position.z + 1 < enemyPosition.transform.position.z)
                {
                    ToothShotLocation.transform.localPosition = new Vector3(-7.5f, 5.5f, 1.75f);
                    shootDown = true;
                }
                else
                {
                    ToothShotLocation.transform.localPosition = new Vector3(-7.5f, 5.5f, -1.75f);
                    shootUp = true;
                }
            }

            startShooting = false;
            StartCoroutine(ShootRoutine());
        }

        
    }

    IEnumerator ShootRoutine()
    {

        

        while (canShoot)
        {
            EAC.Attacking = true;

            if(shootUp)
            {
                ToothShotLocation.transform.localPosition += new Vector3(0, 0, 0.75f);
            }
            else if(shootDown) 
            {
                ToothShotLocation.transform.localPosition -= new Vector3(0, 0, 0.75f);
            }

            

            StartShootTeeth(bodyShootLoc, ToothShotLocation.transform, correctTooth);
            heavyBullets--;
            yield return new WaitForSeconds(0.25f);
        }

        ToothShotLocation.transform.localPosition = new Vector3(7.5f,5.5f, 1.75f);

        EAC.Attacking = false;
        shootOnCD = true;
        shootUp = false;
        shootDown = false;
        StartCoroutine(cooldown());

        yield return null;
    }

    private void StartShootTeeth(Transform body, Transform fireLocation, GameObject toothToShoot)
    {
        if (toothLeft && toothRight)
        {
            Vector3 direction = fireLocation.position - body.position;
            direction.y = 0;
            GameObject ToothGO = null;
            direction.Normalize();
            if (toothToShoot != null)
                ToothGO = GameObject.Instantiate(toothToShoot, bodyShootLoc.position, Quaternion.identity);


            if (ToothGO != null)
            {
                Tooth Tooth = ToothGO.GetComponent<Tooth>();
                ToothGO.SetActive(true);
                Tooth.Fire(direction);
            }
        }
    }

    public void ResetShotVar()
    {
        startShooting = true;
    }

    public void StartShockwave()
    {
        if(startShock)
        {
            
            shockwaveCol.SetActive(true);
            noShockCooldown = false;
            startShock = false;
            canShoot = false;
            StartCoroutine(Shockwave());
        }
    }

    IEnumerator Shockwave()
    {

        yield return new WaitForSeconds(0.7f);

        for (int i = 0; i < 3; i++) 
        {
            EAC.ShockWave = true;

            while (shockwaveCol.transform.localScale.x < targetShockwaveScale.x)
            {
                shockwaveCol.transform.localScale = new Vector3(shockwaveCol.transform.localScale.x + 1, shockwaveCol.transform.localScale.y + 1, shockwaveCol.transform.localScale.z);
                yield return new WaitForSeconds(0.03f);
            }

            
            shockwaveCol.transform.localScale = shockwaveScaleInitial;
        }

        
        doneShockwave = true;
        AddHealth(5f);
        canShoot = true;
        shockwaveCol.SetActive(false);
        shockwaveOnCD = true;
        StartCoroutine(cooldown());

        yield return null;
    }

    

    public void ResetShockVar()
    {
        startShock = true;
        doneShockwave = false;
    }

    public void StartReload()
    {

        if(startReload)
        {

            startReload = false;
            StartCoroutine(Reload());
        }

        
    }

    IEnumerator Reload()
    {

        yield return new WaitForSeconds(0.75f);

        heavyBullets = maxAmmo;
        doneReloading = true;

        yield return null;
    }   
    
    public void ResetReloadVar()
    {
        startReload = true;
        doneReloading = false;
        canShoot = true;
    }

    public IEnumerator FlashRed(SpriteRenderer s)
    {

        //s.color = s.color * 1000;

        yield return new WaitForSeconds(0.15f);

        //s.color = Color.white;
    }


    // all cooldowns

    

    public IEnumerator cooldown()
    {

        
        if(gameObject.CompareTag("Medium"))
        {
            if(dashOnCD == true)
            {
                yield return new WaitForSeconds(0.77f);
                dashOnCD = false;
            }
            else if(roamOnCD == true)
            {
                yield return new WaitForSeconds(0.77f);
                roamOnCD = false;
            }

        }
        else if(gameObject.CompareTag("Heavy"))
        {
            if(shockwaveOnCD == true)
            {
                yield return new WaitForSeconds(6.5f);
                shockwaveOnCD = false;
            }
            else if(shootOnCD == true) 
            {
                yield return new WaitForSeconds(2f);
                shootOnCD = false;
            }
        }

        yield return null;
    }

    //seperation

    public void StartSeperation()
    {
        if(canSeperate)
        {
            canSeperate = false;
            StartCoroutine(Seperation());
        }

        

        if (goLeft)
        {
            if(gameObject.CompareTag("Medium"))
            {
                movementVector = (SepLoc.position - enemyPosition.transform.position).normalized * mediumSpeed;
                movementVector.y = 0;
                enemyPosition.transform.position += movementVector * Time.deltaTime;//moves to player

                if (closest != null && enemyPosition != null && closest.transform.position.x + 1 > enemyPosition.transform.position.x)
                {
                    //EAC.SR.flipX = false;
                }
                else
                {
                    //EAC.SR.flipX = true;
                }

            }
            else if(gameObject.CompareTag("Heavy"))
            {
                movementVector = (SepLoc.position - enemyPosition.transform.position).normalized * heavySpeed;
                movementVector.y = 0;
                enemyPosition.transform.position += movementVector * Time.deltaTime;//moves to player

                if (closest != null && enemyPosition != null && closest.transform.position.x + 1 > enemyPosition.transform.position.x)
                {
                    //EAC.SR.flipX = false;
                }
                else
                {
                    //EAC.SR.flipX = true;
                }

            }
            
        }
        else if (goRight)
        {
            if (gameObject.CompareTag("Medium"))
            {
                movementVector = (SepLoc.position - enemyPosition.transform.position).normalized * mediumSpeed;
                movementVector.y = 0;
                enemyPosition.transform.position += movementVector * Time.deltaTime;//moves to player

                if (closest != null && enemyPosition != null && closest.transform.position.x + 1 > enemyPosition.transform.position.x)
                {
                    //EAC.SR.flipX = false;
                }
                else
                {
                    //EAC.SR.flipX = true;
                }

            }
            else if (gameObject.CompareTag("Heavy"))
            {
                movementVector = (SepLoc.position - enemyPosition.transform.position).normalized * heavySpeed;
                movementVector.y = 0;
                enemyPosition.transform.position += movementVector * Time.deltaTime;//moves to player

                if (closest != null && enemyPosition != null && closest.transform.position.x + 1 > enemyPosition.transform.position.x)
                {
                    //EAC.SR.flipX = false;
                }
                else
                {
                    //EAC.SR.flipX = true;
                }

            }
        }

        

    }


    IEnumerator Seperation()
    {

        if(otherAI != null && otherAI.transform.position.x > enemyPosition.position.x)
        {
            int randLeft = Random.Range(-5, 5);

            SepLoc.localPosition = new Vector3(-5, 0, randLeft);

            goLeft = true;

            yield return new WaitForSeconds(0.6f);

            goLeft = false;

        }
        else
        {
            int randRight = Random.Range(-5, 5);

            SepLoc.localPosition = new Vector3(5, 0, randRight);

            goRight = true;

            yield return new WaitForSeconds(0.6f);

            goRight = false;
        }

        sepCheck.enabled = true;
        canSeperate = true;
        doneSeperating = true;

        yield return null;
    }

    public void StartWallSeperation()
    {
        if(canWall)
        {
            canWall = false;
            StartCoroutine(wallSeperation());
        }

        if(gameObject.CompareTag("Medium") && wallGo)
        {
            movementVector = (SepLoc.position - enemyPosition.transform.position).normalized * mediumSpeed;
            movementVector.y = 0;
            enemyPosition.transform.position += movementVector * Time.deltaTime;//moves to player
        }
        else if(gameObject.CompareTag("Heavy") && wallGo)
        {
            //Debug.Log("HEAVY WALL HIT");
            movementVector = (SepLoc.position - enemyPosition.transform.position).normalized * heavySpeed;
            movementVector.y = 0;
            enemyPosition.transform.position += movementVector * Time.deltaTime;//moves to player
        }
        else
        {
            doneWall = true;

        }

    }

    IEnumerator wallSeperation()
    {
        SepLoc.localPosition = new Vector3(-SepLoc.localPosition.x, -SepLoc.localPosition.y, -SepLoc.localPosition.z);

        wallGo = true;

        if (CompareTag("Medium"))
        {
            yield return new WaitForSeconds(0.35f);
        }     
        else
        {
            yield return new WaitForSeconds(2f);
        }


            
        wallGo = false;
        doneWall = true;
        wallContact = false;
        


        
        yield return null;
    }



}
