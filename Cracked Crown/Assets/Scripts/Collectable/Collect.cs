using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collect : MonoBehaviour
{

    private PlayerBody body;
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {

           body = other.GetComponent<PlayerBody>();
            
            if (gameObject.tag == "Eye")
            {
                gameManager.eyeCount = gameManager.eyeCount + 1;
                Debug.Log(gameManager.eyeCount);
            }
            if (gameObject.tag == "Bomb")
            {
                //body.hasBomb = true;
            }
            if (gameObject.tag == "Potion")
            {
                //body.hasPotion = true;
            }

            gameObject.SetActive(false);
        }
    }
}
