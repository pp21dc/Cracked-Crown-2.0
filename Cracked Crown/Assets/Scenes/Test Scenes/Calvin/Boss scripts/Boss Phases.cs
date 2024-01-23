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

    private Vector3 clawtarget;
    bool attackLoop = true;
    private bool clawfollow = false;
    private bool clawgrab = false;
    private bool clawreturn = false;

    // gets references for players
    private GameObject[] PlayerList = new GameObject[4];
    private GameObject GrabbedPlayer;

    private float bossHealth;

    
    private void Awake()
    {
        GameObject[] TempList = GameObject.FindGameObjectsWithTag("BossFollowPoint");
        for (int i = 0; i < TempList.Length; i++)
        {
            PlayerList[i] = TempList[i];
            Debug.Log(i + " " + PlayerList[i]);
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
            if (attacktimer > 22)
            {
                pincerAttack();
            }
            
            attacktimer -= Time.deltaTime;
        }
    }

    IEnumerator AttackCycle() // the loop that calls boss attacks based on attacktimer
    {
        StartCoroutine("PincerAttack");
        yield return new WaitForSeconds(8);
        StartCoroutine("ClawAttack");
        yield return new WaitForSeconds(10);
        StartCoroutine("GooBlast");
        yield return new WaitForSeconds(10);
    }

    private void pincerAttack() // handles actions relying on the pincer IEnumerator
    {
        if (clawfollow)
        {
            clawtarget = PlayerList[0].transform.position + PlayerList[0].transform.TransformDirection(0, 80, 0);
            clawLeft.transform.position = Vector3.MoveTowards(clawLeft.transform.position, clawtarget, clawspeed * Time.deltaTime);
        }
        if (clawgrab)
        {
            clawLeft.transform.position = Vector3.MoveTowards(clawLeft.transform.position, clawtarget, clawspeed*1.5f * Time.deltaTime);
        }
        if (clawreturn)
        {
            clawLeft.transform.position = Vector3.MoveTowards(clawLeft.transform.position, clawtarget, clawspeed * Time.deltaTime);
        }
    }

    IEnumerator PincerAttack() // handles timing for the pincer function
    {
        clawfollow = true;
        yield return new WaitForSeconds(4);

        clawfollow = false;

        clawtarget = PlayerList[0].transform.position + PlayerList[0].transform.TransformDirection(0, 35, 0);
         
        clawgrab = true;

        // damage is dealt
        yield return new WaitForSeconds(2);
        clawgrab = false;
        clawreturn = true;
        clawtarget = LEFTCLAWSPAWN;
    }

    IEnumerator ClawSweep()
    {
        // animation is called
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
            GrabbedPlayer = other.gameObject;
            GrabbedPlayer.transform.position = clawLeft.transform.position;
            // player takes damage and movement is frozen
        }
    }
}