using UnityEngine;
using System.Collections;

public class BossPhases : MonoBehaviour
{
    private static float MAXBOSSHEALTH;
    [SerializeField]
    private Vector3 CLAWSPAWN;

    [SerializeField] // list for the possible boss attacks
    string[] attackList = new string[3] {"Pincer Attack", "Claw Slam", "Goo Blast"};
    

    [Header("Boss Parts")]
    [SerializeField]
    private GameObject Claw;

    [SerializeField]
    private Animator bossAnim;


    [SerializeField]
    private BossPhases otherClaw;

    [Header("Boss Variables")]
    [SerializeField]
    private float clawspeed;
    [SerializeField] // count used to allow the boss to know when to initiate attacks
    private float attacktimer;

    private Vector3 preGrabPlayerPosition;
    private Vector3 clawtarget;

    float dist;
    bool attackLoop = true;
    private bool clawfollow = false;
    private bool clawgrab = false;
    private bool clawreturn = false;
    private GameObject followedPlayer;

    private bool isGrabbed = false;
    private float grabbedTimer;
    [SerializeField]
    private string[] attackIDList = new string[30];

    // gets references for players
    private GameObject[] PlayerList = new GameObject[4];
    private GameObject GrabbedPlayer;

    private float bossHealth;

    
    private void Awake()
    {
        GameObject[] TempList = GameObject.FindGameObjectsWithTag("BossFollowPoint");
        for (int i = 0; i < TempList.Length; i++)
        {
            PlayerList[i] = TempList[i]; // creates a list of all players in the scene
            Debug.Log(PlayerList[i]);
        }
        CLAWSPAWN = Claw.transform.position;
        createIDList();
    }

    public GameObject getfollowedplayer()
    {
        return followedPlayer;
    }

    void Update()
    {
        if (attackLoop)
        {
            // attacks called in a continuous loop
            if (1==1)//attacktimer = 0)  Caused an error in unity!!!!!
            {
                readNextID(1);
            }
            attacktimer -= Time.deltaTime;
        }
    }

    private void createIDList() // randomizes the attacks
    {
        int tempVar;
        int prevVar = -1;
        for (int i = 0; i < 30; i++)
        {
            tempVar = Random.Range(0, 4);
            if (i != 0)
            {
                if (tempVar == prevVar)
                {
                    tempVar = Random.Range(0, 4);
                    prevVar = tempVar;
                }
            }
            switch (tempVar) // populates the ID list
            {
                case 0:
                    attackIDList[i] = "pincerAttack";
                    break;
                case 1:
                    attackIDList[i] = "clawSlam";
                    break;
                case 2:
                    attackIDList[i] = "gooBlast";
                    break;
            }

        }
    }

    private string readNextID(int ID)
    {
        return attackIDList[ID];
    }

    private void pincerAttack() // handles actions relying on the pincer IEnumerator's timings
    {
        if (clawfollow)
        {
            clawtarget = PlayerList[0].transform.position + PlayerList[0].transform.TransformDirection(0, 80, 0); // actively changes the claw to hover above the player
            Claw.transform.position = Vector3.MoveTowards(Claw.transform.position, clawtarget, clawspeed * Time.deltaTime);
        }
        if (clawgrab)
        {
            Claw.transform.position = Vector3.MoveTowards(Claw.transform.position, clawtarget, clawspeed* 2f * Time.deltaTime);
        }
        if (clawreturn)
        {
            Claw.transform.position = Vector3.MoveTowards(Claw.transform.position, clawtarget, clawspeed * Time.deltaTime);
            if (preGrabPlayerPosition != Vector3.zero)
            {
                if (!isGrabbed)
                {

                    Debug.Log(dist);
                    if (dist > 1)
                    {
                        dist = Vector3.Distance(GrabbedPlayer.transform.position, preGrabPlayerPosition);
                        GrabbedPlayer.transform.position = Vector3.MoveTowards(GrabbedPlayer.transform.position, preGrabPlayerPosition, 80 * Time.deltaTime);
                    }
                }
            }
        }
        if (isGrabbed)
        {
            GrabbedPlayer.transform.position = Claw.transform.position - PlayerList[0].transform.TransformDirection(0, 35, 0);

            if (grabbedTimer < -2) // drops the player when the timer is up
            {
                Debug.Log("Done");
                isGrabbed = false;
            }
            grabbedTimer -= Time.deltaTime;
        }
    }

    IEnumerator PincerAttack() // handles timings for the pincer function phases
    {
        clawfollow = true; // allows claw to follow player
        Debug.Log("Follow");
        for (int i = 0; i < PlayerList.Length; i++)
        {
            if (otherClaw.followedPlayer != PlayerList[i])
            {
                followedPlayer = PlayerList[i];
            }
        }

        yield return new WaitForSeconds(4);

        clawfollow = false;
        clawtarget = PlayerList[0].transform.position + PlayerList[0].transform.TransformDirection(0, 35, 0); // sets the claw's target to the player
        clawgrab = true; // allows claw to fall to player position

        yield return new WaitForSeconds(1);
        
        if (isGrabbed)  // when the wait function is over, if the player is grabbed, the grabbed timer will start and the player will be lifted into the air
        {
            preGrabPlayerPosition = GrabbedPlayer.transform.position;
            clawtarget = PlayerList[0].transform.transform.position + PlayerList[0].transform.TransformDirection(0, 80, 0);
            grabbedTimer = 1; // 1 instead of 3 since the grabbedTimer threshold is -2
            
            yield return new WaitForSeconds(3);
            clawgrab = false;

            dist = Vector3.Distance(GrabbedPlayer.transform.position, preGrabPlayerPosition);
        }
        clawreturn = true; // sends the claw back to spawn
        clawtarget = CLAWSPAWN;

        yield return new WaitForSeconds(3);
    }

    IEnumerator ClawSmash()
    {
        
        yield return new WaitForSeconds(3);
        // animation is called
        // damage is dealt
        yield return new WaitForSeconds(1.2f);

        yield return new WaitForSeconds(3);
    }

    IEnumerator GooBlast()
    {
        // animation is called
        // damage is dealt
        yield return new WaitForSeconds(3);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("hit");
        if (other.gameObject.tag == "Player")
        {
            GrabbedPlayer = other.gameObject;
            isGrabbed = true;
            // player takes damage and movement is frozen
        }
    }
}