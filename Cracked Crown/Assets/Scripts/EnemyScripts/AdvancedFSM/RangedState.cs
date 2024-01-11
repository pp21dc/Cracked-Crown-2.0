using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class RangedState : FSMState
{

    private EnemyAIController enemy; //grabs the enemy Controller

    

    public RangedState(EnemyAIController controller)
    {
        stateID = FSMStateID.Ranged;//sets state ID to ranged
        enemy = controller;
        
    }

    //Creates reasoning for if the enemy has no health, die, if the enemy has low health, flee, and if the enemy is out of bullets, reload.
    public override void Reason(Transform player, Transform npc)
    {

        if (enemy.Health <= 0)
        {
            enemy.PerformTransition(Transition.NoHealth);
            return;
        }
        else if (enemy.Health <= 10)
        {

            enemy.PerformTransition(Transition.lowHealth);
            return;

        }
        else if (enemy.magSize == 0)
        {

            enemy.PerformTransition(Transition.NoBullets);
            return;
        
        }

    }

    //makes the enemy look at the player and runs the firing code on the enemy controller.
    public override void Act(Transform player, Transform npc)
    {

        npc.LookAt(player.position);
        npc.Rotate(0, -20, 0);//fixes the animation so that it faces the player again

        enemy.StartFiring();
             
    }
}
