using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public LevelManager LM;
    [SerializeField]
    private Transform[] spawnPoints;
    [SerializeField]
    private GameObject Enemy_prefab;

    private void Awake()
    {
        LM = LevelManager.Instance;
    }

    public void SpawnEnemy()
    {
        int randPoint = Random.Range(0, spawnPoints.Length);
        Instantiate(Enemy_prefab, spawnPoints[randPoint].position, Quaternion.identity);
        LM.EnemySpawned();
    }

}
