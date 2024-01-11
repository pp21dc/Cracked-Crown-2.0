using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadState : FSMState
{

    private EnemyAIController enemy;//grabs the enemy controller

    

    public ReloadState(EnemyAIController controller)
    {
        stateID = FSMStateID.Reload;//sets the reload state id
        enemy = controller;
        curSpeed = 5.5f;
        
    }

    //Creates transtion reasoning for if there is not health to die, if low health flee, and if its in the range of an player and mag is full, attack
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
        else if (IsInCurrentRange(npc, player.position, 50f) && enemy.magSize == 8)
        {

            enemy.PerformTransition(Transition.seePlayer);
            return;

        }
        


    }

    //makes the enemy look at the player, rotate 90 degrees, and move forward, and starts the enemies reload code
    public override void Act(Transform player, Transform npc)
    {

        npc.LookAt(player.position);
        npc.Rotate(0, 90, 0);
        npc.Translate(Vector3.forward * curSpeed *Time.deltaTime);

        

        enemy.StartReloading();
    }
}
