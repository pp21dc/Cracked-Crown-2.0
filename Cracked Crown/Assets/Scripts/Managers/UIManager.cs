using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class UIManager : MonoBehaviour
{
    [Header("Player Controllers")]
    [SerializeField] PlayerController player1;
    [SerializeField] PlayerController player2;
    [SerializeField] PlayerController player3;
    [SerializeField] PlayerController player4;

    [Header("Main Menu")]
    [SerializeField] private GameObject MainMenu;

    [Header("In Game UI")]
    [SerializeField] private GameObject InGameUI;


    [Header("Pause Menu")]
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject pauseMenuInitButton;
    private bool gamePaused = false;

    [Header("Settings Menu")]
    [SerializeField] private GameObject SettingsMenu;
    [SerializeField] Slider MasterSlider;
    [SerializeField] Slider SFXSlider;
    [SerializeField] Slider MusicSlider;
    [SerializeField] TMP_Dropdown Resolution;
    [SerializeField] private AudioMixer Mixer;
    [SerializeField] private GameObject settingsInitButton;
    private bool isFullscreen = true;

    [Header("Character Select")]
    [SerializeField] private GameObject CharacterSelectUI;
    private void Update()
    {
        if (player1.PauseDown || player2.PauseDown || player3.PauseDown || player4.PauseDown)
        {
            Debug.Log("if 1");
            if (PauseMenu.activeSelf == false)
            {
                Pause();
            }

        }
    }

    //Functions For Pause Menu (Copied from PauseMenu Script 1/30/2024)
    private void Pause()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0f;

        gamePaused = true;
    }

    public void Resume()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1f;

        gamePaused = false;
    }

    private void closePause()
    {
        PauseMenu.SetActive(false);
    }

    public void OpenSettings()
    {
        pauseMenuInitButton = EventSystem.current.currentSelectedGameObject;
        SettingsMenu.SetActive(true);
        closePause();
        EventSystem.current.SetSelectedGameObject(settingsInitButton);
    }

    public void CloseSettings()
    {
        SettingsMenu.SetActive(false);
        Pause();
        EventSystem.current.SetSelectedGameObject(pauseMenuInitButton);
    }

    //Settings Menu
    public void setResolution()
    {
        if (Resolution.value == 0)
        {
            Screen.SetResolution(1920, 1080, isFullscreen);
        }
        else if (Resolution.value == 1)
        {
            Screen.SetResolution(2560, 1440, isFullscreen);
        }
        else if (Resolution.value == 2)
        {
            Screen.SetResolution(1280, 720, isFullscreen);
        }
    }

    public void setMasterVol()
    {
        Mixer.SetFloat("MasterVol", MasterSlider.value);
    }

    public void setSFXVol()
    {
        Mixer.SetFloat("SFXVol", SFXSlider.value);
    }

    public void setMusicVol()
    {
        Mixer.SetFloat("MusicVol", MusicSlider.value);
    }
}
