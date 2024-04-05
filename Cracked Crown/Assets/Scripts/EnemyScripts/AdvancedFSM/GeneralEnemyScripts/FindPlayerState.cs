using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FindPlayerState : FSMState
{

    private EnemyAIController enemy;//grabs the enemy controller

    public FindPlayerState(EnemyAIController controller)
    {
        stateID = FSMStateID.FindPlayer;//sets the FSM state ID to Idle
        enemy = controller;
        
    }

    //creates transition reasoning for if the enemy has no health, die, if the enemy has low health, flee, and if the enemy is in range of the player, attack
    public override void Reason(Transform player, Transform npc)
    {
        //Debug.Log("Made it to the Find player Reasoning");

        if (enemy.Health <= enemy.maxHealth * 0.74f && enemy.Health >= 1) 
        {
            enemy.PerformTransition(Transition.LowHealth);
        }
        else if (enemy.Health < 1)
        {
            enemy.PerformTransition(Transition.NoHealth);
        }
        else if (enemy.InContact == true)
        {
            Debug.Log("in the incontact thing");
            enemy.sepCheck.enabled = false;
            enemy.InContact = false;
            enemy.PerformTransition(Transition.enemiesInContact);
            
        }
        else if (enemy.CompareTag("Light"))
        {
            if (enemy.startSlam == true && enemy.canPickup && enemy.checkPlayerBelow.IsPlayerBelow())
            {
                //Debug.Log("EEEEEEEEEEEE");
                enemy.startSlam = false;
                enemy.PerformTransition(Transition.AbovePlayer);
                
                return;
            }
        }
        else if (enemy.CompareTag("Medium") == true && player != null)
        {
            //Debug.Log("Found the Medium");
            if (Vector3.Distance(enemy.ePosition.position,player.position) <= 60f && enemy.dashOnCD == false)
            {
                enemy.PerformTransition(Transition.InFirstRange);
                return;
            }
            
        }
        else if (enemy.CompareTag("Heavy") && player != null)
        {
            if (Vector3.Distance(enemy.ePosition.position,player.position) >= 105f && enemy.shootOnCD == false)
            {
                enemy.PerformTransition(Transition.InShootingRange);
                return;
            }
            else if (enemy.Health <= 50f && enemy.shockwaveOnCD == false)
            {
                enemy.PerformTransition(Transition.InShockwaveRange);
                return;
            }
        }
        
    }

    //no need for anything in act as you are jsut standing there until a player attacks you or you see them
    public override void Act(Transform player, Transform npc)
    {
        //Debug.Log("Made it to the Find player Act");
        enemy.checkShortestDistance();
    }
}
