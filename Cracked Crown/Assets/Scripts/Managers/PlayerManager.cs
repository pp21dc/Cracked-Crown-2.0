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
    GameObject LockIn_F;
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
        gm = GameManager.Instance;
    }

    private void Update()
    {
        if (PB != null)
            PB.PM = this;
        if (launch && !launch_lock)
        {

            int lnch = CheckPlayersIndex();
            PB.currentIN = lnch;
            launch_lock = true;
            START_Text.SetActive(false);
            LockIn_F.SetActive(true);

            Destroy(PB.CharacterFolder.transform.GetChild(0).gameObject);
            Instantiate(CharacterPrefabs[lnch], PB.CharacterFolder.transform);

            for (int i = 0; i < 4; i++)
                CharacterImages[i].SetActive(false);
            CharacterImages[lnch].SetActive(true);
            PB.CharacterType = CharacterTypes[lnch];
            PB.SetCharacterData();
            PB.spriteRenderer = PB.CharacterFolder.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>(); 
            foreach (PlayerContainer container in gm.Players)
            {
                //SetAllMarkers();
                container.PB.PM.SetMarkers();
            }
            Debug.Log(gm.Players.Length);
        }
        if (PI != null)
        {
            launch = true;
            CallUI();
        }
    }

    public void SetMarkers()
    {
        if (PB.CharacterFolder.transform.childCount == 0 && gm.Players != null)
            return;
        SetAllMarkers();
        for (int i = 0; i < gm.Players.Length; i++)
        {
            if (gm.Players[i].PB.playerID == PB.playerContainer.PB.playerID)
                PB.CharacterFolder.transform.GetChild(0).GetChild(1).GetChild(i).gameObject.SetActive(true);
        }
    }

    private void SetAllMarkers()
    {
        for (int i = 0; i < 4; i++)
        {
            PB.CharacterFolder.transform.GetChild(0).GetChild(1).GetChild(i).gameObject.SetActive(false);
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
            SetMarkers();
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
                SetMarkers();
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
        for (int i = 0; i <= 3; i++)
        {
            foreach (PlayerContainer pc in GameManager.Instance.Players)
            {
                if (pc.PB.currentIN == i)
                {
                    break;
                }
                else
                {
                    x++;
                }
                
            }
            if (x >= GameManager.Instance.Players.Length-1)
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
