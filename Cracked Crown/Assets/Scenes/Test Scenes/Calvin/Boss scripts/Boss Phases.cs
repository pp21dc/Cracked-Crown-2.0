using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhases : MonoBehaviour
{
    [SerializeField] // list for the possible boss attacks
    string[] attackList = new string[3] {"Pincer Attack", "Claw Sweep", "Goo Blast"};
    bool attackLoop = true;

    // gets references for players
    private GameObject[] PlayerList = new GameObject[4];

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
            //attacks called in a continuous loop
        }
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
