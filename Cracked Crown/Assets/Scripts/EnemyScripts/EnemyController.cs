using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;


public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private float speed = 0.008f; //speed of the enemy

    [SerializeField]
    private GameObject[] Players = new GameObject[4]; //holds all players


    private GameObject closest;//holds the closest player

    [SerializeField]
    private Transform enemyBody; //holds the enemy player position

    private float currShortest = 100000f; //current shortest distance
    private Vector3 movementVector = Vector3.zero; // the vector that the enemy is moving towards

    public int health = 100;






    private void Awake()
    {

        Players = GameObject.FindGameObjectsWithTag("Player");//finds and add all players to array



        

    }

    private void Update()
    {

        checkShortestDistance();//finds closest player
        
        



    }

    
    //finds the closest player and sets the target position
    private void checkShortestDistance()
    {

        float check;

        for (int i = 0; i < Players.Length; i++)
        {

            check = Vector3.Distance(gameObject.transform.position, Players[i].transform.position);

            if (check < currShortest)
            {

                currShortest = check;
                closest = Players[i];

            }

        }

        //setAndMoveToTarget();

    }

    //sets enemy target position and moves towards it
    private void setAndMoveToTarget() 
    {
        Debug.Log("HIIIIII");
        movementVector = (closest.transform.position - enemyBody.transform.position).normalized * speed;
        enemyBody.transform.position += movementVector * Time.deltaTime;//moves to player
        enemyBody.transform.position = new Vector3(enemyBody.position.x, 0f, enemyBody.position.z); //keeps it on ground

    }
        
        



        



}
