using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GunState : FSMState
{

    private EnemyAIController enemy;//grabs the enemy controller

    public GunState(EnemyAIController controller)
    {
        stateID = FSMStateID.Gun;//sets the FSM state ID to Idle
        enemy = controller;
        curSpeed = 0.0f;
        curRotSpeed = 0.0f;
    }

    //creates transition reasoning for if the enemy has no health, die, if the enemy has low health, flee, and if the enemy is in range of the player, attack
    public override void Reason(Transform player, Transform npc)
    {
        if (enemy.Health <= 20 && enemy.Health >= 1)
        {
            enemy.startShooting = true;
            enemy.heavyBullets = enemy.maxAmmo;
            enemy.PerformTransition(Transition.LowHealth);
            return;
        }
        else if (enemy.Health <= 0)
        {

            enemy.PerformTransition(Transition.NoHealth);
            return;
        }
        else if (enemy.Health <= 50f && enemy.shockwaveOnCD == false)
        {
            enemy.startShooting = true;
            enemy.heavyBullets = enemy.maxAmmo;
            enemy.PerformTransition(Transition.InShockwaveRange);
            return;
        }
        else if (enemy.heavyBullets == 0)
        {
            
            enemy.ResetShotVar();
            enemy.canShoot = false;
            enemy.PerformTransition(Transition.NoBullets);
            return;
        }
        


    }

    //no need for anything in act as you are jsut standing there until a player attacks you or you see them
    public override void Act(Transform player, Transform npc)
    {
        enemy.StartShooting();
    }
}
