using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class FinisherCollider : MonoBehaviour
{

   public List<GameObject> enemiesInRange;

   [SerializeField]
    private PlayerBody PB;
    [SerializeField]
    private PlayerController controller;

    private void Update()
    {
        if (enemiesInRange != null)
        {
            foreach (GameObject enemy in enemiesInRange)
            {
                if (((enemy.gameObject.GetComponent<EnemyAIController>().Health) / (enemy.gameObject.GetComponent<EnemyAIController>().maxHealth)) <= 0.5) // if health is less then 50% can execute
                {
                    Debug.Log("Can Execute");
                    if (controller.ExecuteDown)
                    {
                        PB.Execute(enemiesInRange[0].gameObject);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            enemiesInRange.Add(other.transform.parent.GetChild(0).gameObject); // add enemy to nearby list
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            enemiesInRange.Remove(other.transform.parent.GetChild(0).gameObject);
        }
    }

}
