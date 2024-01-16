using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhases : MonoBehaviour
{
    private static float MAXBOSSHEALTH;

    [SerializeField] // list for the possible boss attacks
    string[] attackList = new string[3] {"Pincer Attack", "Claw Slam", "Goo Blast"};
    bool attackLoop = true;
    [SerializeField]
    GameObject clawLeft;
    [SerializeField]
    GameObject clawRight;

    // gets references for players
    private GameObject[] PlayerList = new GameObject[4];

    private float bossHealth;

    [SerializeField]
    GameObject BossObject;
    private void Awake()
    {
        GameObject[] TempList = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < TempList.Length; i++)
        {
            PlayerList[i] = TempList[i];
            Debug.Log(PlayerList[i]);
        }
    }

    void Update()
    {
        if (attackLoop)
        {
            
            clawLeft.transform.position = PlayerList[0].transform.position + transform.TransformDirection(100,0,0);
            // attacks called in a continuous loop
        }
    }

    IEnumerator AttackCycle() // the loop that calls boss attacks throughout the fight
    {
        StartCoroutine("PincerAttack");
        yield return new WaitForSeconds(3);
        StartCoroutine("ClawAttack");
        yield return new WaitForSeconds(3);
        StartCoroutine("GooBlast");
        yield return new WaitForSeconds(3);
    }

    IEnumerator PincerAttack()
    {
        // animation is called
        // damage is dealt
        yield return new WaitForSeconds(3);
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
}
