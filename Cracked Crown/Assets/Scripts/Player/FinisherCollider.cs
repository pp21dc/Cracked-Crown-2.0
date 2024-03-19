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
        //Debug.Log("CanExecute: " + PB.canExecute + " :: " + controller.ExecuteDown);
        if (enemiesInRange != null && controller.ExecuteDown)
        {
            distance = -100;
            foreach (GameObject enemy in enemiesInRange)
            {
                
                if (enemy != null && enemy.transform.parent.gameObject.activeSelf)
                {

                    if ((enemy.gameObject.GetComponent<EnemyAIController>() != null && enemy.gameObject.GetComponent<EnemyAIController>().inFinish && PB.canExecute)) // if health is less then 50% can execute
                    {
                        if (Vector3.Distance(enemy.transform.position, PB.transform.position) > distance)
                        {
                            //PB.canExecute = false;
                            distance = Vector3.Distance(enemy.transform.position, PB.transform.position);
                            e = enemy;
                        }
                    }
                    else if (enemy.gameObject.GetComponent<CrabWalk>() != null && PB.canExecute)
                    {
                        if (Vector3.Distance(enemy.transform.position, PB.transform.position) > distance)
                        {
                            //PB.canExecute = false;
                            distance = Vector3.Distance(enemy.transform.position, PB.transform.position);
                            e = enemy;
                        }
                    }
                }
                
            }
            if (e != null && e.transform.parent.GetChild(1) != null)
            {
                 if (e.gameObject.tag != "MiniCrabExecutable" && !e.gameObject.GetComponent<EnemyAIController>().EAC.Dead)
                    PB.Execute(e.transform.parent.GetChild(1).gameObject);
                else if (e.gameObject.tag == "MiniCrabExecutable")
                {
                    PB.Execute(e.transform.parent.GetChild(1).gameObject);
                }
            }
                
            else if (e == null)
            {
                enemiesInRange.Remove(e);
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
        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "MiniCrabExecutable")
        {
            enemiesInRange.Add(other.transform.parent.GetChild(0).gameObject); // add enemy to nearby list
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "MiniCrabExecutable")
        {
            enemiesInRange.Remove(other.transform.parent.GetChild(0).gameObject);
        }
    }

}
