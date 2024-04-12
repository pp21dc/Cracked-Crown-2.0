using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SlamGroundState : FSMState
{

    private EnemyAIController enemy;//grabs the enemy controller

    public SlamGroundState(EnemyAIController controller)
    {
        stateID = FSMStateID.SlamGround;//sets the FSM state ID to Idle
        enemy = controller;
        curSpeed = 0.0f;
        curRotSpeed = 0.0f;
    }

    //creates transition reasoning for if the enemy has no health, die, if the enemy has low health, flee, and if the enemy is in range of the player, attack
    public override void Reason(Transform player, Transform npc)
    {



       
        if (enemy.Health <= 0)
        {
            enemy.PerformTransition(Transition.NoHealth);
            return;
        }
        else if (enemy.Health <= enemy.maxHealth * 0.45f)
        {

            enemy.PerformTransition(Transition.LowHealth);
            return;

        }
        else if (enemy.moveToCarry )
        {
            
            enemy.ResetSlamVar();
            enemy.doneCarry = false;
            enemy.PerformTransition(Transition.SlamSuceeded);
        }
        else if (enemy.moveToStunned) 
        {
            enemy.ResetSlamVar();
            enemy.PerformTransition(Transition.SlamFailed);
        }
        
    }

    //no need for anything in act as you are jsut standing there until a player attacks you or you see them
    public override void Act(Transform player, Transform npc)
    {
        enemy.StartSlam();
    }
}
