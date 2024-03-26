using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
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
                return null;
                //Debug.LogError("ERROR: NO GAME MANAGER PRESENT");
            }

            return instance;
        }
    }
    private LevelManager LM;
    private MusicManager MM;
    public GameObject PIM;
    public PlayerContainer[] Players;
    public PlayerManager[] PMs;
    public GameObject[] Characters;

    [SerializeField]
    OpeningVideoController video_lose;
    [SerializeField]
    OpeningVideoController video_win;

    [SerializeField]
    public Transform cam;

    [SerializeField]
    public string[] levelNames;
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
    public bool win = false;
    public bool Pause = false;
    public bool waitforvideo = true;
    public float eyeCount = 0;
    public TextMeshProUGUI eyeText;

    public bool TEST_SCENE = false;

    [SerializeField]
    private Transform[] spawnPoints;

    [SerializeField]
    private Dialogue dialogue;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        LM = LevelManager.Instance;
        MM = MusicManager.instance;
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
        if (!TEST_SCENE)
        {
            if (eyeText != null)
                eyeText.text = eyeCount.ToString();
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
                UIManager.Instance.InGameUI.SetActive(true);
                PIM.SetActive(true);
            }
            else if (MainMenu != null)
            {

                MainMenu.SetActive(false);
            }
            if (currentLevelName == MainMenuName) { }
                //UI.SetActive(false);
            else if (!string.IsNullOrEmpty(currentLevelName))
            {
                UI.SetActive(true);
            }

            if (!lost && AreAllPlayersDead())
                StartCoroutine(LoseCond());
            if (win)
            {
                win = false;
                video_win.PlayVideo();
                RevivePlayers();
                ReturnToMainMenu();
            }
        }
    }

    /*public void E()
    {
        if (roarSpawn)
        {
            LevelManager.Instance.SpawnersActive = true;
        }
        else
        {
            LevelManager.Instance.SpawnersActive = false;
        }

        if (bossDead)
        {
            GameManager.Instance.win = true;
        }

    }*/


    bool lost = false;
    public bool AreAllPlayersDead()
    {
        //SkillIssue
        int x = 0;
        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i].PB.alreadyDead)
            {
                x++;
            }
        }
        if (x == Players.Length && Players.Length != 0)
        {
            lost = true;
            return true;
        }
        else
        {
            lost = false;
            return false;
        }
    }

    public void RevivePlayers()
    {
        for(int i = 0; i < Players.Length; i++)
        {
            Players[i].PB.ResetPlayer();
            SetPlayerPositions();
        }
        lost = false;
    }

    IEnumerator LoseCond()
    {
        yield return new WaitForSeconds(5);
        if (AreAllPlayersDead() && Players[0] != null && !waitforvideo)
        {
            
            video_lose.PlayVideo();
            RevivePlayers();
            
            ReturnToMainMenu();
        }
        else
        {
            yield return null;
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

    public void FreezePlayers(bool freeze)
    {
        foreach (PlayerContainer pc in Players)
        {
            if (pc.PB != null)
                pc.PB.playerLock = freeze;
        }
    }

    private IEnumerator LoadLevel(string levelName)
    {
        isLoading = true;
        if (UIManager.Instance != null)
            UIManager.Instance.InGameUI.SetActive(false);
        if (!levelName.Equals(MainMenuName))
            LoadingScreen.gameObject.SetActive(true);
        MM.PlayNextTrack();
        SetPlayerPositions();
        yield return new WaitForSeconds(0.25f);
        

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
        SetPlayerPositions();
        yield return new WaitForSeconds(0.25f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        //AudioManager.Instance.AudioFadeLevelStart();
        while (!asyncLoad.isDone)
        {
            SetPlayerPositions();
            yield return null;
        }


        SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelName));
        if (!levelName.Equals(MainMenuName) && !levelName.Equals(BossLevelName) && currentLevel <= levelNames.Length && !levelName.Equals(ShopName))
        {
            //AudioManager.LoadLevelComplete();
            //Debug.Log("NEXT LEVEL");
            SetPlayerPositions();
            currentLevelName = levelNames[currentLevel];

            currentLevelName = levelName;
            IsLevelCleared = false;
            
            LM.Enter_Level(true, false);
            MainMenu.SetActive(false);
            UIManager.Instance.InGameUI.SetActive(true);
        }
        else if (levelName.Equals(MainMenuName))
        {
            waitforvideo = true;
            LM.ROOM_CLEARED = true;
            IsLevelCleared = true;
            currentLevelName = MainMenuName;
            //
            currentLevel = -1;
        }
        else if (levelName.Equals(ShopName))
        {
            //MM.PlayNextTrack();
            LM.ROOM_CLEARED = true;
            IsLevelCleared = true;
            currentLevelName = ShopName;
            UIManager.Instance.InGameUI.SetActive(true);
            LM.Enter_Level(false, false);
        }
        else if (levelName.Equals(BossLevelName))
        {
            LM.ROOM_CLEARED = false;
            IsLevelCleared = false;
            currentLevelName = BossLevelName;
            UIManager.Instance.InGameUI.SetActive(true);
            LM.Enter_Level(true, true);
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
        SetPlayerPositions();
        FreezePlayers(false);

        if (!levelName.Equals(MainMenuName))
        {
            dialogue.GetPlayers();
        }
            dialogue.SetDialogue();
            Debug.Log("Called SetDialouge");

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
        if (UIManager.Instance != null)
        {
            LoadingScreen.SetActive(true);
            UIManager.Instance.Resume();
            UIManager.Instance.InGameUI.SetActive(false);
        }
        //locker = false;
        ResetGame(true);
        StartCoroutine("LoadLevel", MainMenuName);
        FreezePlayers(true);
        //currentLevelName = MainMenuName;
    }

    private void ResetGame(bool ifNotToMainMenu)
    {
        LM.ResetLevelManager();
        SetPlayerPositions();
        RevivePlayers();
        eyeCount = 0;
        eyeText.text = "";

        MM.trackIndex = -1;
        if (!ifNotToMainMenu)
            ReturnToMainMenu();
    }

    public void NextLevel()
    {
        currentLevel++;
        UI.SetActive(true);
        if (currentLevel < levelNames.Length)
        {
            //Debug.Log("LEVEL: " + currentLevel);
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
