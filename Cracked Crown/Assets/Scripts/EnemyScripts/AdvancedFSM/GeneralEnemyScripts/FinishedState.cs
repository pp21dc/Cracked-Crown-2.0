using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FinishedState : FSMState
{

    private EnemyAIController enemy;//grabs the enemy controller

    public FinishedState(EnemyAIController controller)
    {
        stateID = FSMStateID.Finished;//sets the FSM state ID to Idle
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

        if(enemy.Health >= enemy.maxHealth * (0.75))
        {
            
            enemy.PerformTransition(Transition.HealthBack);
            return;
        }

        
    }

    //no need for anything in act as you are jsut standing there until a player attacks you or you see them
    public override void Act(Transform player, Transform npc)
    {
        enemy.StartFinish();
    }
}
