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
    public int PlayersKilled = 0;
    public int CrabsKilled = 0;

    public void Reset()
    {
        TimesSwung = 0;//
        EnemiesKilled = 0;//
        ExecutesDone = 0;//
        BombsThrown = 0;//
        PotionsUsed = 0;//
        Deaths = 0;//
        Revives = 0;//
        CoinsCollected = 0;//
        GhostCoinsCollected = 0;//
        Steps = 0;
        Dashes = 0;//
        Struggled = 0;//
        PlayersKilled = 0;
        CrabsKilled = 0;
    }

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

    public string CreateString()
    {
        string output = "";
        output = "Swings:\t\t" + TimesSwung + "\n";
        output += "Kills:\t\t\t" + EnemiesKilled + "\n";
        output += "Executes:\t\t" + ExecutesDone+ "\n";
        output += "Bombs Thrown:\t" + BombsThrown + "\n";
        output += "Potions Used:\t" + PotionsUsed + "\n";
        output += "Deaths:\t\t" + Deaths + "\n";
        output += "Revives:\t\t" + Revives + "\n";
        output += "Eyes Collected:\t" + CoinsCollected + "\n";
        output += "Steps:\t\t\t" + Steps + "\n";
        output += "Dashes:\t\t" + Dashes + "\n";
        output += "Struggled:\t\t" + Struggled + "\n";
        output += "Players Killed:\t" + PlayersKilled + "\n";
        output += "Crabs Killed:\t" + CrabsKilled + "\n";

        return output;
    }

}
