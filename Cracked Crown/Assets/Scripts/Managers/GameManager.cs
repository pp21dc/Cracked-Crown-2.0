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

    public GameObject PIM;
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
    public string currentLevelName;
    public string MainMenuName;
    public string CutSceneName;
    public string BossLevelName;
    public string ShopName;

    [Header("UI")]
    public GameObject PlayerUI;
    [SerializeField]
    //private LoadingScreen loadingScreen;
    public GameObject GameOverScreen;
    public GameObject WinScreen;
    public GameObject MainMenu;
    public GameObject UI;
    public GameObject LoadingScreen;
   
    public bool CampaignStart = false;

    public bool Pause = false;
    public bool waitforvideo = true;
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
        Physics.IgnoreLayerCollision(7, 6);
        if (CampaignStart)
        {
            ReturnToMainMenu();
        }
        
        Application.targetFrameRate = 120;
    }
    public GameObject enem;
    bool locker;
    bool locker_Boss;
    private void FixedUpdate()
    {
        if (Input.GetKeyUp(KeyCode.KeypadEnter))
        {
            StartNewGame();
        }
        if (Input.GetKeyUp(KeyCode.H))
            SetPlayerPositions();
        if (!locker && Input.GetKey(KeyCode.M))
        {
            locker = true;
            ReturnToMainMenu();
        }
        if (!locker_Boss && Input.GetKey(KeyCode.B))
        {
            locker_Boss = true;
            LoadAScene("BossLevel");
        }

        if (!waitforvideo && currentLevelName == MainMenuName)
        {
            MainMenu.SetActive(true);
            PIM.SetActive(true);
        }
        else
        {
            
            MainMenu.SetActive(false);
        }
        if (currentLevelName == MainMenuName)
            UI.SetActive(false);
        else if (!string.IsNullOrEmpty(currentLevelName))
        {
            UI.SetActive(true);
        }
        
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

        if (!levelName.Equals(MainMenuName))
            LoadingScreen.gameObject.SetActive(true);
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
            SetPlayerPositions();
            yield return null;
        }


        SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelName));

        if (!levelName.Equals(MainMenuName) && !levelName.Equals(BossLevelName) && currentLevel < levelName.Length && !levelName.Equals(ShopName))
        {
            //AudioManager.LoadLevelComplete();
            Debug.Log("NEXT LEVEL");
            currentLevelName = levelNames[currentLevel];

            currentLevelName = levelName;
            IsLevelCleared = false;
            
            LM.Enter_Level(true);
            MainMenu.SetActive(false);
        }
        else if (levelName.Equals(MainMenuName))
        {
            waitforvideo = true;
            LM.ROOM_CLEARED = true;
            IsLevelCleared = true;
            currentLevelName = MainMenuName;
            //MainMenu.SetActive(true);
            currentLevel = -1;
        }
        else if (levelName.Equals(ShopName))
        {
            LM.ROOM_CLEARED = true;
            IsLevelCleared = true;
            currentLevelName = ShopName;
            LM.Enter_Level(false);
        }
        else if (levelName.Equals(BossLevelName))
        {
            LM.ROOM_CLEARED = true;
            IsLevelCleared = true;
            currentLevelName = BossLevelName;
            currentLevel = 5;
        }
        //yield return new WaitForSeconds(0.25f);
        //AudioManager.Instance.AudioFadeLevelStart();



        //PlayerUI.SetActive(false);
        // playerGO.SetActive(false);
        Time.timeScale = 1;
        LoadingScreen.gameObject.SetActive(false);
        locker = false;
        locker_Boss = false;
        //isLoading = false;

    }

    public void SetPlayerPositions()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            Players[i].PB.transform.localPosition = new Vector3(0, 0, 0);
            Players[i].PB.transform.position = spawnPoints[i].position;
            
            //Debug.Log(Players[i].PB.transform.position);
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
        UI.SetActive(true);
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
