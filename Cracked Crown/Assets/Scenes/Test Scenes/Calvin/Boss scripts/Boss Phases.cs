using UnityEngine;
using System;
using System.Collections;
using System.Collections.Specialized;

public class BossPhases : MonoBehaviour
{
    private static float MAXBOSSHEALTH;
    private static Vector3 LEFTCLAWSPAWN;

    [SerializeField] // list for the possible boss attacks
    string[] attackList = new string[3] {"Pincer Attack", "Claw Slam", "Goo Blast"};
    

    [Header("Boss Parts")]
    [SerializeField]
    private GameObject BossObject;
    [SerializeField]
    private GameObject clawLeft;
    [SerializeField]
    private GameObject clawRight;

    [Header("Boss Variables")]
    [SerializeField]
    private float clawspeed;
    [SerializeField] // count used to allow the boss to know when to initiate attacks
    private float attacktimer;

    private Vector3 preGrabPlayerPosition;
    private Vector3 clawtarget;
    bool attackLoop = true;
    private bool clawfollow = false;
    private bool clawgrab = false;
    private bool clawreturn = false;

    private bool isGrabbed = false;
    private float grabbedTimer;

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
        }
        LEFTCLAWSPAWN = clawLeft.transform.position;
    }

    void Update()
    {
        if (attackLoop)
        {
            // attacks called in a continuous loop
            if (attacktimer <= 0)
            {
                StartCoroutine("PincerAttack");
                attacktimer = 30;
            }
            if (attacktimer > 18)
            {
                pincerAttack();
            }
            attacktimer -= Time.deltaTime;
        }
    }

    IEnumerator AttackCycle() // the loop that calls boss attacks based on attacktimer
    {
        StartCoroutine("PincerAttack");
        yield return new WaitForSeconds(11);
        StartCoroutine("ClawSweep");
        yield return new WaitForSeconds(10);
        StartCoroutine("GooBlast");
        yield return new WaitForSeconds(10);
    }

    private void pincerAttack() // handles actions relying on the pincer IEnumerator's timings
    {
        if (clawfollow)
        {
            clawtarget = PlayerList[0].transform.position + PlayerList[0].transform.TransformDirection(0, 80, 0); // actively changes the claw to hover above the player
            clawLeft.transform.position = Vector3.MoveTowards(clawLeft.transform.position, clawtarget, clawspeed * Time.deltaTime);
        }
        if (clawgrab)
        {
            clawLeft.transform.position = Vector3.MoveTowards(clawLeft.transform.position, clawtarget, clawspeed* 2f * Time.deltaTime);
        }
        if (clawreturn)
        {
            clawLeft.transform.position = Vector3.MoveTowards(clawLeft.transform.position, clawtarget, clawspeed * Time.deltaTime);
            if (preGrabPlayerPosition != Vector3.zero)
            {
                if (!isGrabbed)
                {
                    GrabbedPlayer.transform.position = Vector3.MoveTowards(GrabbedPlayer.transform.position, preGrabPlayerPosition, 40 * Time.deltaTime);
                }
            }
        }
        if (isGrabbed)
        {
            GrabbedPlayer.transform.position = clawLeft.transform.position;

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

        yield return new WaitForSeconds(4);

        clawfollow = false;
        clawtarget = PlayerList[0].transform.position + PlayerList[0].transform.TransformDirection(0, 35, 0); // sets the claw's target to the player
        clawgrab = true; // allows claw to fall to player position

        yield return new WaitForSeconds(1);
        
        if (isGrabbed)  // when the wait function is over, if the player is grabbed, the grabbed timer will start and the player will be lifted into the air
        {
            preGrabPlayerPosition = GrabbedPlayer.transform.position;
            clawgrab = false;

            clawtarget = PlayerList[0].transform.transform.position + PlayerList[0].transform.TransformDirection(0, 80, 0);
            grabbedTimer = 1; // 1 instead of 3 since the grabbedTimer threshold is -2
            
            yield return new WaitForSeconds(3);
        }
        clawreturn = true; // sends the claw back to spawn
        clawtarget = LEFTCLAWSPAWN;

        yield return new WaitForSeconds(3);
    }

    IEnumerator ClawSweep()
    {
        // animation is called
        gameObject.transform.position = new Vector3(0, 0, 0);
        // damage is dealt
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
            GrabbedPlayer = other.gameObject.transform.root.gameObject;
            isGrabbed = true;
            // player takes damage and movement is frozen
        }
    }
}