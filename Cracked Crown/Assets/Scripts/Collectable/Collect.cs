using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collect : MonoBehaviour
{

    private PlayerBody body;
    private GameManager gameManager;
    Vector3 initialPos;
    float currentPos = 0;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        initialPos = this.transform.position;
    }

    private void Update()
    {
        // so it only does it to the bomb and potion
        if (gameObject.tag == "Bomb" || gameObject.tag == "Potion")
        {
            for (int i = 0; i < gameManager.Players.Length; i++)
            {
                float distance = Vector3.Distance(initialPos, gameManager.Players[i].PB.transform.position);
                currentPos = ((-Mathf.Log(distance)) + 5);

                if (currentPos >= 3)
                {
                    currentPos = 3;
                }
                if (currentPos <= 0)
                {
                    currentPos = 0;
                }
                if (transform.position.y - initialPos.y >= 3)
                {
                    transform.position = initialPos + new Vector3(0, currentPos, 0);
                }
                if (transform.position.y - initialPos.y <= 0)
                {
                    transform.position = initialPos;
                }
                transform.position = initialPos + new Vector3(0, currentPos, 0);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if ((other.tag == "Player" || other.tag == "Ghost"))
        {
            body = other.GetComponent<PlayerBody>();
            if (body.canCollect)
            {

                if (gameObject.tag == "Eye")
                {
                    if (body.alreadyDead)
                    {
                        body.scoreboard.GhostCoinsCollected += 0.5f;
                        body.ghostCoins = body.ghostCoins + 0.5f;
                        //Debug.Log(body.ghostCoins);
                        body.PAM.PlayAudio(PlayerAudioManager.AudioType.CoinPickUp);
                        Destroy(gameObject.transform.parent.gameObject);
                    }
                    else
                    {
                        body.scoreboard.CoinsCollected += 0.5f;
                        body.PAM.PlayAudio(PlayerAudioManager.AudioType.CoinPickUp);
                        gameManager.eyeCount = gameManager.eyeCount + 0.5f;
                        Destroy(gameObject.transform.parent.gameObject);
                    }
                }

                if (other.tag == "Player")
                {
                    body = other.GetComponent<PlayerBody>();

                    if (gameObject.tag == "Bomb")
                    {
                        body.collectable = this;
                        body.canCollectPotion = false;
                        body.canCollectBomb = true;
                    }
                }

                if (other.tag == "Player")
                {
                    body = other.GetComponent<PlayerBody>();

                    if (gameObject.tag == "Potion")
                    {
                        body.collectable = this;
                        body.canCollectBomb = false;
                        body.canCollectPotion = true;
                    }
                }
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            body = other.GetComponent<PlayerBody>();

            if (gameObject.tag == "Bomb")
            {
                body.canCollectBomb = false;
            }
        }
        if (other.tag == "Player")
        {
            body = other.GetComponent<PlayerBody>();

            if (gameObject.tag == "Potion")
            {
                body.canCollectPotion = false;
            }
        }
    }
}
