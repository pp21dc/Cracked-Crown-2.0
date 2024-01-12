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
    private GameObject Bullet;//add a bullet for the enmy to shoot

    [SerializeField]
    private Transform firelocation;//adds a fire location later used to help find the direction for the bullet when instantiated
    [SerializeField]
    private Transform gun;//adds a gun transform later used to help find the direction for the bullet when instantiated

    public float magSize = 8;//amount of bullet that can be shot before needing to reload

    private bool canShoot = true; //if the enemy can shoot

    private bool isReloading = true; //if the enemy is reloading

    [SerializeField]
    private Animator animator; //grabs the animator

    [SerializeField]
    private ParticleSystem deathEffect;

    [SerializeField]
    private AudioSource audioSource;//what manages the noises

    [SerializeField]
    private AudioClip ouch;//when the enemy is hit

    [SerializeField]
    private AudioClip death; //when the enemy dies


    private float health;//health of the enemy
    public float Health
    {
        get { return health; }
    }
    public void DecHealth(float amount) { health = Mathf.Max(0, health - amount); }//allows us to decrease the health of an enemy
    public void AddHealth(float amount) { health = Mathf.Min(100, health + amount); }//allows us to add health to the enemy

    public void Awake()
    {
        audioSource.clip = ouch;//sets the default audio to ouch
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

    //starts a death coroutine
    public void StartDeath()
    {

        audioSource.volume = 0.15f;//turns down volume (your welcome)
        audioSource.PlayOneShot(death);//plays death audio (I am so sorry)
        StartCoroutine(Death());
    }

    //Starts a firing coroutine
    public void StartFiring()
    {
        if (canShoot)
        {
            StartCoroutine(Fire(firelocation, gun));
            canShoot = false;
        }
    }

    
    //set a reloading coroutine
    public void StartReloading()
    {
        

        animator.SetFloat("Speed", 1);//sets animation to run
        

        if (isReloading)
        {

            isReloading = false;
            StartCoroutine(Reload(this));

        }
    }

    public void StartFleeAnimation()
    {
        animator.SetFloat("Speed", 1); //sets the animation to run
    }

    //destroys the enemy game object
    IEnumerator Death()
    {

        deathEffect.Play();

        animator.SetTrigger("isDead");

        

        yield return new WaitForSeconds(2.2f);

        Destroy(gameObject);

        yield return null;
    }

    //Same logic as the player and turret fire, uses the firelocation and gun to find the direction to shoot and instantiates a bullet going in said direction 
    IEnumerator Fire(Transform fireLocation, Transform Gun) 
    {

        for (int i = 0; i < 8; i++)//runs until the mag is empty, decreasing the mag by one every shot
        {

            Shoot(fireLocation, Gun);

            magSize--;

            //decrease mag

            yield return new WaitForSeconds(0.75f);

        }

        canShoot = true;//allows us to shoot again once reloaded
        yield return null;

    }

    //set the mag back to 8 after waiting a bit of time so we can shoot again
    IEnumerator Reload(EnemyAIController enemy)
    {

        animator.SetTrigger("isReloading");//starts reloading animation

        yield return new WaitForSeconds(4f);

        magSize = 8;
        isReloading = true;//allows us to reload again

        animator.SetFloat("Speed", 0);


        yield return null;
    
    }

    //when the enemy is hit with a playerBullet, it decreases the enemy health by ten
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("playerBullet"))
        {

            this.DecHealth(10);
            audioSource.Play();//plays hit audio
            other.gameObject.SetActive(false);

        }
    }

    //fires a bullet when called
    public void Shoot(Transform fireLocation, Transform Gun)
    {

        if (Bullet)
        {

            Vector3 direction = fireLocation.position - Gun.position;
            direction.y = 0f;
            direction.Normalize();

            GameObject bulletGo = GameObject.Instantiate(Bullet, fireLocation.position, Quaternion.identity);

           // Bullet bullet = bulletGo.GetComponent<Bullet>();

            bulletGo.SetActive(true);

            //bullet.Fire(direction);

            animator.SetTrigger("isFiring");//runs the shoot animation

        }

    }

}
