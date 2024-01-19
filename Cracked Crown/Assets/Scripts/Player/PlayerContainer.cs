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

    public PlayerAnimController PAC;

    private void FixedUpdate()
    {
        if (PAC == null)
        {
            if (transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).TryGetComponent<PlayerAnimController>(out PlayerAnimController Pac))
            {
                PAC = Pac;
            }
            else
            {
                PAC = null;
            }
        }
    }
}
