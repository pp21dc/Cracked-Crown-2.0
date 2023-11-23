using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterType", menuName = "ScriptableObjects/CharacterTypes", order = 1)]
public class CharacterType : ScriptableObject
{
    public float moveSpeed;
    public float dashSpeed;
    public float dashTime;
    public float attack;
    public float attackSpeed;
}
