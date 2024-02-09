using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                Debug.LogError("ERROR: NO GAME MANAGER PRESENT");
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
    private float WAIT_NEXTSPAWN_VALUE = 1;

    [SerializeField]
    private int CURRENT_WAVE;
    [SerializeField]
    private int CURRENT_ROOM;
    [SerializeField]
    private bool ROOM_CLEARED;
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


    private void Awake()
    {
        GM = GameManager.Instance;
    }
    float SpawnTimer = 0;
    bool loc;
    private void Update()
    {
        EnemySpawnSystem();
        if (!loc)
        {
            loc = true;
            StartCoroutine(HOLDSTILLCUN());
        }
    }

    private IEnumerator HOLDSTILLCUN()
    {
        GM.SetPlayerPositions();
        yield return new WaitForSeconds(7);
        GM.SetPlayerPositions();
    }

    private void EnemySpawnSystem()
    {
        if (ROOM_CLEARED || !SpawnersActive)
            return;
        if (ENEMIES_SPAWNED > Current_Room.EnemyCount_PerWave[CURRENT_WAVE-1])
        {
            StartCoroutine(ON_ROUNDEND());
            return;
        }

        SpawnTimer += Time.deltaTime;
        if (SpawnTimer > WAIT_NEXTSPAWN_VALUE)
        {
            SpawnTimer = 0;
            WAIT_NEXTSPAWN_VALUE = Random.Range(WAIT_NEXTSPAWN_LWB, WAIT_NEXTSPAWN_UPB);
            Spawner.SpawnEnemy();
        }
    }

    public void Enter_Level()
    {
        StartCoroutine(ON_ENTER());
    }

    public void EnemySpawned()
    {
        ENEMIES_SPAWNED += 1;
    }

    private IEnumerator ON_ENTER()
    {
        CURRENT_WAVE = 0;
        ROOM_CLEARED = false;
        yield return new WaitForSeconds(WAIT_ONENTER);
        
        CURRENT_WAVE = 1;
        CURRENT_ROOM += 1;
        Current_Room = Rooms[CURRENT_ROOM];
        SpawnersActive = true;
    }

    private IEnumerator ON_ROUNDEND()
    {
        SpawnersActive = false;
        if (CURRENT_WAVE <= 3)
        {
            yield return new WaitForSeconds(WAIT_NEXTROUND);
            CURRENT_WAVE++;
            SpawnersActive = true;

        }
        else
        {
            ROOM_CLEARED = true;
            GM.IsLevelCleared = true;
        }
    }









}
