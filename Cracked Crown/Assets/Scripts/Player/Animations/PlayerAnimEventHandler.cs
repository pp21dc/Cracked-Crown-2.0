using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimEventHandler : MonoBehaviour
{
    public PlayerContainer PC;

    private void Awake()
    {
        PC = transform.parent.parent.parent.parent.GetComponent<PlayerContainer>();
    }

    public void AttackFinish()
    {
        //Debug.Log("FINISH");
        PC.PAC.Attacking = false;
        PC.PB.canMove = true;
        PC.PB.canAttack = true;
    }

    public void DeathFinish()
    {
        PC.PB.RESETINGGHOST += 1;
        if (PC.PB.RESETINGGHOST == 2)
        {
            //PC.PB.RESETINGGHOST += 1;
            PC.PB.GhostMode();
        }
        else if (PC.PB.RESETINGGHOST == 5)
            PC.PB.resetPlayer();

        Debug.Log(PC.PB.RESETINGGHOST);

    }
}
