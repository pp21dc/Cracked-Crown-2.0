using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;


public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private float speed = 0.008f;

    [SerializeField]
    private GameObject[] Players = new GameObject[4];

    public GameObject[] players { get { return Players; } }

    private GameObject closest;

    [SerializeField]
    private Transform enemyBody;

    private float currShortest = 100000f;
    private Vector3 movementVector = Vector3.zero;






    private void Awake()
    {

        Players = GameObject.FindGameObjectsWithTag("AddPlayer");



        StartCoroutine("findDistance");

    }

    private void Update()
    {

        checkShortestDistance();
        
        enemyBody.transform.position += movementVector * Time.deltaTime;
        enemyBody.transform.position = new Vector3 (enemyBody.position.x, 0f, enemyBody.position.z);



    }

    

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

        setTarget();

    }

    private void setTarget() 
    {
    
        movementVector = (closest.transform.position - enemyBody.transform.position).normalized * speed;
    
    }
        
        



        



}
