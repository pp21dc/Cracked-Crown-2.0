using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject InGameUI;

    [SerializeField]
    private GameObject MainMenu;

    [SerializeField]
    private GameObject PauseMenu;
    private PauseMenu pauseMenu;

    [SerializeField]
    private GameObject SettingsMenu;

    [SerializeField]
    private GameObject CharacterSelectUI;


    int state = 0;
    private enum States
    {
        InGameUI,
        MainMenu,
        PauseMenu,
        SettingsMenu,
        CharacterSelectUI
    }
    [SerializeField]
    PlayerController controller;
    // Start is called before the first frame update
    void Start()
    {
        pauseMenu = PauseMenu.GetComponent<PauseMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.PauseDown)
        {
            if (MainMenu.activeSelf)
            {
                //Progress to character select
            }

            if (PauseMenu.activeSelf)
            {
                //close pause menu (run "unpause" function in PauseMenu script)
            }

            if (InGameUI.activeSelf)
            {
                //open pause menu (run "pause" menu in PauseMenu script)
            }

            if (CharacterSelectUI.activeSelf)
            {
                //open pause menu (run "pause" menu in PauseMenu script)
            }
        }
    }
}
