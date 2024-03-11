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

    public AudioClip[] soundeffects;
    public string SFX_0;
    public string SFX_1;
    public string SFX_2;
    public string SFX_3;
    public string SFX_4;
    public string SFX_5;
    public string SFX_6;
}
