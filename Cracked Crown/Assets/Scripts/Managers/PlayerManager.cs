using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{

    [SerializeField]
    private GameObject[] CharacterPrefabs;
    [SerializeField]
    private CharacterType[] CharacterTypes;
    [SerializeField]
    private GameObject[] CharacterImages;
    [SerializeField]
    private GameObject START_Text;
    [SerializeField]
    private PlayerInputManager inputManager;
    [SerializeField]
    private int arrayPos = 0;

    [SerializeField]
    Image readyUpBox;

    GameManager gm;
    public PlayerInput PI;
    public PlayerController PC;
    public PlayerBody PB;

    public int PlayerID;

    private bool Navigate;
    public bool navigate { get { return Navigate; } }

    bool launch;
    bool launch_lock;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (PB != null)
            PB.PM = this;
        if (launch && !launch_lock)
        {
            int lnch = CheckPlayersIndex();
            Debug.Log("LAUNCH: " + lnch);
            PB.currentIN = lnch;
            launch_lock = true;
            START_Text.SetActive(false);
            CharacterImages[lnch].SetActive(true);
            PB.CharacterType = CharacterTypes[lnch];
            PB.SetCharacterData();
        }
        if (PI != null)
        {
            launch = true;
            CallUI();
        }
    }

    public void LockIN(bool active)
    {
        if (!active)
            readyUpBox.color = Color.white;
        else
            readyUpBox.color = Color.green;
    }

    private void CallUI()
    {
        if (PB.lockIN == -1)
        {
            if (PC.NavRight)
                nextButton(1);
            else if (PC.NavLeft)
                nextButton(-1);
        }
    }

    

    public void nextButton(int dir) // for ui to swap to next character
    {
        if (PC != null)
        {
            PC.NavRight = false;
            PC.NavLeft = false;
            
            if (arrayPos + dir > 3)
                arrayPos = 0;
            else if (arrayPos + dir < 0)
                arrayPos = 3;
            else
                arrayPos += dir;
            if (CheckPlayers(arrayPos))
            {
                if (arrayPos - dir > 0 && arrayPos - dir < 4)
                    CharacterImages[arrayPos - dir].SetActive(false);
                else if (arrayPos - dir < 0)
                    CharacterImages[3].SetActive(false);
                else
                    CharacterImages[0].SetActive(false);

                Destroy(PB.CharacterFolder.transform.GetChild(0).gameObject);
                PB.currentIN = arrayPos;
                Instantiate(CharacterPrefabs[arrayPos], PB.CharacterFolder.transform);
                PB.CharacterType = CharacterTypes[arrayPos];
                PB.SetCharacterData();
                CharacterImages[arrayPos].SetActive(true);
                PB.currentIN = arrayPos;
            }
            else
            {
                if (arrayPos - dir > 0 && arrayPos - dir < 4)
                    CharacterImages[arrayPos - dir].SetActive(false);
                else if (arrayPos - dir < 0)
                    CharacterImages[3].SetActive(false);
                else
                    CharacterImages[0].SetActive(false);
                nextButton(dir);
            }
            
            
        }
        
    }

    public bool CheckPlayers(int index)
    {
        foreach (PlayerContainer pc in GameManager.Instance.Players)
        {
            if (pc.PB.lockIN == index)
            {
                return false;
            }
        }
        return true;
    }
    public int CheckPlayersIndex()
    {
        int x = 0;
        for (int i = 0; i < 3; i++)
        {
            foreach (PlayerContainer pc in GameManager.Instance.Players)
            {
                if (pc.PB.lockIN == i)
                {
                    break;
                }
                else
                {
                    x++;
                }
                
            }
            if (x >= GameManager.Instance.Players.Length)
                return i;
        }
        return 0;
    }

        public void backButton() // for ui to swap to previous character
    {
        if (PC != null && PC.NavLeft)
        {
            PC.NavLeft = false;
            CharacterImages[arrayPos].SetActive(false);
            Destroy(PB.CharacterFolder.transform.GetChild(0).gameObject);
            if (arrayPos - 1 < 0)
            {
                Instantiate(CharacterPrefabs[3], PB.CharacterFolder.transform);
                PB.CharacterType = CharacterTypes[3];
                PB.SetCharacterData();
                CharacterImages[3].SetActive(true);
                arrayPos = 3;
            }
            else
            {
                arrayPos--;
                Instantiate(CharacterPrefabs[arrayPos], PB.CharacterFolder.transform);
                PB.CharacterType = CharacterTypes[arrayPos];
                PB.SetCharacterData();
                CharacterImages[arrayPos].SetActive(true);
            }
        }
    }

}
