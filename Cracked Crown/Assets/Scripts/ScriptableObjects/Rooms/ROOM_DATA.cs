using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomData", menuName = "ScriptableObjects/RoomData")]
public class ROOM_DATA : ScriptableObject
{
    public int RoomNumber;
    public List<int> EnemyCount_PerWave;
    public List<int> EnemyCount_Light_PerWave;
    public List<int> EnemyCount_Medium_PerWave;
    public List<int> EnemyCount_Heavy_PerWave;
}
