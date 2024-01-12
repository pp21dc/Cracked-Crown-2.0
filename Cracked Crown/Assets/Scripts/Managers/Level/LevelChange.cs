using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChange : MonoBehaviour
{
    List<GameObject> players;
    GameManager GM;
    bool locked;

    private void Awake()
    {
        GM = GameManager.Instance;
        players = new List<GameObject>();
    }

    private void FixedUpdate()
    {
        if (players.Count >= GM.Players.Length && !locked)
        {
            locked = true;
            GM.NextLevel();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            players.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            players.Remove(other.gameObject);
        }
    }
}
