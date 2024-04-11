using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard
{
    public int TimesSwung = 0;//
    public int EnemiesKilled = 0;//
    public int ExecutesDone = 0;//
    public int BombsThrown = 0;//
    public int PotionsUsed = 0;//
    public int Deaths = 0;//
    public int Revives = 0;//
    public float CoinsCollected = 0;//
    public float GhostCoinsCollected = 0;//
    public int Steps = 0;
    public int Dashes = 0;//
    public int Struggled = 0;//

    public void LogAll()
    {
        Debug.Log(TimesSwung);
        Debug.Log(EnemiesKilled);
        Debug.Log(ExecutesDone);
        Debug.Log(BombsThrown);
        Debug.Log(PotionsUsed);
        Debug.Log(Deaths);
        Debug.Log(Revives);
        Debug.Log(CoinsCollected);
        Debug.Log(GhostCoinsCollected);
        Debug.Log(Steps);
        Debug.Log(Dashes);
        Debug.Log(Struggled);

    }
}
