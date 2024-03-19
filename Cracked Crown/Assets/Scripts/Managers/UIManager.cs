using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class UIManager : MonoBehaviour
{
    [SerializeField] Sprite bomb;
    [SerializeField] Sprite potion;
    [Header("Game Manager")]
    [SerializeField] GameManager GM;

    [Header("Player Info")]
    [SerializeField] PlayerController player1Controller;
    [SerializeField] PlayerController player2Controller;
    [SerializeField] PlayerController player3Controller;
    [SerializeField] PlayerController player4Controller;

    [SerializeField] PlayerBody player1Body;
    [SerializeField] PlayerBody player2Body;
    [SerializeField] PlayerBody player3Body;
    [SerializeField] PlayerBody player4Body;

    [Header("Main Menu")]
    [SerializeField] private GameObject MainMenu;

    [Header("In Game UI")]
    [SerializeField] private GameObject InGameUI;

    //Gold Rings For UI
    [SerializeField] private GameObject player1Ring;
    [SerializeField] private GameObject player2Ring;
    [SerializeField] private GameObject player3Ring;
    [SerializeField] private GameObject player4Ring;
    private float shineTimer = 0f;
    [SerializeField] private float maxRandShineTimer = 20f;
    private float player1ShineTimer = 0f;
    private float player2ShineTimer = 0f;
    private float player3ShineTimer = 0f;
    private float player4ShineTimer = 0f;

    [Header("Item Ring Images")]
    [SerializeField] private Image[] playerItems = new Image[4];

    [Header("Health Bars")]
    [SerializeField] private GameObject player1HealthBarObj;
    [SerializeField] private GameObject player2HealthBarObj;
    [SerializeField] private GameObject player3HealthBarObj;
    [SerializeField] private GameObject player4HealthBarObj;

    private Material[] playerHealthBars = new Material[4];

    [SerializeField] float animSpeed = 0.5f;
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

    [Header("Dialogue Bar")]
    [SerializeField] private TMP_Text DialogueBar;
    [SerializeField] private Image CharacterIcon;
    [SerializeField] private Sprite[] CharacterIcons;
    [SerializeField] private GameObject[] iconObjects;
    public enum characterIcons {Frog, Bunny, Duck, HoneyBadger };

    public static UIManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        GM = GameManager.Instance;
        
        //RecordPlayerHealths();
        //animSpeed = 1.0f / animSpeed;

        GetHealthBarMats();

    }
    private void Update()
    {

        if (GM.PMs != null)
        {
            //CheckPlayerHealths();
            for (int i = 0; i < GM.Players.Length; i++)
            {
                playerHealthBars[i].SetFloat("_Position", 1 - GM.PMs[i].PB.Health / 50);
            }


            for (int i = 0; i < GM.Players.Length; i++)
            {
                if (GM.PMs[i].PB.hasBomb)
                {
                    playerItems[i].color = new Color(1, 1, 1, 1);
                    playerItems[i].sprite = bomb;
                }
                else if (GM.PMs[i].PB.hasPotion)
                {
                    playerItems[i].color = new Color(1, 1, 1, 1);
                    playerItems[i].sprite = potion;
                }
                else if (!GM.PMs[i].PB.hasPotion && !GM.PMs[i].PB.hasBomb)
                {
                    playerItems[i].sprite = null;
                    playerItems[i].color = new Color(1, 1, 1, 0);
                }
            }

            for (int i = 0; i < GM.Players.Length; i++)
            {
                if (GM.PMs[i].PB.CharacterType.ID == 0)
                {
                    iconObjects[i].GetComponent<Image>().sprite = CharacterIcons[0];
                    iconObjects[i].GetComponent<Image>().SetNativeSize();

                }
                else if (GM.PMs[i].PB.CharacterType.ID == 1)
                {
                    iconObjects[i].GetComponent<Image>().sprite = CharacterIcons[1];
                    iconObjects[i].GetComponent<Image>().SetNativeSize();
                }
                else if (GM.PMs[i].PB.CharacterType.ID == 2)
                {
                    iconObjects[i].GetComponent<Image>().sprite = CharacterIcons[2];
                    iconObjects[i].GetComponent<Image>().SetNativeSize();
                }
                else if (GM.PMs[i].PB.CharacterType.ID == 3)
                {
                    iconObjects[i].GetComponent<Image>().sprite = CharacterIcons[3];
                    iconObjects[i].GetComponent<Image>().SetNativeSize();
                }
            }

            if (GM.PMs[0].PC.PauseDown)
            {
                if (PauseMenu.activeSelf == false)
                {
                    Pause();
                }
            }
        }

        
        //RecordPlayerHealths();
    }

    //Functions For Pause Menu (Copied from PauseMenu Script 1/30/2024)
    private void Pause()
    {
        PauseMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(pauseMenuInitButton);
        Time.timeScale = 0f;

        gamePaused = true;
    }

    public void Resume()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1f;

        gamePaused = false;
    }

    private void ClosePause()
    {
        PauseMenu.SetActive(false);
    }

    public void OpenSettings()
    {
        pauseMenuInitButton = EventSystem.current.currentSelectedGameObject;
        SettingsMenu.SetActive(true);
        ClosePause();
        EventSystem.current.SetSelectedGameObject(settingsInitButton);
    }

    public void CloseSettings()
    {
        SettingsMenu.SetActive(false);
        Pause();
        EventSystem.current.SetSelectedGameObject(pauseMenuInitButton);
    }

    //Settings Menu
    public void SetResolution()
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

    public void SetMasterVol()
    {
        Mixer.SetFloat("MasterVol", MasterSlider.value);
    }

    public void SetSFXVol()
    {
        Mixer.SetFloat("SFXVol", SFXSlider.value);
    }

    public void SetMusicVol()
    {
        Mixer.SetFloat("MusicVol", MusicSlider.value);
    }

    public void DisplayDialogue(string dialogue, characterIcons character)
    {
        DialogueBar.SetText(dialogue);
        CharacterIcon.sprite = CharacterIcons[((int)character)];
        CharacterIcon.SetNativeSize();
        CharacterIcon.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
    }

    private void GetHealthBarMats()
    {
        Debug.Log("getting Mats");
        playerHealthBars[0] = player1HealthBarObj.GetComponent<Image>().material;
        playerHealthBars[1] = player2HealthBarObj.GetComponent<Image>().material;
        playerHealthBars[2] = player3HealthBarObj.GetComponent<Image>().material;
        playerHealthBars[3] = player4HealthBarObj.GetComponent<Image>().material;
        Debug.Log("got mats");
    }  

   /* IEnumerator ModifyHealthBar(Material healthBar, float prevHealth, float currentHealth)
    {
        float 
        healthBar.SetFloat("_Position", Mathf.Lerp(prevHealth, currentHealth, ))
    }*/
    private void RecordPlayerHealths()
    {
        /*player1LastFrameHealth = player1Body.Health;
        player2LastFrameHealth = player2Body.Health;
        player3LastFrameHealth = player3Body.Health;
        player4LastFrameHealth = player4Body.Health;*/
    }
}
