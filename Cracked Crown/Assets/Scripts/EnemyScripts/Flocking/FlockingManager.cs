using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingManager : MonoBehaviour
{

    public static FlockingManager FM;
    public EnemyAIController Ai;
    public GameObject[] fishPrefab;
    public int numFish = 25;
    public GameObject[] allFish;
    public Vector3 swimLimits = new Vector3(5, 0, 5); //orgin of the fish tank
    public Vector3 currentGoalPos;
    
    public GameManager gameManager;
    

    [Header("Fish Settings")]
    [Range(0.0f, 15.0f)]
    public float minSpeed;
    [Range(0.0f, 15.0f)]
    public float maxSpeed;
    [Range(1.0f, 10.0f)]
    public float NeighboursDistance;
    [Range(1.0f, 5.0f)]
    public float RotationSpeed;


    // Start is called before the first frame update
    void Start()
    {

        Ai = GetComponentInChildren<EnemyAIController>();

        allFish = GameObject.FindGameObjectsWithTag("Fish");
        numFish = allFish.Length;

        

        FM = this;
        

    }

    // Update is called once per frame
    void Update()
    {
        if(Random.Range(0, 1000) < 5)
        {
            // goalPos = this.transform.position + new Vector3(Random.Range(-swimLimits.x, swimLimits.x), Random.Range(-swimLimits.y, swimLimits.y), Random.Range(-swimLimits.z, swimLimits.z));


            //choses one fo the four players to circle around
            int randomGoal = Random.Range(0, gameManager.Players.Length);

            if(randomGoal == 0)
            {
                currentGoalPos = gameManager.Players[0].PB.transform.position;
                Ai.closest = gameManager.Players[0].PB.gameObject;
            }
            else if(randomGoal == 1)
            {
                currentGoalPos = gameManager.Players[1].PB.transform.position;
                Ai.closest = gameManager.Players[1].PB.gameObject;
            }
            else if (randomGoal == 2)
            {
                currentGoalPos = gameManager.Players[2].PB.transform.position;
                Ai.closest = gameManager.Players[2].PB.gameObject;
            }
            else
            {
                currentGoalPos = gameManager.Players[3].PB.transform.position;
                Ai.closest = gameManager.Players[3].PB.gameObject;
            }

        }
    }
}
