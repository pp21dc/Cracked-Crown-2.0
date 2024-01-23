using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

[System.Serializable]
public abstract class AIProperties // the properties that are most commonly used by all states and are now accesible to those states
{
    public float speed = 3.0f; //speed of enemy
    
}





public class EnemyAIController : AdvancedFSM
{

    

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
    private Transform enemyBody; //holds the enemy player position

    public Transform eBody { get { return enemyBody; } }

    //speedChanges
    [SerializeField]
    private float speed = 5f; //speed of the enemy
    [SerializeField]
    private float HeavyDashSpeed = 2f;
    

    //Medium needed variables
    public Transform TargetPlayerPos;
    public bool isHeavyDashing = true;
    public bool isDoneDashing = false;
    

    //health, finisher, and death states
    public float maxHealth = 100; // its total Health
    private float health = 100;//health of the enemy
    

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
        
        float check;
        
        //simple distance check where it checks the current shortest and compares to the other players, replacing when neccisary
        for (int i = 0; i < Players.Length; i++)
        {

            check = Vector3.Distance(enemyBody.transform.position, Players[i].transform.position);

            if (check < currShortest)
            {

                currShortest = check;
                closest = Players[i];
                playerTransform = closest.transform;

            }
            
        }

        setAndMoveToTarget(speed);

    }

    //sets enemy target position and moves towards it
    private void setAndMoveToTarget(float Speed)
    {
        
        movementVector = (closest.transform.position - enemyBody.transform.position).normalized * Speed;
        enemyBody.transform.position += movementVector * Time.deltaTime;//moves to player
        //enemyBody.transform.position = new Vector3(enemyBody.position.x, 0, enemyBody.position.z); //keeps it on ground

    } 

    //starts the finished coroutine
    public void StartFinish()
    {
        StartCoroutine(Finished());
    }


    //starts a death coroutine
    public void StartDeath()
    {

        
        StartCoroutine(Death());
    }

    //stops the enemy starts the animation and heals the enemy overtime until it can move again
    IEnumerator Finished()
    {

        //stunned animation here

        

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

    
    //starts the heavy dash if in range
    public void StartHeavyDash()
    {
        
        Debug.Log("Outside the If statement");
        if (isHeavyDashing)
        {
            Debug.Log("Made it to the if statement");
            isHeavyDashing = false;
            StartCoroutine(HeavyDash());
        }

        movementVector = (TargetPlayerPos.transform.position - enemyBody.transform.position).normalized * HeavyDashSpeed;
        enemyBody.transform.position += movementVector * Time.deltaTime;//moves to player
        //enemyBody.transform.position = new Vector3(enemyBody.position.x, 0f, enemyBody.position.z); //keeps it on ground


        

    }

    //moves fastly towars the player direction to try and knock them back
    
    IEnumerator HeavyDash()
    {

        TargetPlayerPos = closest.transform;

        Damage.enabled = true;

        yield return new WaitForSeconds(2.5f);

        isHeavyDashing = true;
        isDoneDashing = true;
        Damage.enabled = false;

        yield return null;
    }

    





}
