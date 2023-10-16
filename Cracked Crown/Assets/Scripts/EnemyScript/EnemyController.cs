using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;


public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private float speed = 8f;

    [SerializeField]
    private GameObject[] Players = new GameObject[4];

    public GameObject[] players { get { return Players; } }

    private GameObject closest;

    [SerializeField]
    private GameObject enemyBody;




    private void Awake()
    {

        Players = GameObject.FindGameObjectsWithTag("AddPlayer");


        StartCoroutine(findDistance(closest));

    }

    private void Update()
    {

        closest = Players[0];
        enemyBody.transform.position = Vector3.MoveTowards(closest.transform.position, Vector3.up, speed * Time.deltaTime) * Time.deltaTime;
        enemyBody.transform.position = new Vector3 (enemyBody.transform.position.x, 0, enemyBody.transform.position.z);



    }

    IEnumerator findDistance(GameObject closest)
    {

        float check;
        float currShortest = 10000000000f;


        for (int i = 0; i < Players.Length; i++)
        {
            
            check = Vector3.Distance(gameObject.transform.position, Players[i].transform.position);

            if (check < currShortest) 
            {
            
                currShortest = check;
                closest = Players[i];
            
            }
        
        }

        
         yield return new WaitForSeconds(5);

        StartCoroutine(findDistance(closest));
    
    
    }
        
        



        



}
