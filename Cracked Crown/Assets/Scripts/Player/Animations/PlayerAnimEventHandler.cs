using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimEventHandler : MonoBehaviour
{
    public PlayerContainer PC;

    private void Awake()
    {
        PC = transform.parent.parent.parent.parent.GetComponent<PlayerContainer>();
    }

    public void AttackFinish()
    {
        PC.PAC.Attacking = false;
    }
}
