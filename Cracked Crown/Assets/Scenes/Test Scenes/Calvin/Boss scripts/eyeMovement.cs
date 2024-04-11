using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eyeMovement : MonoBehaviour
{
    [SerializeField]
    private BossPhases claw;
    [SerializeField]
    private Animator anim;
    bool running = false;
    void Update()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.isLoading) // stops boss script while the level is loading
            {
                return;
            }
        }
        if (!claw.sendToWin && !running)
        {
            StartCoroutine(idle());
        }
        else if (claw.sendToWin && !running)
        {
            running = true;
            anim.Play("dying");
        }
    }

    IEnumerator idle ()
    {
        running = true;
        yield return new WaitForSeconds(5);
        int coin = Random.Range(0, 1);
        Debug.Log(coin);
        if (coin == 0)
        {
            anim.Play("IdleBlinking");
        }
        if (coin == 1)
        {
            anim.Play("IdleBlinkingRight");
        }
        yield return new WaitForSeconds(1);
        running = false;
    }
}
