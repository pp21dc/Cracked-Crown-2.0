using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerContainer : MonoBehaviour
{
    [SerializeField]
    private PlayerController pc;
    public PlayerController PC { get { return pc; } }

    [SerializeField]
    private PlayerInput pi;
    public PlayerInput PI { get { return pi; } }

    [SerializeField]
    private PlayerBody pb;
    public PlayerBody PB { get { return pb; } }
}
