using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public GameManager GM;
    private static LevelManager instance;
    public static LevelManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LevelManager>();
            }
            if (!instance)
            {
                return null;
                //Debug.LogError("ERROR: NO LEVEL MANAGER PRESENT");
            }

            return instance;
        }
    }

    public bool SpawnersActive;
    public bool PausePlayers;
    [Tooltip("Time Until Initial Room Wave Begins")]
    public float WAIT_ONENTER;
    [Tooltip("Time Until Next Wave of Enemies")]
    public float WAIT_NEXTROUND;
    [Tooltip("Longest Time For Next Enemy Spawn")]
    public float WAIT_NEXTSPAWN_UPB;
    [Tooltip("Shortest Time For Next Enemy Spawn")]
    public float WAIT_NEXTSPAWN_LWB;
    public float WAIT_NEXTSPAWN_VALUE = 0.2f;

    [SerializeField]
    private int CURRENT_WAVE;
    [SerializeField]
    public int CURRENT_ROOM;
    [SerializeField]
    public bool ROOM_CLEARED;
    [SerializeField]
    private int WAVES = 3;

    [Header("Room Info")]
    [SerializeField]
    private List<ROOM_DATA> Rooms;
    [SerializeField]
    private ROOM_DATA Current_Room;
    [SerializeField]
    private Spawner Spawner;
    [SerializeField]
    private int ENEMIES_SPAWNED;
    [SerializeField]
    private int ENEMIES_KILLED;

    [HideInInspector]
    public Scene persScene;

    private void Awake()
    {
        GM = GameManager.Instance;
        persScene = SceneManager.GetSceneByBuildIndex(0);
    }
    public float SpawnTimer = 0;
    public float currentSpawnTotal = 0;
    public bool loc = false;
    bool boss = false;

    private void Update()
    {
        if (boss)
        {
            loc = false;
        }

        if (Input.GetKeyUp(KeyCode.J))
            loc = !loc;
        if (!loc)
            EnemySpawnSystem();
        if (Input.GetKey(KeyCode.H))
            GM.SetPlayerPositions();
        if (GM.currentLevelName == GM.MainMenuName)
            ResetLevelManager();
    }

    public void ResetLevelManager()
    {
        StopAllCoroutines();
        SpawnTimer = 0;
        SpawnersActive = false;
        CURRENT_ROOM = 0;
        CURRENT_WAVE = 0;
        ENEMIES_KILLED = 0;
        ENEMIES_SPAWNED = 0;
        Current_Room = Rooms[0];
        ROOM_CLEARED = true;
        SpawnersActive = false;
        loc = false;
    }

    private void EnemySpawnSystem()
    {
        if (ROOM_CLEARED || !SpawnersActive)
            return;
        //Debug.Log(Current_Room.RoomNumber + ": " + CURRENT_WAVE);
        if (ENEMIES_SPAWNED >= currentSpawnTotal && !boss)
        {
            if (ENEMIES_KILLED >= ENEMIES_SPAWNED)
            {
                StartCoroutine(ON_ROUNDEND());
                return;
            }
        }

        if (boss)
        {
            loc = false;
        }

        SpawnTimer += Time.deltaTime;
        if (SpawnTimer > WAIT_NEXTSPAWN_VALUE && ENEMIES_SPAWNED < currentSpawnTotal)
        {
            SpawnTimer = 0;
            if (!boss)
                WAIT_NEXTSPAWN_VALUE = Current_Room.SpawnRate[CURRENT_WAVE - 1] * Random.Range(0.8f, 1f);
            else
                WAIT_NEXTSPAWN_VALUE = (1 / (GM.Players.Length + 1)) + 0.8f;
            Spawner.SpawnEnemy(PickEnemy());
        }
    }

    

    public int PickEnemy()
    {
        if (Random.Range(0, 100) < Current_Room.EnemyCount_Light_PerWave[CURRENT_WAVE - 1])
            return 0;
        if (Random.Range(0, 100) < Current_Room.EnemyCount_Medium_PerWave[CURRENT_WAVE - 1])
            return 1;
        if (Random.Range(0, 100) < Current_Room.EnemyCount_Heavy_PerWave[CURRENT_WAVE - 1])
            return 2;
        return 1;
    }

    public void Enter_Level(bool hostile, bool boss)
    {
        CURRENT_WAVE = 1;
        //GM.SetPlayerPositions();
        this.boss = boss;
        StartCoroutine(ON_ENTER(hostile, boss));
    }

    public void EnemySpawned()
    {
        ENEMIES_SPAWNED += 1;
    }

    public void EnemyKilled()
    {
        ENEMIES_KILLED += 1;
    }

    private IEnumerator ON_ENTER(bool hostile, bool boss)
    {
        CURRENT_WAVE = 1;
        ROOM_CLEARED = !hostile;
        yield return new WaitForSeconds(WAIT_ONENTER);
        //GM.SetPlayerPositions();
        
        if (hostile)
        {
            CURRENT_WAVE = 1;
            if (CURRENT_ROOM < 3)
                CURRENT_ROOM += 1;
            Current_Room = Rooms[CURRENT_ROOM - 1];
            currentSpawnTotal = Current_Room.EnemyCount_PerWave[0] * (GM.Players.Length*0.25f) + 1;
            Debug.Log(currentSpawnTotal);
            SpawnTimer = 0;
            WAIT_NEXTSPAWN_VALUE = 0.2f;
            ENEMIES_SPAWNED = 0;
            ENEMIES_KILLED = 0;
            this.boss = boss;

            if (!boss)
                SpawnersActive = true;
            else
            {
                
                currentSpawnTotal += 999;
                CURRENT_ROOM = 3;
                Current_Room = Rooms[CURRENT_ROOM - 1];
                WAIT_NEXTSPAWN_VALUE = 0.2f;
            }
        }
        yield return null;
    }

    private IEnumerator ON_ROUNDEND()
    {
        SpawnersActive = false;
        if (CURRENT_WAVE < 3)
        {
            currentSpawnTotal = Current_Room.EnemyCount_PerWave[CURRENT_WAVE] * (GM.Players.Length * 0.25f) + 1;
            Debug.Log("Current Total RE: " + currentSpawnTotal);
            yield return new WaitForSeconds(WAIT_NEXTROUND);
            CURRENT_WAVE++;
            ENEMIES_SPAWNED = 0;
            ENEMIES_KILLED = 0;
            SpawnersActive = true;

        }
        else
        {
            ROOM_CLEARED = true;
            GM.IsLevelCleared = true;
        }
        //Debug.Log("ROUND END");
    }









}
