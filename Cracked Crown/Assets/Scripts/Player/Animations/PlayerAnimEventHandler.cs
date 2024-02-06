using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        //Debug.Log("FINISH-ATTACK");
        PC.PAC.Attacking = false;
        PC.PB.canMove = true;
        PC.PB.canAttack = true;
    }

    public void FinishFinish()
    {
        PC.PAC.Attacking = false;
        PC.PAC.Finishing = false;
        PC.PB.canMove = true;
        PC.PB.canAttack = true;
        PC.PB.AddHealth(PC.PB.executeHeal);
        PC.PB.canMove = true;
        PC.PB.canTakeDamage = true;
        PC.PB.canMovePlayerForexecute = false;
        PC.PB.lockExecAnim = false;
    }

    public void DeathFinish()
    {
        PC.PB.RESETINGGHOST += 1;
        if (PC.PB.RESETINGGHOST == 2)
        {
            //PC.PB.RESETINGGHOST += 1;
            PC.PB.GhostMode();
        }
        else if (PC.PB.RESETINGGHOST >= 5)
            PC.PB.RevivePlayer();

        Debug.Log("RHOST: " + PC.PB.RESETINGGHOST);

    }
}
