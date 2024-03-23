using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class ShockwaveState : FSMState
{

    private EnemyAIController enemy;//grabs the enemy controller

    public ShockwaveState(EnemyAIController controller)
    {
        stateID = FSMStateID.Shockwave;//sets the FSM state ID to Idle
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
            return;
            
        }
        if (enemy.Health <= 0)
        {
            enemy.PerformTransition(Transition.NoHealth);
        }
        else if (enemy.doneShockwave == true)
        {

            enemy.ResetShockVar();

            if (Vector3.Distance(enemy.ePosition.position, player.position) <= 55f && enemy.shootOnCD == false)
            {
                
                enemy.PerformTransition(Transition.InShootingRange);
                return;
            }
            else
            {
                
                enemy.PerformTransition(Transition.LookForPlayer);
                return;
            }
        }


    }

    //no need for anything in act as you are jsut standing there until a player attacks you or you see them
    public override void Act(Transform player, Transform npc)
    {
        enemy.StartShockwave();
    }
}
