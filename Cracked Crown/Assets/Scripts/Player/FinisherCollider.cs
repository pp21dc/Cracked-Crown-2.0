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
    GameObject e;
    float distance;
    private void Update()
    {
        Debug.Log("CanExecute: " + PB.canExecute + " :: " + controller.ExecuteDown);
        if (enemiesInRange != null && controller.ExecuteDown)
        {
            foreach (GameObject enemy in enemiesInRange)
            {
                
                if (enemy != null && enemy.transform.parent.gameObject.activeSelf)
                {

                    
                    if ((((enemy.gameObject.GetComponent<EnemyAIController>().Health) / (enemy.gameObject.GetComponent<EnemyAIController>().maxHealth)) <= 0.5) && PB.canExecute) // if health is less then 50% can execute
                    {
                        if (Vector3.Distance(enemy.transform.position, PB.transform.position) > distance)
                        {
                            //PB.canExecute = false;
                            distance = Vector3.Distance(enemy.transform.position, PB.transform.position);
                            PB.Execute(enemy.transform.parent.GetChild(1).gameObject);
                            e = enemy;
                            break;
                        }
                        
                    }
                }
                
            }
            //PB.canExecute = true;
            distance = 0;
            if (e != null)
            {
                enemiesInRange.Remove(e);
                e = null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Medium")
        {
            enemiesInRange.Add(other.transform.parent.GetChild(0).gameObject); // add enemy to nearby list
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Medium")
        {
            enemiesInRange.Remove(other.transform.parent.GetChild(0).gameObject);
        }
    }

}
