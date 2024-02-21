using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomData", menuName = "ScriptableObjects/RoomData")]
public class ROOM_DATA : ScriptableObject
{
    public int RoomNumber;
    public List<float> SpawnRate;
    public List<int> EnemyCount_PerWave;
    [Header("PERCENT CHANCE OF SPAWNING PER WAVE")]
    [Tooltip("This is the percentage chance of it spawning this wave")]
    public List<int> EnemyCount_Light_PerWave;
    public List<int> EnemyCount_Medium_PerWave;
    public List<int> EnemyCount_Heavy_PerWave;
}
