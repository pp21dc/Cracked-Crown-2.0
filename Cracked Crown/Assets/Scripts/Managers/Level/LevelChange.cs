using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class LevelChange : MonoBehaviour
{
    List<GameObject> players;   //Stores the players in the level change collider
    GameManager GM;             //GameManager
    bool locked;                //Locks level transition to avoid multiple level skips
    public SpriteRenderer Eye;

    [SerializeField]
    GameObject doorLight;

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
        if ((players.Count >= GM.Players.Length && !locked && GM.Players.Length > 0 && GM.IsLevelCleared && !GM.waitforvideo) || (Input.GetKey(KeyCode.N) && !locked))
        {
            
            if (CheckLockedIn() && !GM.isLoading)
            {
                Debug.Log("NEXT");
                openDoor = false;
                GM.IsLevelCleared = false;  //Set new level to not cleared
                locked = true;              //Lock out level change while changing
                GM.NextLevel();             //Call game manager to change the level
            }
        }
        else if (GM.IsLevelCleared && !openDoor)
        {
            openDoor = true;
            StartCoroutine(ShutEye());
        }
    }

    private bool CheckLockedIn()
    {
        int x = 0;
        foreach(PlayerContainer player in GM.Players)
        {
            if (player.PB.lockIN != -1)
                x++;
        }
        if (x >= GM.Players.Length)
            return true;
        else
            return false;
    }

    bool openDoor;
    IEnumerator ShutEye()
    {
        doorLight.SetActive(true);
        while (Eye.color.a > 0.01f)
        {
            Eye.color = new Color(Eye.color.r, Eye.color.g, Eye.color.b, Eye.color.a-0.25f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        };
        Eye.gameObject.SetActive(false);
        
    }

    private void OnTriggerStay(Collider other)
    {
        //If a player enters the level change collider add them to the player list
        if (!players.Contains(other.gameObject) && (other.gameObject.tag == "Player" || other.gameObject.tag == "Ghost") && GM.IsLevelCleared && openDoor)
        {
            PlayerBody pb = other.GetComponent<PlayerBody>();
            if (pb.lockIN != -1)
            {
                pb.ExitLevel();
                players.Add(other.gameObject);
            }
            

        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Remove players from player list if they leave the collider
        if (other.gameObject.tag == "Player")
        {
            //players.Remove(other.gameObject);
        }
    }
}
