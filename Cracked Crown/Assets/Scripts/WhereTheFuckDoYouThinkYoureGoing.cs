using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhereTheFuckDoYouThinkYoureGoing : MonoBehaviour
{
    GameManager GM;

    private void Start()
    {
        GM = GameManager.Instance;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            Debug.Log(other.name);
            GM.SetPlayerPositions();
        }
        if (other.tag.Equals("Tooth"))
        {
            Destroy(other.gameObject);
        }
    }

}
