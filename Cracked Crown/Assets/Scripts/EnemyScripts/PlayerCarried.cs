using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerCarried : MonoBehaviour
{
    [SerializeField]
    private PlayerBody body;

    [SerializeField]
    private PlayerController controller;

    [SerializeField]
    private GameObject player;

    public EnemyAIController enemyAIController;
    public bool timeToCarry;

    public bool canRelease;



    private void Update()
    {
        if(timeToCarry)
        {
            player.SetActive(false);

            if(StartSpam())
            {
                canRelease = true;
            }
        }
    }

    public bool StartSpam()
    {
        body.canMove = false;
        body.canAttack = false;

        StartCoroutine(spamX());

        if (canRelease == true)
        {
            canRelease = false;
            return true;
        }
        else
        {
            return false;
        }

    }

    IEnumerator spamX()
    {
        int timesHit = 0;

        while (timesHit < 8)
        {
            if (controller.InteractDown)
            {
                timesHit++;
            }
        }

        canRelease = true;
        body.canMove = true;
        body.canAttack = true;

        yield return null;
    }

}
