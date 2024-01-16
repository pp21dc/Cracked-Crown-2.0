using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{

    public bool gamePaused = false;

    [SerializeField]
    GameObject menuUI;
    [SerializeField]
    GameObject settingsMenu;

    [SerializeField]
    GameObject settingsInitButton;
    [SerializeField]
    GameObject pauseMenuInitButton;

    [SerializeField]
    PlayerController Player1;

    [SerializeField]
    PlayerController Player2;

    [SerializeField]
    PlayerController Player3;

    [SerializeField]
    PlayerController Player4;
    //once we have the players in the scene, we'll 
    
    
    // Update is called once per frame
    void Update()
    {
        if(Player1.PauseDown || Player2.PauseDown || Player3.PauseDown || Player4.PauseDown)
        {
            Debug.Log("if 1");
            if (menuUI.activeSelf == false)
            {
                Pause();
            }
            
        }
    }

    private void Pause()
    {
        menuUI.SetActive(true);
        Time.timeScale = 0f;

        gamePaused = true;
    }

    public void Resume()
    {
        menuUI.SetActive(false);
        Time.timeScale = 1f;

        gamePaused = false;
    }

    private void closePause()
    {
        menuUI.SetActive(false);
    }

    public void OpenSettings()
    {
        pauseMenuInitButton = EventSystem.current.currentSelectedGameObject;
        settingsMenu.SetActive(true);
        closePause();
        EventSystem.current.SetSelectedGameObject(settingsInitButton);
    }

    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
        Pause();
        EventSystem.current.SetSelectedGameObject(pauseMenuInitButton);
    }
}
