using UnityEngine;
using System.Collections;

public class BossPhases : MonoBehaviour
{

    [SerializeField]
    private Vector3 CLAWSPAWN;

    [SerializeField] // list for the possible boss attacks
    string[] attackList = new string[3];
    

    [Header("Boss Parts")]
    [SerializeField]
    private GameObject Claw;
    [SerializeField]
    private Animator bossAnim;
    [SerializeField]
    private BossPhases otherClaw;
    [SerializeField]
    private GameObject ClawSprite;

    [Header("Instantiated Objects")]
    [SerializeField]
    GameObject ShockWave;

    [Header("Boss Variables")]
    [SerializeField]
    private float clawspeed;
    [SerializeField] // count used to allow the boss to know when to initiate attacks
    public float attacktimer;
    [SerializeField]
    private float bosshealth;
    [SerializeField]
    private float MAXBOSSHEALTH;
    [SerializeField]
    private float clawdropspeed;

    private Vector3 preGrabPlayerPosition;
    private Vector3 clawtarget;
    private float dist;
    private int currattackid;
    public string testAttack;

    bool attackLoop = true; // controls the running of the boss's attacks
    static int LISTLENGTH = 100;

    private bool clawfollow = false; // pincer attack bools
    private bool clawgrab = false;
    public bool clawreturn = false;

    public bool cantakedmg= false;

    public GameObject FollowedPlayer;
    private PlayerBody GrabbedPlayerBody;

    private bool isGrabbed = false;
    private float grabbedTimer;
    [SerializeField]
    private string[] attackIDList = new string[LISTLENGTH];

    // gets references for players
    [Header("Player List")]
    [SerializeField]
    private GameObject[] PlayerList = new GameObject[4];
    [SerializeField]
    private GameObject GrabbedPlayer;

    
    private void Awake()
    {
        currattackid = -1; // starts at -1 because it adds once before it is needed, making it 0
        bosshealth = MAXBOSSHEALTH;
        GameObject[] TempList = GameObject.FindGameObjectsWithTag("BossFollowPoint");
        for (int i = 0; i < TempList.Length; i++)
        {
            PlayerList[i] = TempList[i]; // creates a list of all players in the scene
        }
        CLAWSPAWN = Claw.transform.position; // gets the original claw point
        createIDList(); // creates a randomized list of IDs to decide boss attack order
    }

    void Update()
    {
        /*if (attacktimer <= 0)
        {
            testAttack = "empty";
            TestLoop();
        }
        if (testAttack == "pincer")
        {
            pincerAttack();
        }
        attacktimer -= Time.deltaTime;*/

        if (attackLoop)
       {
           // attacks called in a continuous loop
           if (attacktimer <= 0)
           {
               currattackid++; // counts to next id in the list
               startNextAttack(); // starts the next attack, uses currattackid to do so
           }
           switch (attackIDList[currattackid]) // populates the ID list
           {
               case "PincerAttack":
                   pincerAttack();
                   break;
               case "ClawSmash":
                   break;
               case "GooBlast":
                   break;
           }
           attacktimer -= Time.deltaTime;
       }
    }

    public string displayAttack()
    {
        return attackIDList[currattackid];
    }
    private void TestLoop()
    {
        if (gameObject.name == "clawLeft")
        {
            if (Input.GetKeyDown("7"))
            {
                attacktimer = 8;
                StartCoroutine("PincerAttack");
                testAttack = "pincer";
                Debug.Log(attacktimer);
            }
            if (Input.GetKeyDown("8"))
            {
                attacktimer = 4.5f;
                StartCoroutine("ClawSmash");
                testAttack = "smash";
                Debug.Log(attacktimer);
            }
        }
        if (gameObject.name == "clawRight")
        {
            if (Input.GetKeyDown("9"))
            {
                attacktimer = 8;
                StartCoroutine("PincerAttack");
                testAttack = "pincer";
            }
            if (Input.GetKeyDown("0"))
            {
                attacktimer = 4.5f;
                StartCoroutine("ClawSmash");
                testAttack = "smash";
            }
        }
    }

    public void decHealth(float amount)
    {
        bosshealth -= amount;
        Debug.Log(bosshealth);
    }

    private void createIDList() // randomizes the attacks
    {
        int tempVar;
        int prevVar = -1;
        for (int i = 0; i < LISTLENGTH; i++)
        {
            tempVar = Random.Range(0, 3);
            if (i != 0)
            {
                if (tempVar == prevVar)
                {
                    tempVar = Random.Range(0, 3);
                }
                prevVar = tempVar;
            }
            switch (tempVar) // populates the ID list
            {
                case 0:
                    attackIDList[i] = attackList[0];
                    break;
                case 1:
                    attackIDList[i] = attackList[1];
                    break;
                case 2:
                    attackIDList[i] = attackList[2];
                    break;
            }
        }
    }

    private void startNextAttack()
    {
        switch (attackIDList[currattackid]) // sets attack timer based on the next boss attack
        {
            case "PincerAttack":
                attacktimer = 8;
                StartCoroutine("PincerAttack");
                break;
            case "ClawSmash":
                attacktimer = 4.5f;
                StartCoroutine("ClawSmash");
                break;
            case "GooBlast":
                StartCoroutine("GooBlast");
                attacktimer = 1;
                break;
        }
    }

    private void pincerAttack() // handles actions relying on the pincer IEnumerator's timings
    {
        if (FollowedPlayer == null)
        {
            return;
        }
        if (clawfollow)
        {
            clawtarget = FollowedPlayer.transform.position + FollowedPlayer.transform.TransformDirection(0, 80, 0); // actively changes the claw to hover above the player
            Claw.transform.position = Vector3.MoveTowards(Claw.transform.position, clawtarget, clawspeed * Time.deltaTime);
        }
        if (clawgrab)
        {
            Claw.transform.position = Vector3.MoveTowards(Claw.transform.position, clawtarget - new Vector3(0, 10, 0), clawspeed * clawdropspeed * Time.deltaTime);
        }
        if (clawreturn)
        {
            Claw.transform.position = Vector3.MoveTowards(Claw.transform.position, clawtarget, clawspeed * Time.deltaTime);
            if (preGrabPlayerPosition != Vector3.zero)
            {
                if (!isGrabbed && GrabbedPlayerBody != null)
                {
                    if (dist > 1)
                    {
                        dist = Vector3.Distance(GrabbedPlayer.transform.position, preGrabPlayerPosition); // drops the player back to pre-grabbed position
                        GrabbedPlayer.transform.position = Vector3.MoveTowards(GrabbedPlayer.transform.position, preGrabPlayerPosition, 80 * Time.deltaTime);
                    }
                    else
                    {
                        GrabbedPlayerBody.playerLock = false;
                    }
                }
            }
        }
        if (isGrabbed)
        {
            GrabbedPlayer.transform.position = Claw.transform.position - FollowedPlayer.transform.TransformDirection(0, 35, 0);
            if (grabbedTimer < -2) // drops the player when the timer is up
            {
                isGrabbed = false;
            }
            grabbedTimer -= Time.deltaTime;
        }
    }

    IEnumerator PincerAttack() // handles timings for the pincer function phases
    {
        for (int i = 0; i < PlayerList.Length; i++)
        {
            if (otherClaw.FollowedPlayer != PlayerList[i]) // sets the player to be followed to a player not targetted by the other claw
            {
                FollowedPlayer = PlayerList[i];
                break;
            }
        }
        Debug.Log("Followed Player: " + FollowedPlayer);
        clawfollow = true; // allows claw to follow player

        yield return new WaitForSeconds(4);

        if (FollowedPlayer == null)
        {
            Debug.Log("Had to break");
            attacktimer = 0f;
            yield break;
        }
        bossAnim.StopPlayback();
        bossAnim.enabled = false;
        clawfollow = false;
        clawtarget = FollowedPlayer.transform.position + FollowedPlayer.transform.TransformDirection(0, 35, 0); // sets the claw's target to the player
        clawgrab = true; // allows claw to fall to player position

        yield return new WaitForSeconds(0.8f);
        if (isGrabbed)  // when the wait function is over, if the player is grabbed, the grabbed timer will start and the player will be lifted into the air
        {
            yield return new WaitForSeconds(0.2f);
            preGrabPlayerPosition = GrabbedPlayer.transform.position;
            clawtarget = FollowedPlayer.transform.transform.position + FollowedPlayer.transform.TransformDirection(0, 80, 5);
            attacktimer += 3;
            grabbedTimer = 1; // 1 instead of 3 since the grabbedTimer threshold is -2

            yield return new WaitForSeconds(3);
            dist = Vector3.Distance(GrabbedPlayer.transform.position, preGrabPlayerPosition); // marks the place to return the player
        }
        else
        {
            clawgrab = false;
            attacktimer += 1;
            yield return new WaitForSeconds(1.2f);
        }

        clawgrab = false;
        clawreturn = true; // unlocks the coresponding code to return to spawn

        clawtarget = CLAWSPAWN; // sets the claw's target to it's spawn

        yield return new WaitForSeconds(3);
        clawreturn = false; // resets final bool and timer for the grab;

        GrabbedPlayer = null;
        isGrabbed = false;
        GrabbedPlayerBody = null;
        FollowedPlayer = null;
        bossAnim.enabled = true;
    }

    IEnumerator ClawSmash()
    {
        if (Claw.name == "clawLeft") // plays the appropriate smash animation for the claw using the script
        {
            bossAnim.Play("clawSmash");
        }
        else
        {
            bossAnim.Play("clawSmashRight");
        }
        yield return new WaitForSeconds(2.05f);
        cantakedmg = true; // lets the boss take damage while the claw is on the ground
        if (gameObject.name == "clawRight")
        {
            Instantiate(ShockWave, ClawSprite.transform.position - new Vector3(8, 25f, 16), Quaternion.Euler(80, 0, 0)); // instantiates the shockwave part of attack
        }
        else
        {
            Instantiate(ShockWave, ClawSprite.transform.position - new Vector3(-8, 25f, 16), Quaternion.Euler(80, 0, 0)); // instantiates the shockwave part of attack
        }
        yield return new WaitForSeconds(1.95f);
        cantakedmg = false; // sets boss damage off when claw leaves the ground
        ClawSprite.transform.localPosition = new Vector3(0, 0, 0);
        bossAnim.Play("clawPassive");
        // damage is dealt

        yield return new WaitForSeconds(0.4f); // plays off last part of the animation
    }

    IEnumerator GooBlast()
    {
        // animation is called
        // damage is dealt
        yield return new WaitForSeconds(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (clawgrab && other.gameObject.transform.GetChild(0).GetChild(0).gameObject == FollowedPlayer)
            {
                GrabbedPlayerBody = other.GetComponent<PlayerBody>();
                GrabbedPlayer = other.gameObject;
                isGrabbed = true;
                GrabbedPlayerBody.playerLock = true;
                grabbedTimer = 1;
            }
        }
    }
}