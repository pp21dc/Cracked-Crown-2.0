using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    GameManager gm;
    UIManager um;

    int levelCount;

    [SerializeField] DialogueSingleSO[] bunny;
    [SerializeField] DialogueSingleSO[] frog;
    [SerializeField] DialogueSingleSO[] duck;
    [SerializeField] DialogueSingleSO[] badger;

    [SerializeField] List<int> players = new List<int>();
    private void Awake()
    {
        gm = GameManager.Instance;
        um = UIManager.Instance;
        
    }
    private void Update()
    {
        if(um == null)
        {
            um = UIManager.Instance;
        }

        if (gm.levelNames[gm.CurrentLevel].Equals("1-1"))
        {
            levelCount = 0;
        }
        else if (gm.levelNames[gm.CurrentLevel].Equals("1-2"))
        {
            levelCount = 1;
        }
        else if(gm.levelNames[gm.CurrentLevel].Equals("1-3"))
        {
            levelCount = 2;
        }
    }
    public void SetDialogue()
    {
        for(int i = 0; i < players.Count; i++)
        {
            Debug.Log(players[i]);
        }
        if (players.Contains(1))
        {
            StartCoroutine(um.DisplayDialogue(GetBunnySingle().dialogue, GetBunnySingle().icon));
            Debug.Log(GetBunnySingle().dialogue);
        }
        else if (players.Contains(0))
        {   
            StartCoroutine(um.DisplayDialogue(GetBadgerSingle().dialogue, GetBadgerSingle().icon));
        }
        
        else if (players.Contains(2))
        {
            StartCoroutine(um.DisplayDialogue(GetDuckSingle().dialogue, GetDuckSingle().icon));
        }
        else if (players.Contains(3))
        {
            StartCoroutine(um.DisplayDialogue(GetFrogSingle().dialogue, GetFrogSingle().icon));
        }
    }

    //Get single dialogue based on level
    DialogueSingleSO GetBunnySingle()
    {
        Debug.LogWarning(levelCount);
        return bunny[levelCount];
        
    }
    DialogueSingleSO GetFrogSingle()
    {
        return frog[levelCount];
    }
    DialogueSingleSO GetDuckSingle()
    {
        return duck[levelCount];
    }
    DialogueSingleSO GetBadgerSingle()
    {
        return badger[levelCount];
    }

    public void GetPlayers()
    {
        foreach (PlayerContainer pc in gm.Players)
        {
            players.Add(pc.PB.CharacterType.ID);
        }
    }
}
