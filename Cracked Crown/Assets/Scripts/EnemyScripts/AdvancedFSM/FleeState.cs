using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FleeState : FSMState
{

    private EnemyAIController enemy; //grabs the enemy controller

    

    public FleeState(EnemyAIController controller)
    {
        stateID = FSMStateID.Flee;//sets the state ID to flee
        enemy = controller;
        curSpeed = 7.0f;
        curRotSpeed = 0.0f;
    }

    //only one transition out of fleeing and that is to die if there is no health
    public override void Reason(Transform player, Transform npc)
    {

        if (enemy.Health <= 0)
        {
            enemy.PerformTransition(Transition.NoHealth);
            return;
        }

    }

    //the code here just makes the enemy look at the player, then look in the opposite direction and run like hell outta there
    public override void Act(Transform player, Transform npc)
    {
        npc.LookAt(player);
        npc.transform.Rotate(0, 180, 0);
        npc.Translate(Vector3.forward * curSpeed * Time.deltaTime);

        Vector3 pos = npc.position;
        pos.y = 0f;
        npc.position = pos;

        enemy.StartFleeAnimation();

    }
}
