using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class RushState : FSMState
{

    private EnemyAIController enemy;

    public RushState(EnemyAIController controller)
    {
        stateID = FSMStateID.Rush;
        enemy = controller;
        curSpeed = 8.5f;
        curRotSpeed = 0.0f;
    }

    public override void Reason(Transform player, Transform npc)
    {

    }

    //Act
    public override void Act(Transform player, Transform npc)
    {
        
    }
}
