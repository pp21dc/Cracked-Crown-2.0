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

    public void SpawnEnemy(int enemyType)
    {
        int randPoint = Random.Range(0, spawnPoints.Length);
        GameObject E = Instantiate(Enemy_prefab, spawnPoints[randPoint].position, Quaternion.identity);
        LM.EnemySpawned();
        if (enemyType == 0)
        {
            E.tag = "Light";
            E.transform.GetChild(0).tag = "Light";
        }
        else if (enemyType == 1)
        {
            E.tag = "Medium";
            E.transform.GetChild(1).tag = "Medium";
        }
        else if (enemyType == 2)
        {
            E.tag = "Heavy";
            E.transform.GetChild(2).tag = "Heavy";
        }
    }

}
