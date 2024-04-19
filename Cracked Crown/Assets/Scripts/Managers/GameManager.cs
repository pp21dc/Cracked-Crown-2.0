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
using System.Threading;
using UnityEditor;
using Unity.VisualScripting;
//using System.Diagnostics;

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
    public List<BossPhases> claws;
    public CameraShake css;

    [SerializeField]
    OpeningVideoController video_lose;
    [SerializeField]
    OpeningVideoController video_win;

    [SerializeField]
    public Transform cam;

    [SerializeField]
    public string[] levelNames;
    public bool isLoading = false;
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
    //public GameObject LoadingScreen;
   
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

    [SerializeField]
    private int loadCount = -1;

    [SerializeField]
    private GameObject[] LoadingScreen;

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
        claws = new List<BossPhases>();
        Physics.IgnoreLayerCollision(7, 6);
        Physics.IgnoreLayerCollision(7, 3);
        Physics.IgnoreLayerCollision(7, 9);
        if (CampaignStart)
        {
            ReturnToMainMenu(true);
        }
        css = CameraShake.instance;
        Application.targetFrameRate = 60;
    }
    public GameObject enem;
    bool locker;
    bool locker_Boss;
    private void FixedUpdate()
    {
        if (!TEST_SCENE)
        {
            int x = 0;
            if (claws != null)
            {
                for (int i = 0; i < claws.Count; i++)
                {
                    if (claws[i].isDead())
                        x++;
                }
                if ((x >= 2 && Players.Length < 1) || (x >= 2 && Players.Length >= 1))
                {
                    Debug.Log("WIN");
                    claws = new List<BossPhases>();
                    win = true;

                }
            }

            if (eyeText != null)
                eyeText.text = eyeCount.ToString();
            if (Input.GetKeyUp(KeyCode.H))
                SetPlayerPositions();
            if (!locker && Input.GetKeyUp(KeyCode.M))
            {
                locker = true;
                ReturnToMainMenu(false);
            }
            if (!locker_Boss && Input.GetKeyUp(KeyCode.B))
            {
                locker_Boss = true;
                //Debug.Break();
                LoadAScene(BossLevelName);
            }

            if (!waitforvideo && currentLevelName == MainMenuName)
            {
                //MainMenu.SetActive(true);
                UIManager.Instance.InGameUI.SetActive(false);
                PIM.SetActive(true);
            }
            else if (!string.IsNullOrEmpty(currentLevelName))
            {
                UI.SetActive(true);
            }

            if (!loseLock && AreAllPlayersDead() && lost && !waitforvideo && !currentLevelName.Equals(MainMenuName))
            {
                StartCoroutine(LoseCond());
            }
            else
            {
                lost = false;
            }
            if (!winLock && win && !waitforvideo && !currentLevelName.Equals(MainMenuName))
            {
                winLock = true;
                StartCoroutine(WINGAME());
            }
        }
    }

    public bool winLock = false;
    public bool loseLock = false;

    IEnumerator WINGAME()
    {
        yield return new WaitForSeconds(10.5f);
        eyeCount = 0;
        video_win.stopAudio = false;
        video_win.PlayVideo();
        MM.PlayTrack(MusicManager.TrackTypes.loading);
        RevivePlayers();
        SetPlayerPositions();
        star = true;
        SetPlayerScore();
        ReturnToMainMenu(true);
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


    public bool lost = false;
    public bool AreAllPlayersDead()
    {
        //SkillIssue
        int x = 0;
        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i].PB.alreadyDead && !Players[i].PB.reving && !waitforvideo)
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
            Players[i].PB.ResetPlayer(true);
            SetPlayerPositions();
        }
        lost = false;
    }

    private void SetPlayerScore()
    {
        foreach(PlayerContainer pb in Players)
        {
            if (pb.PB.CharacterType.ID == 1)
                ScoreBoardManager.instance.SetBunnyStats(pb.PB.scoreboard.CreateString());
            else if (pb.PB.CharacterType.ID == 2)
                ScoreBoardManager.instance.SetDuckStats(pb.PB.scoreboard.CreateString());
            else if (pb.PB.CharacterType.ID == 3)
                ScoreBoardManager.instance.SetFrogStats(pb.PB.scoreboard.CreateString());
            else if (pb.PB.CharacterType.ID == 0)
                ScoreBoardManager.instance.SetBadgerStats(pb.PB.scoreboard.CreateString());
        }
    }

    IEnumerator LoseCond()
    {
        yield return new WaitForSeconds(10);
        if (AreAllPlayersDead() && Players[0] != null && !waitforvideo)
        {
            eyeCount = 0;
            video_lose.stopAudio = false;
            video_lose.PlayVideo();
            MM.PlayTrack(MusicManager.TrackTypes.loading);
            RevivePlayers();
            SetPlayerPositions();
            star = true;
            SetPlayerScore();
            ReturnToMainMenu(true);
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
            {
                pc.PB.playerLock = freeze;
                pc.PB.StopPlayer(freeze);
            }
        }
    }

    public void ResetPlayers(bool main)
    {
        foreach (PlayerContainer pc in Players)
        {
            PlayerBody pb = pc.PB;
            //Debug.Log(pb.CharacterType.name);
            if (pb != null)
            {
                //pb.StopAllCoroutines();
                pb.rb.isKinematic = false;
                pb.canMove = true;
                pb.canCollect = true;
                pb.canAttack = true;
                pb.canCollectBomb = false;
                pb.canCollectPotion = false;
                pb.canExecute = true;
                pb.canMovePlayerForexecute = false;
                pb.canTakeDamage = true;
                pb.canUseItem = true;
                pb.gotHit = false;
                pb.timesHit = 0;
                pb.timesSwung = 0;
                pb.timer_ifswung = 0;
                pb.attackImpLock = false;
                pb.lockDash = false;
                pb.dashing = false;
                pb.dashQueue = false;
                pb.dashOnCD = false;
                gameObject.tag = "Player";
                if (main)
                {
                    pb.ghostCoins = 0;
                    pb.health = pb.maxHealth;
                    //pb.alreadyDead = true;
                    pb.playerLock = false;
                }
                if (pb.alreadyDead && !main)
                {
                    pb.canAttack = false;
                    pb.canTakeDamage = false;
                    pb.canUseItem = false;
                    pb.lockDash = true;
                    gameObject.tag = "Ghost";
                }
                else
                {
                    pb.animController.SetAll();
                    pb.alreadyDead = false;
                }
            }
        }
    }

    public void ResetPlayer(PlayerBody pb)
    {
        if (pb != null)
        {
            //Debug.Log("RESETING PLAYER");
            pb.playerLock = false;
            pb.canMove = true;
            pb.canCollect = true;
            pb.canAttack = true;
            pb.canCollectBomb = false;
            pb.canCollectPotion = false;
            pb.reving = false;
            pb.timesSwung = 0;
            pb.timer_ifswung = 0;
            pb.attackImpLock = false;
            pb.canExecute = true;
            pb.canMovePlayerForexecute = false;
            pb.canTakeDamage = true;
            pb.canUseItem = true;
            pb.gotHit = false;
            pb.timesHit = 0;
            pb.lockDash = false;
            pb.Grabbed = false;
            pb.ghostCoins = 0;
            pb.dashing = false;
            pb.dashOnCD = false;
            pb.dashQueue = false;
            if (pb.alreadyDead)
            {
                pb.canAttack = false;
                pb.canExecute = false;
                pb.canTakeDamage = false;
                pb.canUseItem = false;
                pb.lockDash = true;
            }
        }
    }
    bool star;
    public float progress = 0;
    private IEnumerator LoadLevel(string levelName)
    {
        isLoading = true;
        SetPlayerPositions();
        if (UIManager.Instance != null)
            UIManager.Instance.InGameUI.SetActive(false);
        if ((!levelName.Equals(MainMenuName) || star) && !waitforvideo)
        {
            MM.PlayTrack(MusicManager.TrackTypes.loading);
            LoadingScreen[loadCount + 1].SetActive(true);
            MainMenu.SetActive(false);
        }
        else
        {
            //Debug.Log("ON");
            if (!waitforvideo)
                MainMenu.SetActive(true);
            else
                MainMenu.SetActive(false);
            
            LoadingScreen[loadCount + 1].SetActive(false);
        }
        FreezePlayers(true);
        ResetPlayers(levelName.Equals(MainMenuName));
        

        if ((!string.IsNullOrEmpty(currentLevelName)))
        {
            //Debug.Log(currentLevelName);
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(currentLevelName);
            //yield return AudioManager.Instance.UnloadLevel();
            while (!asyncUnload.isDone)
            {
                
                progress = asyncUnload.progress;
                yield return null;
            }

        }
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        //AudioManager.Instance.AudioFadeLevelStart();
        Debug.Log("WEEEE");
        while (!asyncLoad.isDone)
        {
            progress = asyncLoad.progress;
            yield return null;
        }
        SetPlayerPositions();
        FreezePlayers(true);
        foreach (PlayerContainer pc in Players)
        {
            pc.PB.EnterLevel();
        }
        if (levelName.Equals(MainMenuName))
            RevivePlayers();

        if (!waitforvideo)
            yield return new WaitForSeconds(5.25f);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelName));
        if (!levelName.Equals(MainMenuName) && !levelName.Equals(BossLevelName) && currentLevel <= levelNames.Length &&
            (!levelName.Equals("TempShop") && !levelName.Equals("GreenShop") && !levelName.Equals("PurpleShop") && !levelName.Equals("RedShop")))
        {
            MM.PlayTrack((MusicManager.TrackTypes)LM.CURRENT_ROOM + 4);
            //SetPlayerPositions();
            currentLevelName = levelNames[currentLevel];

            currentLevelName = levelName;
            IsLevelCleared = false;

            LM.Enter_Level(true, false);
            MainMenu.SetActive(false);
            UIManager.Instance.InGameUI.SetActive(true);
        }
        else if (levelName.Equals(MainMenuName))
        {
            //SetPlayerPositions();
            if (star && !waitforvideo)
            {
                MainMenu.SetActive(true);
                MM.PlayTrack(MusicManager.TrackTypes.windy);
            }
            
            star = false;
            LM.ROOM_CLEARED = true;
            IsLevelCleared = true;
            currentLevelName = MainMenuName;
            UIManager.Instance.InGameUI.SetActive(false);
            
            currentLevel = -1;
        }
        else if (levelName.Equals(ShopName) || levelName.Equals("GreenShop") || levelName.Equals("RedShop") || levelName.Equals("PurpleShop"))
        {
            //MM.PlayNextTrack();
            LM.ROOM_CLEARED = true;
            IsLevelCleared = true;
            //currentLevelName = ShopName;
            if (levelName.Equals("GreenShop"))
                currentLevelName = "GreenShop";
            else if (levelName.Equals("PurpleShop"))
                currentLevelName = "PurpleShop";
            else if (levelName.Equals("RedShop"))
                currentLevelName = "RedShop";
            UIManager.Instance.InGameUI.SetActive(true);
            MM.PlayTrack(MusicManager.TrackTypes.shop);
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
            MM.PlayTrack(MusicManager.TrackTypes.boss);
        }
        //yield return new WaitForSeconds(0.25f);
        //AudioManager.Instance.AudioFadeLevelStart();

        if (!star)
        {
            star = true;
        }


        //PlayerUI.SetActive(false);
        // playerGO.SetActive(false);
        Time.timeScale = 1;
        LoadingScreen[loadCount+1].SetActive(false);
        locker = false;

        locker_Boss = false;
        //isLoading = false;
        
        if (!waitforvideo)
            FreezePlayers(false);
        SetPlayerPositions();

        if (!levelName.Equals(MainMenuName)/* || !levelName.Equals("TempShop")*/)
        {
            if (!levelName.Equals("TempShop") || !levelName.Equals("GreenShop") || !levelName.Equals("PurpleShop") || !levelName.Equals("RedShop"))
            {
                //Debug.LogWarning(levelName);
                dialogue.GetPlayers();
                dialogue.SetDialogue();
            }
            
        }
        loadCount++;
        isLoading = false;
    }

    public void SetPlayerPositions()
    {
        //Debug.Log("SET POS");
        for (int i = 0; i < Players.Length; i++)
        {
            Players[i].PB.playerLock = false;
            Players[i].PB.EnterLevel();
            Players[i].PB.transform.localPosition = new Vector3(0, 0, 0);
            Players[i].PB.transform.position = spawnPoints[i].position;
            if (Players[i].PB.spriteRenderer != null)
                Players[i].PB.spriteRenderer.color = new Color(1, 1, 1, 1);
            //Debug.Log(Players[i].PB.transform.position);
        }
        if (spawnPoints.Length <= 0)
            Debug.Log("GAMEMANAGER:: NO SPAWN POINTS SET FOR PLAYERS ON LEVEL CHANGE // GameManager/SetPlayerPositions");
    }

    public void ReturnToMainMenu(bool cond)
    {
        if (star)
            loadCount = 0;
        loadCount = -1;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.Resume();
            UIManager.Instance.InGameUI.SetActive(false);
        }
        //locker = false;
        waitforvideo = cond;
        ResetGame(true);
        if (!isLoading)
            StartCoroutine("LoadLevel", MainMenuName);
        isLoading = true;
        FreezePlayers(true);
        //currentLevelName = MainMenuName;
        dialogue.ResetDialoguePlayerList();
    }

    private void ResetGame(bool ifNotToMainMenu)
    {
        LM.ResetLevelManager();
        //SetPlayerPositions();
        win = false;
        lost = false;
        winLock = false;
        loseLock = false;
        eyeCount = 0;
        eyeText.text = "";

       
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
        waitforvideo = false;
        star = false;
        currentLevel = 5;
        //currentLevelName = MainMenuName;
        //StopAllCoroutines();
        if (!isLoading)
            StartCoroutine("LoadLevel", sceneName);
    }

}
