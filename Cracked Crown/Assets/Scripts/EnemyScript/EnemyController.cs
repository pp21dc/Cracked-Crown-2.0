using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private float speed = 8f;

    [SerializeField]
    private GameObject[] Players = new GameObject[4];

    public GameObject[] players { get { return Players; } }

    private GameObject playerLocator;



    private void Awake()
    {

        Players = GameObject.FindGameObjectsWithTag("AddPlayer");


        StartCoroutine("findDistance");

    }

    IEnumerator findDistance()
    {

        float check;
        float currShortest = 10000000000f;
        GameObject closest;
    
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

        StartCoroutine("findDistance");
    
    
    }
        
        



        



}
