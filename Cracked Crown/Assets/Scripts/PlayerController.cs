using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    /**
     * This is all standard stuff from our Unity fundementals course
     */

    private float forwardMagnitude;
    public float ForwardMagnitude { get { return forwardMagnitude; } }

    private float horizontalMagnitude;
    public float HorizontalMagnitude { get { return horizontalMagnitude; } }

    private bool primaryAttackDown = false;
    public bool PrimaryAttackDown { get { return primaryAttackDown; } }

    private bool executeDown = false;
    public bool ExecuteDown { get { return executeDown; } }

    private void LateUpdate()
    {
        primaryAttackDown = false;
        executeDown = false;
    }

    public void OnMove(InputValue inputValue)
    {
        Vector2 inputVal = inputValue.Get<Vector2>();

        forwardMagnitude = inputVal.y;
        horizontalMagnitude = inputVal.x;
    }

    public void OnPrimaryAttack()
    {
        primaryAttackDown = true;
    }

    public void OnExecute()
    {
        executeDown = true;
    }
}
