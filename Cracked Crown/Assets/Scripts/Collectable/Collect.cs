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
                        body.ghostCoins = body.ghostCoins + 1;
                        //Debug.Log(body.ghostCoins);
                        body.PAM.PlayAudio(PlayerAudioManager.AudioType.CoinPickUp);
                        Destroy(gameObject.transform.parent.gameObject);
                    }
                    else
                    {
                        body.PAM.PlayAudio(PlayerAudioManager.AudioType.CoinPickUp);
                        gameManager.eyeCount = gameManager.eyeCount + 1;
                        Destroy(gameObject.transform.parent.gameObject);
                    }
                }

                if (other.tag == "Player")
                {
                    body = other.GetComponent<PlayerBody>();

                    if (gameObject.tag == "Bomb")
                    {
                        body.collectable = this;
                        body.canCollectBomb = true;
                    }
                }

                if (other.tag == "Player")
                {
                    body = other.GetComponent<PlayerBody>();

                    if (gameObject.tag == "Potion")
                    {
                        body.collectable = this;
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
