using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.InteropServices;

[System.Serializable]
public abstract class AIProperties // the properties that are most commonly used by all states and are now accesible to those states
{
    public float speed = 3.0f;
    public float rotSpeed = 2.0f;
    public float maxSize = 8;
}





public class EnemyAIController : AdvancedFSM
{

    

    [SerializeField]
    private bool debugDraw;//draws the debug text
    [SerializeField]
    private Text StateText;//shows the state the enmy is currently on
    [SerializeField]
    private Text HealthText;//shows enemy current health

    [SerializeField]
    private GameObject[] Players = new GameObject[4]; //holds all players


    private GameObject closest;//holds the closest player

    [SerializeField]
    private Transform enemyBody; //holds the enemy player position

    private float currShortest = 100000f; //current shortest distance
    private Vector3 movementVector = Vector3.zero; // the vector that the enemy is moving towards

    private bool isFollowing = false;
    public bool isfollowing { get { return isFollowing; } set { isFollowing = value; } }

    [SerializeField]
    private float speed = 0.008f; //speed of the enemy

    public float maxHealth = 100;

    private bool isInFinishedState = false;

    public bool isinFinishedState { get { return isInFinishedState; } set { isInFinishedState = value; } }

    




    private float health;//health of the enemy
    public float Health
    {
        get { return health; }
    }
    public void DecHealth(float amount) { health = Mathf.Max(0, health - amount); }//allows us to decrease the health of an enemy
    public void AddHealth(float amount) { health = Mathf.Min(100, health + amount); }//allows us to add health to the enemy

    public void Awake()
    {
        Players = GameObject.FindGameObjectsWithTag("Player");//finds and add all players to array
    }

    private void Update()
    {
        if(isFollowing)
        {
            checkShortestDistance();
        }

        if(isInFinishedState)
        {
            AddHealth(0.5f * Time.deltaTime);
        }
    }

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
            state = "FindPlayer";
        }
        else if (CurrentState.ID == FSMStateID.Finished)
        {
            state = "Finished";
        }
        else if (CurrentState.ID == FSMStateID.SlamGround)
        {
            state = "SlamGround";
        }
        else if (CurrentState.ID == FSMStateID.Carry)
        {
            state = "Carry";
        }
        else if (CurrentState.ID == FSMStateID.Stunned)
        {
            state = "Stunned";
        }
        else if (CurrentState.ID == FSMStateID.HeavyDash)
        {
            state = "HeavyDash";
        }
        else if (CurrentState.ID == FSMStateID.LightDash)
        {
            state = "LightDash";
        }
        else if (CurrentState.ID == FSMStateID.Gun)
        {
            state = "Gun";
        }
        else if (CurrentState.ID == FSMStateID.Shockwave)
        {
            state = "Shockwave";
        }
        else if (CurrentState.ID == FSMStateID.Reload)
        {
            state = "Reload";
        }
        else if (CurrentState.ID == FSMStateID.Hole)
        {
            state = "Hole";
        }
        


        return state;
    }

    //intializes the enemy with the player location and sets enemy health to 100 theb calls Construct FSM
    protected override void Initialize()
    {
       // GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
       // playerTransform = objPlayer.transform;
        health = 100;
        ConstructFSM();
    }

    protected override void FSMUpdate()
    {

        if (CurrentState != null)
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
        findPlayerState.AddTransition(Transition.InSecondRange, FSMStateID.LightDash);
        findPlayerState.AddTransition(Transition.InShootingRange, FSMStateID.Gun);
        findPlayerState.AddTransition(Transition.InShockwaveRange, FSMStateID.Shockwave);
        findPlayerState.AddTransition(Transition.LowHealth, FSMStateID.Finished);
        findPlayerState.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        //if at low health it allows the enemy to be finished, tranistions if no health and not finished.
        FinishedState finishedState = new FinishedState(this);
        finishedState.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        finishedState.AddTransition(Transition.HealthBack, FSMStateID.FindPlayer);
        

        //ded
        DeadState deadState = new DeadState(this);
        findPlayerState.AddTransition(Transition.WasNotExecuted, FSMStateID.Hole);

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

        //Slam into player dealing damage, Transition if done to find player, low health to finished, no health to dead
        LightDashState lightDashState = new LightDashState(this);
        lightDashState.AddTransition(Transition.LookForPlayer, FSMStateID.FindPlayer);
        lightDashState.AddTransition(Transition.LowHealth, FSMStateID.Finished);
        lightDashState.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        //Heavy enemy states down here

        GunState gunState = new GunState(this);
        gunState.AddTransition(Transition.NoBullets, FSMStateID.Reload);
        gunState.AddTransition(Transition.LowHealth, FSMStateID.Finished);
        gunState.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        ShockwaveState shockwaveState = new ShockwaveState(this);
        shockwaveState.AddTransition(Transition.LookForPlayer, FSMStateID.FindPlayer);
        shockwaveState.AddTransition(Transition.LowHealth, FSMStateID.Finished);
        shockwaveState.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        ReloadState reloadState = new ReloadState(this);
        reloadState.AddTransition(Transition.LookForPlayer, FSMStateID.FindPlayer);
        reloadState.AddTransition(Transition.LowHealth, FSMStateID.Finished);
        reloadState.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        HoleState holeState = new HoleState(this);


        //Add all states to the state list

        AddFSMState(findPlayerState);
        AddFSMState(finishedState);
        AddFSMState(deadState);

        AddFSMState(slamGroundState);
        AddFSMState(carryState);
        AddFSMState(stunnedState);

        AddFSMState(heavyDashState);
        AddFSMState(lightDashState);

        AddFSMState(gunState);
        AddFSMState(shockwaveState);
        AddFSMState(reloadState);
        AddFSMState(holeState);
        
        
    }

    //finds the closest player and sets the target position
    public void checkShortestDistance()
    {

        float check;
        
        //simple distance check where it checks the current shortest and compares to the other players, replacing when neccisary
        for (int i = 0; i < Players.Length; i++)
        {

            check = Vector3.Distance(gameObject.transform.position, Players[i].transform.position);

            if (check < currShortest)
            {

                currShortest = check;
                closest = Players[i];

            }

        }

        setAndMoveToTarget();

    }

    //sets enemy target position and moves towards it
    private void setAndMoveToTarget()
    {

        movementVector = (closest.transform.position - enemyBody.transform.position).normalized * speed;
        enemyBody.transform.position += movementVector * Time.deltaTime;//moves to player
        enemyBody.transform.position = new Vector3(enemyBody.position.x, 0f, enemyBody.position.z); //keeps it on ground

    } 

    public void StartFinish()
    {
        StartCoroutine(Finished());
    }


    //starts a death coroutine
    public void StartDeath()
    {

        
        StartCoroutine(Death());
    }

    
    IEnumerator Finished()
    {

        //stunned animation here

        isInFinishedState = true;

        yield return new WaitForSeconds(3f);

        
    
    }
    
    

    

    //destroys the enemy game object
    IEnumerator Death()
    {

        
        //animation here
        
        //scale time to animation
        yield return new WaitForSeconds(2.2f);

        Destroy(gameObject);

        yield return null;
    }

    //Same logic as the player and turret fire, uses the firelocation and gun to find the direction to shoot and instantiates a bullet going in said direction 
   /* IEnumerator Fire(Transform fireLocation, Transform Gun) 
    {

        for (int i = 0; i < 8; i++)//runs until the mag is empty, decreasing the mag by one every shot
        {

            Shoot(fireLocation, Gun);

            

            //decrease mag

            yield return new WaitForSeconds(0.75f);

        }

        canShoot = true;//allows us to shoot again once reloaded
        yield return null;

    }
   */
    //set the mag back to 8 after waiting a bit of time so we can shoot again
    

    //when the enemy is hit with a playerBullet, it decreases the enemy health by ten
   /* private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("playerBullet"))
        {

            this.DecHealth(10);
            
            other.gameObject.SetActive(false);

        }
    }*/

    //fires a bullet when called
    public void Shoot(Transform fireLocation, Transform Gun)
    {

       /* if (Bullet)
        {

            Vector3 direction = fireLocation.position - Gun.position;
            direction.y = 0f;
            direction.Normalize();

            GameObject bulletGo = GameObject.Instantiate(Bullet, fireLocation.position, Quaternion.identity);

           // Bullet bullet = bulletGo.GetComponent<Bullet>();

            bulletGo.SetActive(true);

            //bullet.Fire(direction);

            animator.SetTrigger("isFiring");//runs the shoot animation

        }*/

    }

}
