using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using UnityEngine.Rendering;
using Unity.VisualScripting;



[System.Serializable]
public abstract class AIProperties // the properties that are most commonly used by all states and are now accesible to those states
{
    public float speed = 3.0f; //speed of enemy
    
}





public class EnemyAIController : AdvancedFSM
{


    [SerializeField]
    private EnemyAnimationController EAC;
    [SerializeField]
    GameManager GM;
    [SerializeField]
    private bool debugDraw;//draws the debug text
    [SerializeField]
    private Text StateText;//shows the state the enmy is currently on
    [SerializeField]
    private Text HealthText;//shows enemy current health

    //for finding the players
    [SerializeField]
    private GameObject[] Players = new GameObject[4]; //holds all players
    private GameObject closest;//holds the closest player
    private float currShortest = 100000f; //current shortest distance
    private Vector3 movementVector = Vector3.zero; // the vector that the enemy is moving towards

    //the enemy body
    [SerializeField]
    private Transform enemyPosition; //holds the enemy player position
    [SerializeField]
    private GameObject enemy;
    [SerializeField]
    private Rigidbody enemyBody;

    public Rigidbody eBody { get { return enemyBody; } }

    public Transform ePosition { get { return enemyPosition; } }

    //speedChanges
    [SerializeField]
    private float speed = 5f; //speed of the enemy
    [SerializeField]
    private float HeavyDashSpeed = 2f;

    public bool canMove = true;



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

    private bool canStun;
    private bool canCarry;
    private bool startCarryingUp;
    private bool startCarrying;

    [SerializeField]
    private Transform upPlacement;

    private bool noTransform;

    [SerializeField]
    private Vector3 randTrans;

    private PlayerCarried carriedPlayer;


    //health, finisher, and death states
    public float maxHealth = 40; // its total Health
    [SerializeField]
    private float health = 40;//health of the enemy
    

    [SerializeField]
    private Collider Damage;


    

    
    public float Health
    {
        get { return health; }
    }
    public void DecHealth(float amount) { health = Mathf.Max(0, health - amount); }//allows us to decrease the health of an enemy
    public void AddHealth(float amount) { health = Mathf.Min(100, health + amount); }//allows us to add health to the enemy

    

    

    //allows us to grab the state in which the enemy should be on
    private string GetStateString()
    {

        string state = "NONE";
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
        else if (CurrentState.ID == FSMStateID.Hole)
        {
            state = "HOLE";
        }
        


        return state;
    }

    //intializes the enemy with the player location and sets enemy health to 100 theb calls Construct FSM
    protected override void Initialize()
    {
        

        Players = GameObject.FindGameObjectsWithTag("Player");//finds and add all players to array
        if(Players.Length <= 0) { Players = null; }
        if (Players != null)
        {
            playerTransform = Players[0].transform;
            closest = playerTransform.gameObject;
        }
        Damage = gameObject.GetComponent<Collider>();
        Damage.enabled = false; // deactivates the damage collider
        isHeavyDashing = true;
        health = 40;
        maxHealth = 40;

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

        noTransform = true;

        randTrans = new Vector3(0,0,0);

        ConstructFSM();
    }
    public bool act = false;
    protected override void FSMUpdate()
    {
        act = true;
        if (CurrentState != null && act)
        {
            CurrentState.Reason(playerTransform, transform);
            CurrentState.Act(playerTransform, transform);
        }
        

        
    }

    //lets us add states and transitions in which we can use to move to other states when needed.
    private void ConstructFSM()
    {

        //follows player, transitions out if it is above the player
        FindPlayerState findPlayerState = new FindPlayerState(this);
        findPlayerState.AddTransition(Transition.AbovePlayer, FSMStateID.SlamGround);
        findPlayerState.AddTransition(Transition.InFirstRange, FSMStateID.HeavyDash);
        findPlayerState.AddTransition(Transition.InShootingRange, FSMStateID.Gun);
        findPlayerState.AddTransition(Transition.InShockwaveRange, FSMStateID.Shockwave);
        findPlayerState.AddTransition(Transition.LowHealth, FSMStateID.Finished);
        findPlayerState.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        findPlayerState.AddTransition(Transition.PlayerDead, FSMStateID.FindPlayer);

        //if at low health it allows the enemy to be finished, tranistions if no health and not finished.
        FinishedState finishedState = new FinishedState(this);
        finishedState.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        finishedState.AddTransition(Transition.HealthBack, FSMStateID.FindPlayer);
        
        

        //ded
        DeadState deadState = new DeadState(this);
        deadState.AddTransition(Transition.WasNotExecuted, FSMStateID.Hole);

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
        gunState.AddTransition(Transition.LowHealth, FSMStateID.Finished);
        gunState.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        //sends out a mini shockwave to knock players away from it, Tranisition if done to find player, low health if finish, no health to dead
        ShockwaveState shockwaveState = new ShockwaveState(this);
        shockwaveState.AddTransition(Transition.LookForPlayer, FSMStateID.FindPlayer);
        shockwaveState.AddTransition(Transition.LowHealth, FSMStateID.Finished);
        shockwaveState.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        //Reloads if out of teeth, Tranisition if done to find player, low health if finished, no health to dead
        ReloadState reloadState = new ReloadState(this);
        reloadState.AddTransition(Transition.LookForPlayer, FSMStateID.FindPlayer);
        reloadState.AddTransition(Transition.LowHealth, FSMStateID.Finished);
        reloadState.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        //hole
        HoleState holeState = new HoleState(this);


        //Add all states to the state list

        AddFSMState(findPlayerState);
        AddFSMState(finishedState);
        AddFSMState(deadState);

        AddFSMState(slamGroundState);
        AddFSMState(carryState);
        AddFSMState(stunnedState);

        AddFSMState(heavyDashState);
        

        AddFSMState(gunState);
        AddFSMState(shockwaveState);
        AddFSMState(reloadState);
        AddFSMState(holeState);
        
        
    }

    //finds the closest player and sets the target position
    public void checkShortestDistance()
    {
        if (Players != null)
        {
            float check;

            //simple distance check where it checks the current shortest and compares to the other players, replacing when neccisary
            for (int i = 0; i < Players.Length; i++)
            {

                PlayerBody currentBody = Players[i].GetComponent<PlayerBody>();

                if (currentBody.alreadyDead == false)
                {
                    check = Vector3.Distance(enemyPosition.transform.position, Players[i].transform.position);


                    if (check < currShortest)
                    {

                        currShortest = check;
                        closest = Players[i];
                        playerTransform = closest.transform;

                    }

                }
            }
            if (!knockback)
            {
                if(gameObject.CompareTag("Light"))
                {
                    setAndMoveToTargetLight(speed);
                }
                else
                {
                    setAndMoveToTarget(speed);
                }
            }
                
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
        //Debug.Log(speed);
        if (!lockKnock)
        {
            movementVector = (closest.transform.position - enemyPosition.transform.position).normalized * Speed;
            enemyPosition.transform.position += movementVector * Time.deltaTime;//moves to player
        }
        //enemyBody.transform.position = new Vector3(enemyBody.position.x, 0, enemyBody.position.z); //keeps it on ground
        if (closest.transform.position.x > enemyPosition.transform.position.x)
        {
            EAC.SR.flipX = false;
        }
        else
        {
            EAC.SR.flipX = true;
        }

       

    }

    private void setAndMoveToTargetLight(float Speed)
    {
        Debug.Log("in this one");
        if (Speed > 0.5f)
        {
            EAC.Moving = true;
        }
        else
        {
            EAC.Moving = false;
        }
        //Debug.Log(speed);
        Vector3 currPlayerPos = new Vector3(closest.transform.position.x, closest.transform.position.y + 30, closest.transform.position.z);
        movementVector = (currPlayerPos - enemyPosition.transform.position).normalized * Speed;
        enemyPosition.transform.position += movementVector * Time.deltaTime;//moves to player
        if (closest.transform.position.x > enemyPosition.transform.position.x)
        {
            EAC.SR.flipX = false;
        }
        else
        {
            EAC.SR.flipX = true;
        }

        if (startGoop)
        {
            startGoop = false;
            StartCoroutine(GoopRoutine());
        }
        
        if(belowChecker.IsPlayerBelow())
        {
            canGoop = false;
            startSlam = true;
        }

    }

    

    IEnumerator GoopRoutine()
    {

        while(canGoop)
        {
            StartShootGoop(enemyPosition, fireLocation);
            yield return new WaitForSeconds(0.45f);
        }

        //yield return null;
    }

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



    bool inFinish;
    //starts the finished coroutine
    public void StartFinish()
    {
        if (!inFinish)
        {
            inFinish = true;
            StartCoroutine(Finished());
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
    }
    bool dead;

    //destroys the enemy game object
    IEnumerator Death()
    {


        //animation here
        EAC.Dead = true;
        //scale time to animation

        yield return new WaitForSeconds(0.7f);

        Debug.Log("DEATH");
        DropEyes();

        yield return new WaitForSeconds(2.2f);

        LevelManager.Instance.EnemyKilled();
        Destroy(enemy);

        //yield return null;
    }

    [SerializeField]
    private GameObject eyes;

    private void DropEyes()
    {
        int dropRate = 0;

        if (gameObject.CompareTag("Light"))
        {
            dropRate = Random.Range(1, 3);
        }
        else if (gameObject.CompareTag("Medium"))
        {
            dropRate = Random.Range(2, 6);
        }
        else if (gameObject.CompareTag("Heavy"))
        {
            dropRate = Random.Range(4, 9);
        }

        for (int i = 0; i < dropRate; i++)
        {
            Instantiate(eyes, enemyBody.transform.position + new Vector3(Random.Range(-10, 10), transform.position.y, Random.Range(-10, 10)), Quaternion.identity);
        }
    }
    public bool lockKnock;
    public IEnumerator KB(Vector3 dir)
    {
        lockKnock = true;
        enemyBody.isKinematic = false;
        enemyBody.AddForce(dir);
        yield return new WaitForSeconds(0.8f);
        enemyBody.velocity = Vector3.zero;
        enemyBody.isKinematic = true;
        lockKnock = false;
    }



    public void StartSlam()
    {
        
            
        
        StartCoroutine(SlamAttack());

        
            
        
        if(slamAttack.hasHit == false && slamAttack.HitGround == false)
        {
            movementVector = (SlamLocation.position - enemyPosition.transform.position).normalized * slamSpeed;
            enemyPosition.transform.position += movementVector * Time.deltaTime;//moves to player
        }
    }

    
    

    IEnumerator SlamAttack()
    {
        
        EAC.Attacking = true;
        if (slamAttack.hasHit == true)
        {
            moveToCarry = true;
        Debug.Log("Carry");
                
        }
        else if (slamAttack.HitGround == true)
        {
            moveToStunned = true;
            Debug.Log("Stunned");    

        }
        //Can this coroutine end??
        yield return new WaitForSeconds(1);
    }

    public void ResetSlamVar()
    {
        slamAttack.hasHit = false;
        slamAttack.HitGround = false;
        moveToCarry = false;
        moveToStunned = false;
    }

    public void StartStunned()
    {
        if (canStun)
        {
            canStun = false;
            StartCoroutine(Stunned());
        }

        if (doneOnGround == true)
        {
            if (doneStun == false)
            {
                Debug.Log("LetsGoUp");
                movementVector = (upPlacement.position - enemyPosition.transform.position).normalized * speed;
                enemyPosition.transform.position += movementVector * Time.deltaTime;//moves to player
            }
        }
    }

    IEnumerator Stunned()
    {
        yield return new WaitForSeconds(3f);

        doneOnGround = true;

        yield return new WaitForSeconds(2f);

        doneStun = true;

        //yield return null; //CAN THIS EXIT?
    }

    public void StartCarry()
    {

        PlayerBody body = slamAttack.hitPlayer;

        carriedPlayer = slamAttack.hitPlayer.GetComponentInParent<PlayerCarried>();

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
            StartCoroutine(Carry());
        }

        if(startCarryingUp == true)
        {
            
            Debug.Log("Lets go up");
            movementVector = (upPlacement.position - enemyPosition.transform.position).normalized * speed;
            enemyPosition.transform.position += movementVector * Time.deltaTime;//moves to player

            
        }

        if (startCarrying == true)
        {
            if (noTransform)
            {
                noTransform = false;
                randTrans = new Vector3(xDir * (randTrans.x + (int)Random.Range(1, 200)), randTrans.y, zDir * (randTrans.z + (int)Random.Range(1, 200)));
            }

            Debug.Log(randTrans);
            movementVector = (randTrans - enemyPosition.transform.position).normalized * speed;
            enemyPosition.transform.position += movementVector * Time.deltaTime;//moves to player
        }
        //call player carry method, move the light enemy to a random point, if timer runs out, drop player go to find player, use rand on an x and z for a random direction
        
        

    }

    IEnumerator Carry()
    {
        GameObject hitBody = slamAttack.hitPlayerBody;

        slamAttack.gameObject.SetActive(false);

        hitBody.SetActive(false);
        
         
        startCarryingUp = true;
        belowChecker.gameObject.SetActive(false);
        
            yield return new WaitForSeconds(3f);
        

        startCarryingUp = false;

        startCarrying = true;

        yield return new WaitForSeconds(4f);

        startCarrying = false;

        hitBody.transform.position = new Vector3(enemyPosition.transform.position.x, enemyPosition.transform.position.y - 15, enemyPosition.transform.position.z);

        hitBody.SetActive(true);

        
        
        doneCarry = true;

        yield return new WaitForSeconds(5f);
        belowChecker.gameObject.SetActive(true);
        slamAttack.gameObject.SetActive(true);
        yield return null;
        

        
    }   
    
    public void RestCarryVar()
    {
        canCarry = true;
        startCarryingUp = false;
        startCarrying = false;
        doneCarry = false;
        noTransform = true;
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
            if (Vector3.Distance(enemyPosition.transform.position, TargetPlayerPos) > 0.5f && !knockback)
            {
                movementVector = (TargetPlayerPos - enemyPosition.transform.position).normalized * HeavyDashSpeed;
                enemyPosition.transform.position += movementVector * Time.deltaTime;//moves to player
            }
            else
            {
                EAC.Moving = false;
                EAC.Dashing = false;
            }
            if (TargetPlayerPos.x > enemyPosition.transform.position.x)
            {
                EAC.SR.flipX = false;
            }
            else
            {
                EAC.SR.flipX = true;
            }

        }
        //enemyBody.transform.position = new Vector3(enemyBody.position.x, 0f, enemyBody.position.z); //keeps it on ground
        

        

    }

    //moves fastly towars the player direction to try and knock them back
    
    IEnumerator Dash()
    {
        EAC.Dashing = true;
        TargetPlayerPos = closest.transform.position;
        
        Damage.enabled = true;

        yield return new WaitForSeconds(2.5f);

        ResetDashVar();
        Damage.enabled = false;
        EAC.Dashing = false;

        //yield return null;
    }

    public void ResetDashVar()
    {
        isDoneDashing = false;
        isHeavyDashing = true;
    }

    float knockbackTimer;
    float knockbackTime = 0.10f;
    bool knockback;
    IEnumerator KnockBack(Vector3 playerPos)
    {
        Debug.Log("KNOCKBACK");
        knockback = true;
        Vector3 dir = -(playerPos-enemyPosition.position).normalized * 500;
        while(knockbackTimer < knockbackTime)
        {
            knockbackTimer += Time.deltaTime;
            if(knockbackTimer <= 0.05f)
                enemyPosition.transform.position += dir * Time.deltaTime;
            yield return null;
        }
        knockback = false;
        knockbackTimer = 0;
    }

    //bool damaging;
    float timer;
    public float timeToDamage = 2f;
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (timer > timeToDamage)
            {
                timer = 0;
                //damaging = true;
                PlayerBody pb = other.gameObject.GetComponent<PlayerBody>();
                if (!pb.canTakeDamage)
                {
                    pb.DecHealth(1);
                    EAC.Attacking = true;
                }
                   
            }
            else
            {
                timer += Time.deltaTime;
                EAC.Attacking = false;
            }
            
        }
        if (other.tag == "PlayerAttack" && !knockback)
        {
            Debug.Log("FUCK:: ENEMY KNOCKBACK");
           // StartCoroutine(KnockBack(other.transform.position));
        }
    }

    


}
