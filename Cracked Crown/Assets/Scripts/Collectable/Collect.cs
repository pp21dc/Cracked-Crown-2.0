using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collect : MonoBehaviour
{

    private PlayerBody body;
    private PlayerController controller;
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" || other.tag == "Ghost")
        {
            body = other.GetComponent<PlayerBody>();
            controller = other.transform.parent.GetChild(0).GetComponent<PlayerController>();

            if (gameObject.tag == "Eye")
            {
                if(body.alreadyDead)
                {
                    body.ghostCoins = body.ghostCoins + 1;
                    Debug.Log(body.ghostCoins);
                    gameObject.SetActive(false);
                }
                else
                {
                    gameManager.eyeCount = gameManager.eyeCount + 1;
                    Debug.Log(gameManager.eyeCount);
                    gameObject.SetActive(false);
                }
            }
            
            if (gameObject.tag == "Potion")
            {
                //if (controller.interactDown && gameManager.eyeCount >= 5 && hasPotion = false && hasBomb = false)
                //{
                //    gameManager.eyeCount -= 5;
                //    body.hasPotion = true;
                //    gameObject.SetActive(false);
                //}
            }


        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (gameObject.tag == "Bomb")
        {
            Debug.Log("Collided with bomb");

            if (controller.InteractDown)
            {
                Debug.Log("pressed interact");

                if (gameManager.eyeCount >= 5 && body.hasPotion == false && body.hasBomb == false)
                {
                    gameManager.eyeCount -= 5;
                    body.hasBomb = true;
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
