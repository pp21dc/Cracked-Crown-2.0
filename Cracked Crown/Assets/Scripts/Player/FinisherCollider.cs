using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinisherCollider : MonoBehaviour
{

    [SerializeField]
    private List<GameObject> enemiesInRange;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy") // need to add if its under 50% health but no health in game yet so meh
        {
            //enemiesInRange[0] = collision.gameObject; // need to add enemy to list, and some point take them out when they far away

            //if (collision.gameObject.health = lessthan50%)
            //{
            //    canPressExecute = true;
            //}
        }
    }
}
