using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandlePlayersJoin : MonoBehaviour
{
    [SerializeField]
    private PlayerInputManager PIM;
    private GameManager GM;
    int players;

    private void Awake()
    {
        GM = GameManager.Instance;
    }

    private void Update()
    {
        players = PIM.playerCount;
    }
    public void PlayerJoin()
    {
        GM.Players = FindObjectsOfType<PlayerContainer>();
        int x = 0;
        for (int i = GM.Players.Length-1; i >= 0; i--)
        {
            GM.PMs[x].PI = GM.Players[i].PI;
            GM.PMs[x].PC = GM.Players[i].PC;
            GM.PMs[x].PB = GM.Players[i].PB;
            x++;
        }
    }
}
