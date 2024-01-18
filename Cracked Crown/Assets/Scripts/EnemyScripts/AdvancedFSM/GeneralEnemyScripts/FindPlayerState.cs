using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FindPlayerState : FSMState
{

    private EnemyAIController enemy;//grabs the enemy controller

    public FindPlayerState(EnemyAIController controller)
    {
        stateID = FSMStateID.FindPlayer;//sets the FSM state ID to Idle
        enemy = controller;
        curSpeed = 0.0f;
        curRotSpeed = 0.0f;
    }

    //creates transition reasoning for if the enemy has no health, die, if the enemy has low health, flee, and if the enemy is in range of the player, attack
    public override void Reason(Transform player, Transform npc)
    {

        if (enemy.Health <= 20 && enemy.Health >= 1) 
        {
            enemy.PerformTransition(Transition.LowHealth);
        }
        else if (enemy.Health <= 0)
        {
            enemy.PerformTransition(Transition.NoHealth);
        }
        else if (enemy.CompareTag("Light"))
        {
            if (0 == 1)
            {
                enemy.PerformTransition(Transition.AbovePlayer);
                enemy.isfollowing = false;
                return;
            }
        }
        else if (enemy.CompareTag("Medium"))
        {
            if (IsInCurrentRange(npc, player.position, 15f))
            {
                enemy.PerformTransition(Transition.InFirstRange);
            }
            else if (IsInCurrentRange(npc, player.position, 7.5f))
            {
                enemy.PerformTransition(Transition.InSecondRange);
            }
        }
        else if (enemy.CompareTag("Heavy"))
        {
            if (IsInCurrentRange(npc, player.position, 15f))
            {
                enemy.PerformTransition(Transition.InShootingRange);
            }
            else if (IsInCurrentRange(npc, player.position, 4.5f))
            {
                enemy.PerformTransition(Transition.InShockwaveRange);
            }
        }
        
    }

    //no need for anything in act as you are jsut standing there until a player attacks you or you see them
    public override void Act(Transform player, Transform npc)
    {
        enemy.checkShortestDistance();
    }
}
