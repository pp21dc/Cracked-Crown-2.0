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
    private BossAudioManager BAM;
    [SerializeField]
    private GameObject Claw;
    [SerializeField]
    private Animator bossAnim;
    [SerializeField]
    private Animator mandibleAnim;
    [SerializeField]
    private BossPhases otherClaw;
    [SerializeField]
    private GameObject ClawSprite;
    [SerializeField]
    private GameObject DmgOverlay;

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
    
    
    [Header("Materials and Sprites")]
    [SerializeField]
    Material baseMat;
    [SerializeField]
    Material whiteMat;
    [SerializeField]
    Sprite[] spriteList;

    private Vector3 clawtarget;
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
    public bool roarSpawn = false;
    private bool clawdead = false;

    [SerializeField]
    private CameraShake cameraShake;
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
        CLAWSPAWN = Claw.transform.position; // gets the original claw point
    }


    private void Start()
    {
        GameObject[] TempList = GameObject.FindGameObjectsWithTag("BossFollowPoint");
        PlayerList = new GameObject[TempList.Length];
        for (int i = 0; i < TempList.Length; i++)
        {
            PlayerList[i] = TempList[i]; // creates a list of all players in the scene
        }
        bossAnim.Play("clawPassive");
        cameraShake = FindObjectOfType<CameraShake>();
        GameManager.Instance.claws.Add(this);
    }

    void Update()
    {
        if (otherClaw.isDead() && isDead()) //this var is what ever one you use to tell if boss is dead
        {
            //GameManager.Instance.win = true;
        }

        if (bosshealth <= 0)
        {
            clawdead = true;
            StopAllCoroutines();
            bossAnim.Play("leave");
            return;
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
        
        /*if (roarSpawn)
        {
            
            LevelManager.Instance.SpawnersActive = true;
        }
        else
        {
            LevelManager.Instance.SpawnersActive = false;
        }*/

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

    public bool isDead()
    {
        return clawdead;
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
        StartCoroutine(FlashRed(DmgOverlay.GetComponent<SpriteRenderer>()));
        if (bosshealth / MAXBOSSHEALTH * 100 > 75)
        {
            currSpriteID = 0;

        }
        else if (bosshealth / MAXBOSSHEALTH * 100 > 50)
        {
            currSpriteID = 1;   
        }
        else if (bosshealth / MAXBOSSHEALTH * 100 > 25)
        {
            currSpriteID = 2;
        }
        else if (bosshealth / MAXBOSSHEALTH * 100 > 0)
        {
            currSpriteID = 3;
        }
        DmgOverlay.GetComponent<SpriteRenderer>().sprite = spriteList[currSpriteID];
    }

    public IEnumerator delay(int time)
    {
        yield return new WaitForSeconds(time);
    }

    public IEnumerator FlashRed(SpriteRenderer s)
    {
        DmgOverlay.GetComponent<SpriteRenderer>().color = Color.red;

        yield return new WaitForSeconds(0.2f);

        DmgOverlay.GetComponent<SpriteRenderer>().color = Color.white;

    }

    private string createNextAttack() // randomizes the attacks
    {
        nextattack = Random.Range(0, 3);

        if (nextattack == prevattack)
        {
            nextattack = Random.Range(0, 3);
        }
        if (otherClaw.nextattack == 2)
        {
            if (nextattack == 2)
            {
                return createNextAttack();
            }
        }
        prevattack = nextattack;
        return attackList[nextattack];
    }

    private void startNextAttack()
    {
        CurrentAttack = createNextAttack();
        if (CurrentAttack == "RoarAttack")
        {
            if (Random.Range(0, 2) == 0)
            {
                CurrentAttack = createNextAttack();
            }

        }
        switch (CurrentAttack) // sets attack timer based on the next boss attack
        {
            case "PincerAttack":
                attacktimer = 8;
                StartCoroutine("PincerAttack");
                break;
            case "ClawSmash":
                attacktimer = 4.5f;
                StartCoroutine("ClawSmash");
                break;
            case "RoarAttack":
                StartCoroutine("RoarAttack");
                attacktimer = 3;
                break;
        }
    }

    private GameObject chooseFollow()
    {
        if (PlayerList.Length == 1)
        {
            if (otherClaw.FollowedPlayer != null)
            {
                return null;
            }
        }


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
        if (otherClaw.FollowedPlayer == PlayerList[selection])
        {
             for (int i = 0; i < PlayerList.Length; i++)
            {
                if (PlayerList[selection] != PlayerList[i])
                {
                    return PlayerList[i];
                }
                else
                {
                    return null;
                }
            }
            return null;
        }
        else
        {
            return PlayerList[selection];
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
            if (!isGrabbed && GrabbedPlayerBody != null)
            {
                GrabbedPlayerBody.playerLock = false;
            }
        }
        if (isGrabbed)
        {
            if (Claw.name == "clawLeft")
            {
                GrabbedPlayer.transform.position = Claw.transform.position - FollowedPlayer.transform.TransformDirection(-10, 25, 0);
            }
            else
            {
                GrabbedPlayer.transform.position = Claw.transform.position - FollowedPlayer.transform.TransformDirection(10, 25, 0);
            }
            if (grabbedTimer < -2) // drops the player when the timer is up
            {
                isGrabbed = false;
            }
            grabbedTimer -= Time.deltaTime;
        }
    }

    IEnumerator PincerAttack() // handles timings for the pincer function phases
    {
        FollowedPlayer = chooseFollow();

        if (FollowedPlayer == null)
        {
            attacktimer = 0f;
            yield break;
        }

        clawfollow = true; // allows claw to follow player

        yield return new WaitForSeconds(4);

        if (FollowedPlayer == null)
        {
            attacktimer = 0f;
            yield break;
        }

        bossAnim.StopPlayback();
        if (Claw.name == "clawLeft") // plays the appropriate smash animation for the claw using the script
        {
            ClawSprite.transform.localPosition = new Vector3(0.5f, 0, 0);
        }
        else
        {
            ClawSprite.transform.localPosition = new Vector3(-0.5f, 0, 0);
        }

        clawfollow = false;
        clawtarget = FollowedPlayer.transform.position + FollowedPlayer.transform.TransformDirection(0, 35, 5); // sets the claw's target to the player
        clawgrab = true; // allows claw to fall to player position

        yield return new WaitForSeconds(0.25f);
        if (Claw.name == "clawLeft")
        {
            bossAnim.Play("grabClipLeft");
        }
        else
        {
            bossAnim.Play("grabClipRight");
        }
        yield return new WaitForSeconds(0.55f);
        if (isGrabbed)  // when the wait function is over, if the player is grabbed, the grabbed timer will start and the player will be lifted into the air
        {
            yield return new WaitForSeconds(0.2f);
            clawtarget = FollowedPlayer.transform.transform.position + FollowedPlayer.transform.TransformDirection(0, 80, 5);
            attacktimer += 3;
            grabbedTimer = 1; // 1 instead of 3 since the grabbedTimer threshold is -2

            yield return new WaitForSeconds(3);

            if (Claw.name == "clawLeft")
            {
                bossAnim.Play("dropLeft");
            }
            else
            {
                bossAnim.Play("dropRight");
            }
        }
        else
        {
            clawgrab = false;
            attacktimer += 1;
            yield return new WaitForSeconds(1);
            if (Claw.name == "clawLeft")
            {
                bossAnim.Play("dropLeft");
            }
            else
            {
                bossAnim.Play("dropRight");
            }
            yield return new WaitForSeconds(0.2f);
        }

        clawgrab = false;

        clawreturn = true; // unlocks the coresponding code to return to spawn

        clawtarget = CLAWSPAWN; // sets the claw's target to it's spawn

        bossAnim.Play("clawPassive");

        yield return new WaitForSeconds(3);

        clawreturn = false; // resets final bool and timer for the grab;
        GrabbedPlayer = null;
        isGrabbed = false;
        GrabbedPlayerBody = null;
        FollowedPlayer = null;

    }

    IEnumerator ClawSmash()
    {
        isClawSmash = true;

        if (Claw.name == "clawLeft") // plays the appropriate smash animation for the claw using the script
        {
            ClawSprite.transform.localPosition = new Vector3(0.5f, 0, 0);
            bossAnim.Play("clawSmash");
        }
        else
        {
            ClawSprite.transform.localPosition = new Vector3(-0.5f, 0, 0);
            bossAnim.Play("clawSmashRight");
        }
        yield return new WaitForSeconds(2.05f);
        isClawSmash = false;
        if (gameObject.name == "clawRight")
        {
            BAM.PlayAudio(BossAudioManager.AudioType.Slam);
            Instantiate(ShockWave, ClawSprite.transform.position - new Vector3(8, 5f, 16), Quaternion.Euler(0, 0, 0)); // instantiates the shockwave part of attack (RIGHT)
        }
        else
        {
            BAM.PlayAudio(BossAudioManager.AudioType.Slam);
            Instantiate(ShockWave, ClawSprite.transform.position - new Vector3(-8, 5f, 16), Quaternion.Euler(0, 0, 0)); // instantiates the shockwave part of attack (LEFT)
        }
        cameraShake.StartCoroutine(cameraShake.Shake(0.15f, 2f));
        yield return new WaitForSeconds(1.95f);
        ClawSprite.transform.localPosition = new Vector3(0, 0, 0);
        bossAnim.Play("clawPassive");
        // damage is dealt

        yield return new WaitForSeconds(0.4f); // plays off last part of the animation
    }

    IEnumerator RoarAttack()
    {
        // animation is called
        mandibleAnim.StopPlayback();
        mandibleAnim.Play("mandibles");
        BAM.PlayAudio(BossAudioManager.AudioType.Roar);
        LevelManager.Instance.SpawnersActive = true;
        cameraShake.StartCoroutine(cameraShake.Shake(2f, 0.4f));
        roarSpawn = true;
        yield return new WaitForSeconds(6f);
        roarSpawn = false;
        LevelManager.Instance.SpawnersActive = false;
        yield return new WaitForSeconds(0.5f);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (clawgrab && other.gameObject.transform.GetChild(0).GetChild(0).gameObject == FollowedPlayer)
            {
                GrabbedPlayerBody = other.GetComponent<PlayerBody>();
                GrabbedPlayerBody.DecHealth(4);
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