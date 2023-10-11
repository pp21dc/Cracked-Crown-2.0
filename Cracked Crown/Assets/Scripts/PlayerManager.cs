using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{

    [SerializeField]
    private GameObject[] playerPrefabs;
    [SerializeField]
    private PlayerInputManager inputManager;
    [SerializeField]
    private int arrayPos = 0;

    private void Awake()
    {
        setDefaultPlayer();
    }

    void setDefaultPlayer()
    {
        inputManager.playerPrefab = playerPrefabs[arrayPos]; // default to prefab 0
    }

    public void nextButton() // for ui to swap to next character
    {
        arrayPos++;
        if (arrayPos > 3)
        {
            arrayPos = 0;
        }
        Debug.Log(arrayPos);
        inputManager.playerPrefab = playerPrefabs[arrayPos];
    }

    public void backButton() // for ui to swap to previous character
    {
        arrayPos--;
        if (arrayPos < 0)
        {
            arrayPos = 3;
        }
        Debug.Log(arrayPos);
        inputManager.playerPrefab = playerPrefabs[arrayPos];
    }

}
