using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAnimationEvent : MonoBehaviour
{

    EnemyAIController enemy; //grabs the enemy controller


    public void Awake()
    {
        enemy = GetComponent<EnemyAIController>();
    }

    //calls on the animation event to shoot the bullet at that point in the animation
    public void fireBullet()
    {

        Debug.Log("shoot");

    }

   
}
