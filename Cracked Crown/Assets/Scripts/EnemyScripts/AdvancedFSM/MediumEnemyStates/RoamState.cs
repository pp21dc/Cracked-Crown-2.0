using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class RoamState : FSMState
{

    private EnemyAIController enemy;//grabs the enemy controller

    public RoamState(EnemyAIController controller)
    {
        stateID = FSMStateID.Roam;//sets the FSM state ID to Idle
        enemy = controller;
        curSpeed = 0.0f;
        curRotSpeed = 0.0f;
    }

    //creates transition reasoning for if the enemy has no health, die, if the enemy has low health, flee, and if the enemy is in range of the player, attack
    public override void Reason(Transform player, Transform npc)
    {

        if (enemy.Health <= enemy.maxHealth * 0.20f && enemy.Health >= 1)
        {

            enemy.PerformTransition(Transition.LowHealth);
            return;
        }
        else if (enemy.Health <= 0)
        {

            enemy.PerformTransition(Transition.NoHealth);
            return;
        }
        else if (enemy.CompareTag("Light") || enemy.CompareTag("Heavy"))
        {

            enemy.PerformTransition(Transition.LookForPlayer);
            return;
        }
        else if (Vector3.Distance(enemy.ePosition.position, player.position) <= 500)
        {


            enemy.PerformTransition(Transition.LookForPlayer);
            return;
        }
        else if (enemy.InContact == true)
        {
            Debug.Log("in the incontact thing");
            enemy.sepCheck.enabled = false;
            enemy.InContact = false;
            enemy.PerformTransition(Transition.enemiesInContact);
        }
        else if (enemy.wallContact == true)
        {
            enemy.PerformTransition(Transition.hitDaWall);
        }


    }

    //no need for anything in act as you are jsut standing there until a player attacks you or you see them
    public override void Act(Transform player, Transform npc)
    {
        //Debug.Log("Heavy");

        enemy.StartRoam();
    }
}
