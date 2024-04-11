using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eyeMovement : MonoBehaviour
{
    [SerializeField]
    private GameObject claw;

    void Update()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.isLoading) // stops boss script while the level is loading
            {
                return;
            }
        }
        if (!claw.GetComponent<BossPhases>().sendToWin)
        {

        }
    }
}
