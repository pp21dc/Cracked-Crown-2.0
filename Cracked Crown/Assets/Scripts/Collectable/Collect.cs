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
        if (other.tag == "Player")
        {
            body = other.GetComponent<PlayerBody>();
            controller = other.GetComponent<PlayerController>();
            
            if (gameObject.tag == "Eye")
            {
                gameManager.eyeCount = gameManager.eyeCount + 1;
                Debug.Log(gameManager.eyeCount);
                gameObject.SetActive(false);
            }
            if (gameObject.tag == "Bomb")
            {
                //if (controller.interactDown && gameManager.eyeCount >= 5 && hasPotion = false && hasBomb = false)
                //{
                //    gameManager.eyeCount -= 5;
                //    body.hasBomb = true;
                //    gameObject.SetActive(false);
                //}
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
}
