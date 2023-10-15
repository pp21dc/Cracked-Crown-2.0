using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPlayer : MonoBehaviour
{

    private EnemyController enemyController;

    // Start is called before the first frame update
    private void Awake()
    {
        
        enemyController = GetComponent<EnemyController>();
        
        for (int i = 0; i < 4; i++) 
        {
        
            if (enemyController.players[i] == null)
            {
                enemyController.players[i] = gameObject;
                break;
            }

        }

    }
}
