using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class FinisherCollider : MonoBehaviour
{

    [SerializeField]
   // private List<MonsterControllerAI> enemiesInRange;

   // [SerializeField]
    private PlayerBody PB;
    [SerializeField]
    private PlayerController controller;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy") // need to add if its under 50% health but no health in game yet so meh
        {
           // enemiesInRange.Add(collision.gameObject.GetComponent<MonsterControllerAI>()); // add enemy to nearby list

           // if (collision.gameObject.GetComponent<MonsterControllerAI>().Health /collision.gameObject.GetComponent<MonsterControllerAI>().MaxHealth <= 0.5) // if health is less then 50% can execute
            {
                if (controller.ExecuteDown)
                {
                   // PB.Execute(enemiesInRange[0].gameObject);
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            //enemiesInRange.Remove(collision.gameObject.GetComponent<MonsterControllerAI>());
        }
    }
}
