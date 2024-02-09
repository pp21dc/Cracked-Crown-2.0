using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        GameObject E = Instantiate(Enemy_prefab, spawnPoints[randPoint].position, Quaternion.identity);
        //SceneManager.MoveGameObjectToScene(E, LM.persScene);
        LM.EnemySpawned();
    }

}
