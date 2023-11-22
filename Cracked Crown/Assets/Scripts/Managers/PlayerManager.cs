using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{

    [SerializeField]
    private GameObject[] characterTypes;
    [SerializeField]
    private GameObject[] CharacterImages;
    [SerializeField]
    private GameObject START_Text;
    [SerializeField]
    private PlayerInputManager inputManager;
    [SerializeField]
    private int arrayPos = 0;

    public PlayerInput PI;
    public PlayerController PC;
    public PlayerBody PB;

    public int PlayerID;

    private bool Navigate;
    public bool navigate { get { return Navigate; } }

    bool launch;
    bool launch_lock;

    private void Update()
    {
        if (launch && !launch_lock)
        {
            launch_lock = true;
            START_Text.SetActive(false);
            CharacterImages[0].SetActive(true);
        }
        if (PI != null)
        {
            launch = true;
            CallUI();
        }
    }

    private void CallUI()
    {
        nextButton();
        backButton();
    }

    

    public void nextButton() // for ui to swap to next character
    {
        if (PC.NavRight)
        {
            PC.NavRight = false;
            CharacterImages[arrayPos].SetActive(false);
            Destroy(PB.CharacterFolder.transform.GetChild(0).gameObject);
            if (arrayPos + 1 > 3)
            {
                Instantiate(characterTypes[0], PB.CharacterFolder.transform);
                CharacterImages[0].SetActive(true);
                arrayPos = 0;
            }
            else
            {
                arrayPos++;
                Instantiate(characterTypes[arrayPos], PB.CharacterFolder.transform);
                CharacterImages[arrayPos].SetActive(true);
            }
            
            
        }
        
    }

    public void backButton() // for ui to swap to previous character
    {
        if (PC.NavLeft)
        {
            PC.NavLeft = false;
            CharacterImages[arrayPos].SetActive(false);
            Destroy(PB.CharacterFolder.transform.GetChild(0).gameObject);
            if (arrayPos - 1 < 0)
            {
                Instantiate(characterTypes[3], PB.CharacterFolder.transform);
                CharacterImages[3].SetActive(true);
                arrayPos = 3;
            }
            else
            {
                arrayPos--;
                Instantiate(characterTypes[arrayPos], PB.CharacterFolder.transform);
                CharacterImages[arrayPos].SetActive(true);
            }
        }
    }

}
