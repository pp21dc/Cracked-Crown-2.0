using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterType", menuName = "ScriptableObjects/CharacterTypes", order = 1)]
public class CharacterType : ScriptableObject
{
    public int ID;
    public float moveSpeed;
    public float dashSpeed;
    public float dashTime;
    public float attack;
    public float attackSpeed;
    public float moveCd;
    public float attackDelayTime;
    public float attackKnockback;
    public float finisherRadius;
    public Vector3 executePosition;
    public GameObject corpse;
    public bool hop;
}
