 using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CarryState : FSMState
{

    private EnemyAIController enemy;//grabs the enemy controller


    public CarryState(EnemyAIController controller)
    {
        stateID = FSMStateID.Carry;//sets the FSM state ID to Idle
        enemy = controller;
        curSpeed = 0.0f;
        curRotSpeed = 0.0f;
    }

    //creates transition reasoning for if the enemy has no health, die, if the enemy has low health, flee, and if the enemy is in range of the player, attack
    public override void Reason(Transform player, Transform npc)
    {


        if (enemy.doneCarry == true) 
        {
            enemy.ResetCarryVar();
            
            enemy.PerformTransition(Transition.LookForPlayer);
        }
 
        
    }

    //no need for anything in act as you are jsut standing there until a player attacks you or you see them
    public override void Act(Transform player, Transform npc)
    {
        enemy.StartCarry();
    }
}
