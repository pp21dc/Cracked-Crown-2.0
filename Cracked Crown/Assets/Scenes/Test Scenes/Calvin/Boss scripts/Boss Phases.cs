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

    [SerializeField]
    Material baseMat;
    [SerializeField]
    Material whiteMat;
    [SerializeField]
    Sprite[] spriteList;

    private Vector3 preGrabPlayerPosition;
    private Vector3 clawtarget;
    private float dist;
    public string testAttack;
    private int currSpriteID = 0;
    private bool delayed = false;

    private int nextattack;
    private int prevattack;
    private string CurrentAttack;

    public bool isClawSmash = false;
    bool attackLoop = true; // controls the running of the boss's attacks

    private bool clawfollow = false; // pincer attack bools
    private bool clawgrab = false;
    public bool clawreturn = false;

    public GameObject FollowedPlayer;
    private PlayerBody GrabbedPlayerBody;

    private bool isGrabbed = false;
    private float grabbedTimer;

    // gets references for players
    [Header("Player List")]
    [SerializeField]
    private GameObject[] PlayerList;
    [SerializeField]
    private GameObject GrabbedPlayer;

    
    private void Awake()
    {
        bosshealth = MAXBOSSHEALTH;
        GameObject[] TempList = GameObject.FindGameObjectsWithTag("BossFollowPoint");
        PlayerList = new GameObject[TempList.Length];
        for (int i = 0; i < TempList.Length; i++)
        {
            PlayerList[i] = TempList[i]; // creates a list of all players in the scene
        }
        CLAWSPAWN = Claw.transform.position; // gets the original claw point
    }

    void Update()
    {
        if (gameObject.name == "clawRight")
        {
            if (!delayed)
            {
                StartCoroutine(delay(2));
                return;
            }
        }
        if (bosshealth <= 0)
        {
            if (GrabbedPlayerBody != null)
            {
                GrabbedPlayerBody.playerLock = false;
                StopAllCoroutines();
            }
            gameObject.GetComponent<BossPhases>().enabled = false;
            return;
        }
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
               startNextAttack(); // calls the creation of next attack and sets up for switch statement
           }
           switch (CurrentAttack) // checks id list
           {
               case "PincerAttack":
                   pincerAttack();
                   break;
               case "ClawSmash":
                   break;
               case "RoarAttack":
                   break;
           }
           attacktimer -= Time.deltaTime;
       }
    }

    public string displayAttack()
    {
        return CurrentAttack;
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
        StartCoroutine(FlashWhite(ClawSprite.GetComponent<SpriteRenderer>()));
        if (bosshealth > 60)
        {
            currSpriteID = 0;
        }
        else if (bosshealth > 45)
        {
            currSpriteID = 1;
        }
        else if (bosshealth > 30)
        {
            currSpriteID = 2;
        }
        else if (bosshealth > 15)
        {
            currSpriteID = 3;
        }
        else
        {
            currSpriteID = 4;
        }
        // ClawSprite.GetComponent<SpriteRenderer>().sprite = spriteList[currSpriteID];
    }

    public IEnumerator delay(int time)
    {
        yield return new WaitForSeconds(time);
        delayed = true;
    }

    public IEnumerator FlashWhite(SpriteRenderer s)
    {
        s.material = whiteMat;

        yield return new WaitForSeconds(0.15f);

        s.material = baseMat;

    }

    private string createNextAttack() // randomizes the attacks
    {
        nextattack = Random.Range(0, 3);

        if (nextattack == prevattack)
        {
            nextattack = Random.Range(0, 3);
        }

        prevattack = nextattack;
        Debug.Log(gameObject + " " + attackList[nextattack]);
        return attackList[nextattack];
    }

    private void startNextAttack()
    {
        CurrentAttack = createNextAttack();
        switch (CurrentAttack) // sets attack timer based on the next boss attack
        {
            case "PincerAttack":
                attacktimer = 8;
                Debug.Log("1");
                StartCoroutine("PincerAttack");
                break;
            case "ClawSmash":
                attacktimer = 4.5f;
                StartCoroutine("ClawSmash");
                Debug.Log("2");
                break;
            case "RoarAttack":
                StartCoroutine("RoarAttack");
                Debug.Log("3");
                attacktimer = 1;
                break;
        }
    }

    private void chooseFollow()
    {
        float[] playerdist = new float[PlayerList.Length];
        int selection = 0;
        for (int i = 0; i < PlayerList.Length; i++)
        {
            playerdist[i] = Vector3.Distance(gameObject.transform.position, PlayerList[i].transform.position);
        }
        for (int i = 0; i < PlayerList.Length; i++)
        {
            if (otherClaw.FollowedPlayer != PlayerList[i]) // sets the player to be followed to a player not targetted by the other claw
            {
                if ((i - 1) >= 0)
                {
                    if (playerdist[selection] > playerdist[i])
                    {
                        selection = i;
                    }
                }
                else
                {
                    selection = i;
                }
            }
        }
        FollowedPlayer = PlayerList[selection];
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
        chooseFollow();
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
        isClawSmash = true;
        ClawSprite.transform.localPosition = Vector3.zero;
        if (Claw.name == "clawLeft") // plays the appropriate smash animation for the claw using the script
        {
            bossAnim.Play("clawSmash");
        }
        else
        {
            bossAnim.Play("clawSmashRight");
        }
        yield return new WaitForSeconds(2.05f);
        isClawSmash = false;
        if (gameObject.name == "clawRight")
        {
            Instantiate(ShockWave, ClawSprite.transform.position - new Vector3(8, 25f, 16), Quaternion.Euler(80, 0, 0)); // instantiates the shockwave part of attack
        }
        else
        {
            Instantiate(ShockWave, ClawSprite.transform.position - new Vector3(-8, 25f, 16), Quaternion.Euler(80, 0, 0)); // instantiates the shockwave part of attack
        }
        yield return new WaitForSeconds(1.95f);
        ClawSprite.transform.localPosition = new Vector3(0, 0, 0);
        bossAnim.Play("clawPassive");
        // damage is dealt

        yield return new WaitForSeconds(0.4f); // plays off last part of the animation
    }

    IEnumerator RoarAttack()
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
                GrabbedPlayerBody.DecHealth(8);
                GrabbedPlayer = other.gameObject;
                isGrabbed = true;
                GrabbedPlayerBody.playerLock = true;
                grabbedTimer = 1;
            }
        }
    }

    IEnumerator bossEntry()
    {
        if (gameObject.name == "clawLeft")
        {
            bossAnim.Play("enterLeft");
        }
        else
        {
            bossAnim.Play("enter");
        }
        yield return new WaitForSeconds(2);
    }
}