using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChange : MonoBehaviour
{
    List<GameObject> players;   //Stores the players in the level change collider
    GameManager GM;             //GameManager
    bool locked;                //Locks level transition to avoid multiple level skips

    private void Awake()
    {
        //Declaring vars
        GM = GameManager.Instance;
        players = new List<GameObject>();
    }

    private void FixedUpdate()
    {
        //If all players in collider, not already transitioning, at least 1 player in the game, and the level is completed
        //Debug.Log(players.Count + " // " + GM.Players.Length + " // " + locked + " // " + GM.IsLevelCleared);
        //DEBUG BYPASS
        //GM.IsLevelCleared = true;
        if (players.Count >= GM.Players.Length && !locked && GM.Players.Length > 0 && GM.IsLevelCleared)
        {
            GM.IsLevelCleared = false;  //Set new level to not cleared
            locked = true;              //Lock out level change while changing
            GM.NextLevel();             //Call game manager to change the level
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //If a player enters the level change collider add them to the player list
        if (other.gameObject.tag == "Player")
        {
            players.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Remove players from player list if they leave the collider
        if (other.gameObject.tag == "Player")
        {
            players.Remove(other.gameObject);
        }
    }
}
