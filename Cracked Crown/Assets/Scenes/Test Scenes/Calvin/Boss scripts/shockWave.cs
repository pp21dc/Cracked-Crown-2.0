using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shockWave : MonoBehaviour
{
    private GameObject[] playerList = new GameObject[4];
    void Awake()
    {
        StartCoroutine("Death");
    }


    IEnumerator Death ()
    {
        yield return new WaitForSeconds(1.25f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < 4; i++)
        {
            if (playerList[i] != null)
            {
                if (playerList[i] == other.gameObject)
                {
                    return;
                } 
            }
        }
        if (other.gameObject.tag == "Player")
        {
            PlayerBody playerScript = other.gameObject.GetComponent<PlayerBody>();
            if (!playerScript.dashing)
            {
                playerScript.DecHealth(3);
            }

        }
    }
}
