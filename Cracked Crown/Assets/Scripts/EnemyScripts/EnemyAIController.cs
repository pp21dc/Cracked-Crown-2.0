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

    private bool isPlayerFound = false;
    public bool isPlayerfound {  get { return isPlayerFound; } }

    [SerializeField]
    private float speed = 0.008f; //speed of the enemy




    private float health;//health of the enemy
    public float Health
    {
        get { return health; }
    }
    public void DecHealth(float amount) { health = Mathf.Max(0, health - amount); }//allows us to decrease the health of an enemy
    public void AddHealth(float amount) { health = Mathf.Min(100, health + amount); }//allows us to add health to the enemy

    public void Awake()
    {
        
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
        else if (CurrentState.ID == FSMStateID.FollowPlayer)
        {
            state = "FollowPlayer";
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
        else if (CurrentState.ID == FSMStateID.Finished)
        {
            state = "Finished";
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

        FindPlayerState findPlayerState = new FindPlayerState(this);
        findPlayerState.AddTransition(Transition.PlayerFound, FSMStateID.FollowPlayer);

        FollowPlayerState followPlayerState = new FollowPlayerState(this);
        followPlayerState.AddTransition(Transition.AbovePlayer, FSMStateID.SlamGround);

        SlamGroundState slamGroundState = new SlamGroundState(this);
        slamGroundState.AddTransition(Transition.SlamSuceeded, FSMStateID.Carry);
        slamGroundState.AddTransition(Transition.SlamFailed, FSMStateID.Stunned);
        slamGroundState.AddTransition(Transition.LowHealth, FSMStateID.Finished);
        slamGroundState.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        CarryState carryState = new CarryState(this);
        carryState.AddTransition(Transition.LookForPlayer, FSMStateID.FindPlayer);

        StunnedState stunnedState = new StunnedState(this);
        stunnedState.AddTransition(Transition.LookForPlayer, FSMStateID.FindPlayer);
        stunnedState.AddTransition(Transition.LowHealth, FSMStateID.Finished);
        stunnedState.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        FinishedState finishedState = new FinishedState(this);
        finishedState.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        DeadState deadState = new DeadState(this);


        //Add all states to the state list

        AddFSMState(findPlayerState);
        AddFSMState(followPlayerState);
        AddFSMState(slamGroundState);
        AddFSMState(carryState);
        AddFSMState(stunnedState);
        AddFSMState(finishedState);
        AddFSMState(deadState);
    }

    //finds the closest player and sets the target position
    public void checkShortestDistance()
    {

        float check;

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


    //starts a death coroutine
    public void StartDeath()
    {

        
        StartCoroutine(Death());
    }

    

    
    

    

    //destroys the enemy game object
    IEnumerator Death()
    {

        

        

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
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("playerBullet"))
        {

            this.DecHealth(10);
            
            other.gameObject.SetActive(false);

        }
    }

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
