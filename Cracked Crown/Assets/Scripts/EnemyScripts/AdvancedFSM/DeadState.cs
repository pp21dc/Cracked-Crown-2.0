using UnityEngine;
using System.Collections;

public class DeadState : FSMState
{
    

    private EnemyAIController monster;//grabs the monster controller

    //sets the stateID to dead and sets speed and rotation to 0 so enemy does not move
    public DeadState(EnemyAIController controller)
    {
        stateID = FSMStateID.Dead;
        monster = controller;
        curSpeed = 0.0f;
        curRotSpeed = 0.0f;
    }

    //no reasoning as there is no transition out of death state
    public override void Reason(Transform player, Transform npc)
    {
    }

    //runs the enemy Death script
    public override void Act(Transform player, Transform npc)
    {
        
            monster.StartDeath();
        
    }
}
