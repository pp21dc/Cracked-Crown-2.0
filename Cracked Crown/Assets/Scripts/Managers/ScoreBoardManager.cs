using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreBoardManager : MonoBehaviour
{

    public static ScoreBoardManager instance;
    public GameObject Scoreboard;
    bool canExit = false;
    public bool active;
    [SerializeField]
    private TextMeshProUGUI BunnyStats;
    [SerializeField]
    private TextMeshProUGUI DuckStats;
    [SerializeField]
    private TextMeshProUGUI FrogStats;
    [SerializeField]
    private TextMeshProUGUI BadgerStats;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.anyKeyDown && canExit)
        {
            active = false;
            Scoreboard.SetActive(false);
            canExit = false;
            GameManager.Instance.FreezePlayers(false);
            //GameManager.Instance.RevivePlayers();
            ResetScores();
        }
    }

    public void On()
    {
        active = true;
        StartCoroutine(Skip());
        GameManager.Instance.FreezePlayers(true);
        Scoreboard.SetActive(true);
    }

    IEnumerator Skip()
    {
        yield return new WaitForSeconds(6.5f);
        canExit = true;
    }

    public void SetBunnyStats(string bunnyStats)
    {
        BunnyStats.text = bunnyStats;
    }

    public void SetDuckStats(string duckStats)
    {
        DuckStats.text = duckStats;
    }

    public void SetFrogStats(string frogStats)
    {
        FrogStats.text = frogStats;
    }

    public void SetBadgerStats(string badgerStats)
    {
        BadgerStats.text = badgerStats;
    }

    public void ResetScores()
    {
        BunnyStats.text = "";
        DuckStats.text = "";
        FrogStats.text = "";
        BadgerStats.text = "";
        foreach(PlayerContainer pc in GameManager.Instance.Players)
        {
            pc.PB.scoreboard.Reset();
        }
    }

}
