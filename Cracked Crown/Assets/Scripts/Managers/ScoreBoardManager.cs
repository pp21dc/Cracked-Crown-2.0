using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreBoardManager : MonoBehaviour
{

    public static ScoreBoardManager instance;
    public GameObject Scoreboard;
    bool canExit = false;
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
            Scoreboard.SetActive(false);
            canExit = false;
            ResetScores();
        }
    }

    public void On()
    {
        StartCoroutine(Skip());
        Scoreboard.SetActive(true);
    }

    IEnumerator Skip()
    {
        yield return new WaitForSeconds(3);
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
    }

}
