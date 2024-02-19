using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            if (!instance)
            {
                Debug.LogError("ERROR: NO GAME MANAGER PRESENT");
            }

            return instance;
        }
    }
    private LevelManager LM;

    public PlayerContainer[] Players;
    public PlayerManager[] PMs;
    public GameObject[] Characters;

    [SerializeField]
    private string[] levelNames;
    private bool isLoading = false;
    private bool LevelCleared = false;
    public bool IsLevelCleared
    {
        get { return LevelCleared; }
        set { LevelCleared = value; }
    }
    private int currentLevel = 0; //index of levelNames
    public int CurrentLevel
    {
        get { return currentLevel; }
        set { currentLevel = value; }
    }
    [Header("SceneInfo")]
    private string currentLevelName;
    public string MainMenuName;
    public string CutSceneName;
    public string BossLevelName;

    [Header("UI")]
    public GameObject PlayerUI;
    [SerializeField]
    //private LoadingScreen loadingScreen;
    public GameObject GameOverScreen;
    public GameObject WinScreen;
    public GameObject MainMenu;
    public bool CampaignStart = false;

    public bool Pause = false;

    public float eyeCount = 0;

    [SerializeField]
    private Transform[] spawnPoints;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        LM = LevelManager.Instance;
    }

    private void Start()
    {
        if (CampaignStart)
        {
            ReturnToMainMenu();
        }
        Application.targetFrameRate = 120;
        //Screen.SetResolution(1920, 1080, true, 120);
        Debug.Log(Screen.currentResolution);
    }
    public GameObject enem;
    private void FixedUpdate()
    {
        if (Input.GetKeyUp(KeyCode.KeypadEnter))
        {
            StartNewGame();
        }
        if (Input.GetKeyUp(KeyCode.N))
        {
            Debug.Log(currentLevel);
            NextLevel();
        }
        if (Input.GetKeyUp(KeyCode.H))
            SetPlayerPositions();

    }

    public PlayerInput GetPlayer(int ID)
    {
        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[ID].PI.playerIndex == ID)
            {
                return Players[ID].PI;
            }
        }
        return null;
    }

    private IEnumerator LoadLevel(string levelName)
    {
        isLoading = true;

        //loadingScreen.gameObject.SetActive(true);
        //yield return new WaitForSeconds(0.25f);

        if ((!string.IsNullOrEmpty(currentLevelName)))
        {
            //Debug.Log(currentLevelName);
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(currentLevelName);
            //yield return AudioManager.Instance.UnloadLevel();
            while (!asyncUnload.isDone)
            {
                //loadingScreen.UpdateSlider(asyncUnload.progress / 2);
                yield return null;
            }

        }

        yield return new WaitForSeconds(0.5f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        //AudioManager.Instance.AudioFadeLevelStart();
        while (!asyncLoad.isDone)
        {
            //loadingScreen.UpdateSlider(loadingScreen.loadingSlider.value + asyncLoad.progress / 2);
            yield return null;
        }


        SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelName));

        if (!levelName.Equals(MainMenuName) && currentLevel < levelName.Length)
        {
            //AudioManager.LoadLevelComplete();
            //Debug.Log(currentLevel);
            currentLevelName = levelNames[currentLevel];

            currentLevelName = levelName;
            IsLevelCleared = false;
            SetPlayerPositions();
            LM.Enter_Level();
            MainMenu.SetActive(false);
        }
        else if (levelName.Equals(MainMenuName))
        {
            LM.ROOM_CLEARED = true;
            IsLevelCleared = true;
            currentLevelName = MainMenuName;
            MainMenu.SetActive(true);
            currentLevel = -1;
        }

        //yield return new WaitForSeconds(0.25f);
        //AudioManager.Instance.AudioFadeLevelStart();

        

        //PlayerUI.SetActive(false);
       // playerGO.SetActive(false);
        Time.timeScale = 1;
        //loadingScreen.gameObject.SetActive(false);

        //isLoading = false;
        
    }

    public void SetPlayerPositions()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            
            Players[i].transform.position = spawnPoints[i].position;
            Players[i].PB.transform.localPosition = new Vector3(0,0,0);
            //Debug.Log(Players[i].PB.transform.localPosition);
        }
        if (spawnPoints.Length <= 0)
            Debug.Log("GAMEMANAGER:: NO SPAWN POINTS SET FOR PLAYERS ON LEVEL CHANGE // GameManager/SetPlayerPositions");
    }

    public void ReturnToMainMenu()
    {
        //playerGO.SetActive(false);
        
        StartCoroutine("LoadLevel", MainMenuName);
        //currentLevelName = MainMenuName;
    }

    public void NextLevel()
    {
        currentLevel++;
        if (currentLevel < levelNames.Length)
        {
            
            StartCoroutine("LoadLevel", levelNames[currentLevel]);
        }
        else
        {
            StartCoroutine("LoadLevel", BossLevelName);
            //SceneManager.LoadScene("MainMenu");
        }
        //PM.ResetPlayer();
        //instance.SaveGame();
    }

    public void StartNewGame()
    {
        /*if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }*/
        currentLevel = 0;

        StartCoroutine("LoadLevel", levelNames[currentLevel]);
        //instance.SaveGame();
    }

    public void LoadAScene(string sceneName)
    {
        StartCoroutine("LoadLevel", sceneName);
    }

}
