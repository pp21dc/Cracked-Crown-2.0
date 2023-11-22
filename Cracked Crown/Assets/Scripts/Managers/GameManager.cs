using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            if (!instance)
            {
                Debug.LogError("ERROR: NO GAME MANAGER PRESENT");
            }

            return instance;
        }
    }

    public PlayerContainer[] Players;
    public PlayerManager[] PMs;
    public GameObject[] Characters;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public PlayerInput GetPlayer(int ID)
    {
        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[ID].PI.playerIndex == ID)
            {
                return Players[ID].PI;
            }
        }
        return null;
    }

}
