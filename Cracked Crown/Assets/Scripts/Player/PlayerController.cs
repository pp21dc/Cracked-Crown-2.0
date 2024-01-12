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

    private bool dashDown = false;
    public bool DashDown { get { return dashDown; } }

    private bool pauseDown = false;
    public bool PauseDown { get { return pauseDown; } }


    private bool navRight = false;
    public bool NavRight { get { return navRight; } set { navRight = value; } }

    private bool navLeft = false;
    public bool NavLeft { get { return navLeft; } set { navLeft = value; } }

    [SerializeField]
    private PlayerInput PI;
    [SerializeField]
    private PlayerBody PB;

    private int ID;
    public int id
    {
        get { return ID; }
    }

    private void Awake()
    {
        ID = PI.playerIndex;
    }

    private void LateUpdate()
    {
        primaryAttackDown = false;
        executeDown = false;
        dashDown = false;
        pauseDown = false;
    }

    public void OnMove(InputValue inputValue)
    {
        Vector2 inputVal = inputValue.Get<Vector2>();

        forwardMagnitude = inputVal.y;
        horizontalMagnitude = inputVal.x;
        Transform sprite = PB.CharacterFolder.transform.GetChild(0);
        float scale = Mathf.Abs(sprite.localScale.x);
        if (PB.canMove)
        {
            if (inputVal.x > 0) { sprite.localScale = new Vector3(-scale, sprite.localScale.y, 1); }
            else if (inputVal.x < 0) { PB.CharacterFolder.transform.GetChild(0).localScale = new Vector3(scale, sprite.localScale.y, 1); }
        }
    }

    public void OnPrimaryAttack()
    {
        primaryAttackDown = true;
    }

    public void OnExecute()
    {
        executeDown = true;
    }

    public void OnDash()
    {
        dashDown = true;
    }

    public void OnPause()
    {
        pauseDown = true;
        Debug.Log("Pause Down");
    }

    public void OnNavigate(InputValue inputValue)
    {
        Vector2 inputVal = inputValue.Get<Vector2>();
        if (inputVal.x > 0) { navRight = true; }
        else if (inputVal.x < 0) { navLeft = true; }
    }

}
